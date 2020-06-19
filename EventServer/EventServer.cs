using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using XDEM.Shared;
using System.Collections.Immutable;

using static XDEM.Shared.Constants;

namespace XDEM.Server.Windows
{
    class EventServer
    {
        private readonly static int PORT = 10451;
        private readonly static BlockingCollection<Event> messages = new BlockingCollection<Event>(new ConcurrentQueue<Event>());
        private readonly static ConcurrentDictionary<StreamWriter, ImmutableHashSet<string>> eventListeners = new ConcurrentDictionary<StreamWriter, ImmutableHashSet<string>>();
        private readonly static Thread messageThread = new Thread(HandleMessages);
        private readonly static TcpListener serverSocket = TcpListener.Create(PORT);

        static void Main(string[] args)
        {
            messageThread.Start();
            serverSocket.Start();

            Console.WriteLine("Listening for client");
            for (int i = 0; i < 5; ++i) {
                var client = serverSocket.AcceptTcpClient();
                Console.WriteLine("New connection " + i);
                CreateEventListenerThread(client).Start();
            }

            serverSocket.Stop();
            messageThread.Abort();
            messageThread.Join();
        }

        static void HandleMessages()
        {
            while(Thread.CurrentThread.IsAlive)
            {
                Event ev = messages.Take();

                foreach (var listener in eventListeners.Keys)
                {
                    if (eventListeners[listener].Contains(ev.Name))
                    {
                        string msg = JsonConvert.SerializeObject(ev);
                        listener.WriteLine(msg);
                        listener.Flush();
                    }
                }
            }
            
        }

        static Thread CreateEventListenerThread(TcpClient client)
        {
            var reader = new StreamReader(client.GetStream(), Encoding.UTF8);
            var writer = new StreamWriter(client.GetStream(), Encoding.UTF8);
            return new Thread(() =>
            {
                try
                {
                    Subscription subscription = JsonConvert.DeserializeObject<Subscription>(reader.ReadLine().Trim());
                    eventListeners[writer] = subscription.Events.ToImmutableHashSet();

                    Console.WriteLine("After insertion dictionary contains");
                    PrintDictionary();

                    while (Thread.CurrentThread.IsAlive)
                    {
                        string line = reader.ReadLine().Trim();
                        Event e = JsonConvert.DeserializeObject<Event>(line);

                        Console.WriteLine(line + " " + e.ToString());

                        messages.Add(e);
                    }
                }
                catch (IOException ioException)
                {
                    // Cleanup lost connection
                } 
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e);
                } 
                finally
                {
                    eventListeners.TryRemove(writer, out _);
                    Console.WriteLine("After deletion dictionary contains");
                    PrintDictionary();
                    client.Close();
                }
            });
        }

        private static void PrintDictionary()
        {
            foreach (var key in eventListeners.Keys)
            {
                Console.Write("\t" + key.ToString());
                foreach (var item in eventListeners[key])
                {
                    Console.Write(" - " + item);
                }
                Console.WriteLine();
            }
        }

        static Thread CreateEchoThread(TcpClient client)
        {
            return new Thread(() => { 
                try
                {
                    var stream = client.GetStream();
                    var reader = new StreamReader(stream, Encoding.UTF8);
                    var writer = new StreamWriter(stream, Encoding.UTF8);

                    string line = null;
                    do
                    {                       
                        line = reader.ReadLine().Trim();
                        dynamic msg = JsonConvert.DeserializeObject(line);
                        
                        Console.WriteLine("Received message: " + msg);
                        writer.WriteLine(line);
                        writer.Flush();
                    } while (line != "end");
                } catch (IOException) { }
                catch(Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e);
                }
                finally
                {
                    client.Close();
                }
            });
        }
    }
}
