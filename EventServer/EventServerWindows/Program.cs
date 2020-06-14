using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace CDEM.EventServer.Windows
{
    class Program
    {
        private static ConcurrentQueue<string> messages = new ConcurrentQueue<string>();
        private readonly static int port = 10451;

        static void Main(string[] args)
        {
            var serverSocket = TcpListener.Create(port);
            serverSocket.Start();

            for (int i = 0; i < 5; ++i) {
                Console.WriteLine("Listening for client");
                var client = serverSocket.AcceptTcpClient();
                Console.WriteLine("New connection " + i);
                //CreateEventListenerThread(client).Start();
                CreateEchoThread(client).Start();
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
                        dynamic msg = JsonConvert.DeserializeObject(reader.ReadLine().Trim());
                        Console.WriteLine(msg);
                        // Do Stuff
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
