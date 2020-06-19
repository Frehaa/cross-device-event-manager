using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using XDEM.Shared;

using static XDEM.Shared.Constants;
using System.Threading;
using System.Diagnostics;
using NAudio.Wave;

namespace XDEM.Client.Windows
{
    class EventClient
    {
        private static readonly int PORT = 10451;
        private static readonly TcpClient client = new TcpClient();

        static void Main(string[] args)
        {
            Thread.Sleep(1000);

            client.Connect(new IPEndPoint(IPAddress.Loopback, PORT));

            if (client.Connected)
            {
                var reader = new StreamReader(client.GetStream(), Encoding.UTF8);
                var writer = new StreamWriter(client.GetStream(), Encoding.UTF8);

                var subscriptionMessage = new Subscription(new string[] { SHUTDOWN_DESKTOP_COMMAND, SMS_RECEIVED, PHONE_CALL_INCOMING });
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
                    //Shutdown();
                    break;
                case SMS_RECEIVED:
                    SmsReceived(ev.Arguments);
                    break;
                case PHONE_CALL_INCOMING:
                    PhoneCallIncoming(ev.Arguments);
                    break;
                default:
                    Console.WriteLine("Error: Received event which wasn't subscribed for");
                    break;
            }
        }

        private static void Shutdown()
        {
            client.Close();
            Process.Start("shutdown", "/s /e /t 0");
        }
        private static void SmsReceived(dynamic[] args)
        {
            long sender = args[0];
            string message = args[1];

            PlaySound(@"D:\Music\shelter.wav", 2000);

            string logPath = "./sms_log.txt";

            using var fileStream = new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.Read);
            var writer = new StreamWriter(fileStream);
            
            string logMessage = $"Received message: \"{message}\" - from {sender}";

            writer.WriteLine(logMessage);
            writer.Flush();
        }

        private static void PhoneCallIncoming(dynamic[] arguments)
        {
            PlaySound(@"D:\Music\senbonzakura.wav", 10_000);
        }

        private static void PlaySound(string waveFile, int duration)
        {
            new Thread(() =>
            {
                using var waveOut = new WaveOutEvent();
                using var wavReader = new WaveFileReader(waveFile);
                waveOut.Init(wavReader);
                waveOut.Play();
                Thread.Sleep(duration);
                waveOut.Stop();
            }).Start();
        }
    }
}
