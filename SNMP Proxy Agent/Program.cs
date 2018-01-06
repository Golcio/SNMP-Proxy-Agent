using SnmpSharpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SNMP_Proxy_Agent
{
    static class Program
    {
        static public string address;
        static public int port;
        static public string community;
        static public bool connected = false;
        static public bool doRunSocketThread = true;
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
            while (doRunSocketThread != true)
            {
                serverSocket.Listen(0);
                Socket clientSocket = serverSocket.Accept();

                connected = true;
                byte[] buffer = new byte[serverSocket.SendBufferSize];

                int readByte;
                do
                {
                    try
                    {
                        readByte = clientSocket.Receive(buffer);
                    }
                    catch (Exception ex)
                    {
                        break;
                    }
                    byte[] rData = new byte[readByte];
                    Array.Copy(buffer, rData, readByte);

                    string message = System.Text.Encoding.UTF8.GetString(rData);
                    string[] messageArray = message.Split('|');
                    string output = null;
                    if (messageArray[0].Equals("get"))
                    {
                        List<string> oids = new List<string>();
                        oids.Add(messageArray[1]);
                        Dictionary<Oid, AsnType> results = getResult(oids);

                        if (results != null)
                        {
                            foreach (KeyValuePair<Oid, AsnType> kvp in results)
                            {
                                string value = "." + kvp.Key.ToString();

                                StringBuilder sb = new StringBuilder();
                                sb.Append(value);
                                sb.Append("|");
                                sb.Append(kvp.Value.ToString());
                                sb.Append("|");
                                sb.Append(SnmpConstants.GetTypeName(kvp.Value.Type));
                                sb.Append("|");
                                sb.Append(address);
                                output = sb.ToString();
                            }
                        }
                        else
                            output = "No results.";
                    }
                    clientSocket.Send(System.Text.Encoding.UTF8.GetBytes(output));
                }
                while (readByte > 0);
                connected = false;
            }
            serverSocket.Close();
            socketThread.Abort();
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
            doRunSocketThread = false;
        }

        public static Dictionary<Oid, AsnType> getResult(List<string> oids)
        {
            SimpleSnmp snmp;
            try
            {
                snmp = new SimpleSnmp("localhost", community);
                if (!snmp.Valid)
                {
                    MessageBox.Show("SNMP agent host name/IP address is invalid.");
                    return null;
                }
                Dictionary<Oid, AsnType> result = snmp.Get(SnmpVersion.Ver1, oids.ToArray());
                if (result == null)
                {
                    MessageBox.Show("No results received.");
                    return null;
                }

                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show("SNMP agent host name/IP address is invalid.");
                return null;
            }
        }

    }
}
