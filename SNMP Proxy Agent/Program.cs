using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SNMP_Proxy_Agent
{
    static class Program
    {
        static public string address;
        static public int port;
        static public bool connected = false;
        static Socket serverSocket;
        static Thread socketThread;
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        public static void initializeSocket()
        {
            socketThread = new Thread(() => createSocket());
            socketThread.Start();
        }

        public static void createSocket()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(address), port);
            serverSocket.Bind(ipEnd);
            serverSocket.Listen(0);
            Socket clientSocket = serverSocket.Accept();

            connected = true;
            byte[] buffer = new byte[serverSocket.SendBufferSize];

            int readByte;
            do
            {
                readByte = clientSocket.Receive(buffer);
                byte[] rData = new byte[readByte];
                Array.Copy(buffer, rData, readByte);

                string message = System.Text.Encoding.UTF8.GetString(rData);
                string[] array = message.Split('|');
                MessageBox.Show(array[0] + " " + array[1]);
            }
            while (readByte > 0);
            connected = false;
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static void stopServerThread()
        {
            socketThread.Abort();
            connected = false;
        }

    }
}
