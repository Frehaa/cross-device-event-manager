using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace EventManager
{
    class Program
    {
        private readonly static int port = 10451;
        static void Main(string[] args)
        {
            var serverSocket = TcpListener.Create(port);
            serverSocket.Start();

            Console.WriteLine("Accept clients");
            using (var client = serverSocket.AcceptTcpClient())
            {
                Console.WriteLine("New connection");

                var stream = client.GetStream();
                var reader = new StreamReader(stream, Encoding.UTF8);
                var writer = new StreamWriter(stream, Encoding.UTF8);

                var data = reader.ReadLine();

                Console.Write("Received message: " + data);

                writer.Write(data);
                writer.Flush();
            }
        }
    }
}
