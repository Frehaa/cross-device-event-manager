using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using CDEM.Shared;

using static CDEM.Shared.Constants;

namespace CDEM.Client.Windows
{
    class EventClient
    {
        private static readonly int PORT = 10451;
        private static readonly TcpClient client = new TcpClient();

        static void Main(string[] args)
        {
            client.Connect(new IPEndPoint(IPAddress.Loopback, PORT));

            if (client.Connected)
            {
                var reader = new StreamReader(client.GetStream(), Encoding.UTF8);
                var writer = new StreamWriter(client.GetStream(), Encoding.UTF8);

                var subscriptionMessage = new Subscription(new string[] { SHUTDOWN_DESKTOP_COMMAND, SMS_RECEIVED });
                string json = JsonConvert.SerializeObject(subscriptionMessage);

                Console.WriteLine("Subscribing to " + json);

                writer.WriteLine(json);
                writer.Flush();

                while (true)
                {
                    var line = reader.ReadLine().Trim();
                    Event ev = JsonConvert.DeserializeObject<Event>(line);
                    HandleEvent(ev);

                    Console.WriteLine("Server output: " + line);
                }

            }

        }

        static void HandleEvent(Event ev)
        {
            switch (ev.Name)
            {
                case SHUTDOWN_DESKTOP_COMMAND:
                    break;
                case SMS_RECEIVED:
                    break;
                default:
                    Console.WriteLine("Error: Received event which wasn't subscribed for");
                    break;
            }
        }

    }
}
