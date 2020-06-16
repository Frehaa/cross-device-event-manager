using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using CDEM.Shared;

namespace CDEM.Server.Windows
{
    class EventServer
    {
        private readonly static int TIMEOUT = 2000;
        private readonly static int PORT = 10451;
        private readonly static BlockingCollection<Event> messages = new BlockingCollection<Event>(new ConcurrentQueue<Event>());
        private readonly static ConcurrentDictionary<string, ConcurrentBag<StreamWriter>> eventListeners = new ConcurrentDictionary<string, ConcurrentBag<StreamWriter>>();
        private readonly static Thread messageThread = new Thread(handleMessages);
        private readonly static TcpListener serverSocket = TcpListener.Create(PORT);

        static void Main(string[] args)
        {
            messageThread.Start();
            serverSocket.Start();

            for (int i = 0; i < 5; ++i) {
                Console.WriteLine("Listening for client");
                var client = serverSocket.AcceptTcpClient();
                Console.WriteLine("New connection " + i);
                CreateEventListenerThread(client).Start();
                //CreateEchoThread(client).Start();
            }
        }

        static void handleMessages()
        {
            while(Thread.CurrentThread.IsAlive)
            {
                Event e = messages.Take();

                var listeners = eventListeners.GetOrAdd(e.Name, new ConcurrentBag<StreamWriter>());

                foreach (var listener in listeners)
                {
                    listener.WriteLine(e.Name);
                }
            }
            
        }

        static Thread CreateEventListenerThread(TcpClient client)
        {
            var reader = new StreamReader(client.GetStream(), Encoding.UTF8);
            return new Thread(() =>
            {
                try
                {
                    // Read initial connection message
                    //dynamic msg = JsonConvert.DeserializeObject(reader.ReadLine().Trim());

                    // Append this connection dictionary of event to output stream for each event subscribed to

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
                    reader.Close();
                }
            });
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
