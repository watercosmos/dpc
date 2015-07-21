using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Threading.Tasks;



namespace WindowsService2
{
    public partial class Service1 : ServiceBase
    {
        string defaultpath = "D:\\wfa";
        string path = "D:\\wfa";
        List<Connection> connection = new List<Connection>();
        Socket serverSocket;
        public Service1()
        {
            InitializeComponent();
        }
        public void refresh(object source, System.Timers.ElapsedEventArgs e)
        {
            path = File.ReadAllText(defaultpath + "\\" + "pathfile.txt");
        }
        protected override void OnStart(string[] args)
        {
            if (!Directory.Exists(defaultpath))
            {
                // Create the directory it does not exist.
                Directory.CreateDirectory(defaultpath);
            }
            if (!File.Exists(defaultpath + "\\" + "pathfile.txt"))
            {
                StreamWriter MyWriter = new StreamWriter(defaultpath + "\\" + "pathfile.txt");
                MyWriter.Write(path + "\\");
                MyWriter.Flush();
                MyWriter.Close();
            }
            path = File.ReadAllText(defaultpath + "\\" + "pathfile.txt");
            IPAddress ip = IPAddress.Any;
            if (!PortInUse(11235))
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                serverSocket.Bind(new IPEndPoint(ip, 11235));
                serverSocket.Listen(50);
                Thread th = new Thread(server);
                th.Start(serverSocket);
            }

            MyTimer(); 
        }
        private void MyTimer()
        {
            System.Timers.Timer MT = new System.Timers.Timer(300);
            MT.Elapsed += new System.Timers.ElapsedEventHandler(refresh);
            MT.Enabled = true;

        }
        public static bool PortInUse(int port)
        {
            bool inUse = false;

            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();

            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port)
                {
                    inUse = true;
                    break;
                }
            }

            return inUse;
        }

        protected override void OnStop()
        {
        }
        private void server(Object sk)
        {
            while (true)
            {
                Socket s = (Socket)sk;
                Socket clientSocket = s.Accept();
                Thread reciveTh = new Thread(oneConnection);
                reciveTh.Start(clientSocket);
            }
        }
        private void oneConnection(Object clientSocket)
        {
            byte[] rx_buf = new byte[128];
            if (clientSocket != null)
            {
                int step = 1;
                int length = 11;
                byte[] result = new byte[1];
                Socket myClientSocket = (Socket)clientSocket;
                string TS = "";
                byte st = 0x7e, end = 0x5a;
                int type = -1, addr = -1;

                for (; ; )
                {
                    int ret = myClientSocket.Receive(result, 1, 0);
                    if (ret == 0)
                    {
                        if (type >= 0 && addr >= 0)
                        {
                            foreach (Connection c in connection)
                            {
                                if (c.type == type && c.addr == addr)
                                {
                                    connection.Remove(c);
                                    break;
                                }
                            }
                            string con = "";
                            foreach (Connection co in connection)
                            {
                                con += "类型 " + co.type + " 地址 " + co.addr + "\n";
                            }

                            StreamWriter conn = new StreamWriter(defaultpath + "\\" + "connecting" + ".txt");
                            conn.Write(con);
                            conn.Flush();
                            conn.Close();
                        }
                        return;
                    }

                    if (step == 1)
                    {
                        if (result[0] == 0x38)
                            break;
                        if (result[0] == st)
                            ++step;
                        continue;
                    }
                    if (step == 2)
                        length += result[0] * 256;

                    if (step == 3)
                        length += result[0];

                    if (step == 4)
                    {
                        type = (result[0] >> 4) & 0x0F;
                        addr = (result[0] & 0x0F) * 256;
                    }

                    if (step == 5)
                        addr += result[0];

                    if (step < length)
                        rx_buf[step - 2] = result[0];
                    else if (step == length)
                    {
                        if (result[0] == end)
                            TS = rx_handler(rx_buf, length);

                        step = 1;
                        length = 11;
                        continue;
                    }
                    ++step;
                }

            }
        }
        private string rx_handler(byte[] rx_buf, int length)
        {
            if (!check_crc(rx_buf, length))
                return "";

            //store in file

            if (length < 29)
            {
                rx_buf[27] = rx_buf[length - 4];
                rx_buf[28] = rx_buf[length - 3];

                for (int i = length - 4; i < 27; ++i)
                {
                    rx_buf[i] = 0x00;
                }
            }

            Frame frame = new Frame();

            frame.length = length - 11;
            frame.src_type = (rx_buf[2] >> 4) & 0x0F;
            frame.src_addr = (rx_buf[2] & 0x0F) * 256 + rx_buf[3];
            frame.dest_type = (rx_buf[4] >> 4) & 0x0F;
            frame.dest_addr = (rx_buf[4] & 0x0F) * 256 + rx_buf[5];
            frame.type = rx_buf[6];

            for (int i = 0; i < 5; ++i)
                frame.data1[i] = rx_buf[7 + i];

            for (int i = 0, j = 0, k = 12; j < 2; ++k)
            {
                frame.data2[j, i] = rx_buf[k];
                ++i;
                if (i == 2)
                {
                    ++j;
                    i = 0;
                }
            }

            string TS = "类型 " + frame.src_type.ToString() + " 地址 " + frame.src_addr.ToString();
            // add(TS);
            bool a = false;
            foreach (Connection co in connection)
            {
                if (co.type == frame.src_type && co.addr == frame.src_addr)
                { a = true; }
            }
            if (!a)
            {
                Connection thisConnection = new Connection();
                thisConnection.type = frame.src_type;
                thisConnection.addr = frame.src_addr;
                connection.Add(thisConnection);

                string con = "";
                foreach (Connection co in connection)
                {
                    con += "类型 " + co.type + " 地址 " + co.addr + "\n";
                }

                StreamWriter conn = new StreamWriter(defaultpath + "\\" + "connecting" + ".txt");
                conn.Write(con);
                conn.Flush();
                conn.Close();
            }


            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < frame.length; i++)
                sb.Append(rx_buf[7 + i].ToString("X2") + " ");

            string b = sb.ToString();
            string e = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            System.DateTime currentTime = new System.DateTime();
            currentTime = System.DateTime.Now;
            int y = currentTime.Year;
            int m = currentTime.Month;
            int day = currentTime.Day;

            StreamWriter MyWriter = new StreamWriter(path + "\\" + TS + "." + y + "年" + m + "月" + day + "日" + ".txt", true, Encoding.UTF8);

            MyWriter.Write(b + "- " + e + "\n");
            MyWriter.Flush();
            MyWriter.Close();


            StreamWriter MyWriter1 = new StreamWriter(defaultpath + "\\" + TS + "buffer" + ".txt", false, Encoding.UTF8);

            MyWriter1.Write(b + "- " + e + "\n");
            MyWriter1.Flush();
            MyWriter1.Close();

            return TS;
        }
        public ushort ComputeChecksum(byte[] bytes)
        {
            ushort crc = 0xFFFF;
            const ushort poly = 4129;
            ushort[] table = new ushort[256];
            ushort temp, a;
            for (int i = 0; i < table.Length; ++i)
            {
                temp = 0;
                a = (ushort)(i << 8);
                for (int j = 0; j < 8; ++j)
                {
                    if (((temp ^ a) & 0x8000) != 0)
                    {
                        temp = (ushort)((temp << 1) ^ poly);
                    }
                    else
                    {
                        temp <<= 1;
                    }
                    a <<= 1;
                }
                table[i] = temp;
            }
            for (int i = 0; i < bytes.Length; ++i)
            {
                crc = (ushort)((crc << 8) ^ table[((crc >> 8) ^ (0xff & bytes[i]))]);
            }
            return crc;
        }

        bool check_crc(byte[] rx_buf, int length)
        {
            List<byte> list = new List<byte>();
            for (int i = 0; i < length - 4; i++)
            {
                list.Add(rx_buf[i]);
            }
            ushort culcrc = ComputeChecksum(list.ToArray());
            ushort crc = (ushort)(rx_buf[length - 4] * 256 + rx_buf[length - 3]);
            if (crc == culcrc)
                return true;
            else
                return false;
        }
        public class Frame
        {
            public int length;  //从帧中取得， 该长度包括帧头帧尾
            public int src_type;
            public int src_addr;
            public int dest_type;
            public int dest_addr;
            public int type;
            public byte[] data1 = new byte[5];
            public byte[,] data2 = new byte[7, 2];
        }

        public class Connection
        {
            public int type;
            public int addr;
        }


    }
}


