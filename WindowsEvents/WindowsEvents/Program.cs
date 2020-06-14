using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CDEM.Events.Windows
{
    class Program
    {
        private static readonly int port = 10451;
        static void Main(string[] args)
        {
            TcpClient client = new TcpClient();

            client.Connect(new IPEndPoint(IPAddress.Loopback, port));

            Console.WriteLine(client.Connected);
            if (client.Connected)
            {
                var reader = new StreamReader(client.GetStream(), Encoding.UTF8);
                var writer = new StreamWriter(client.GetStream(), Encoding.UTF8);

                var msg = "Hello!";
                Console.WriteLine("My message: " + msg);
                writer.WriteLine(msg);
                writer.Flush();

                var line = reader.ReadLine();

                Console.WriteLine("Server output: " + line);
            }

        }
    }
}
