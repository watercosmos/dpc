using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
namespace dpc
{
    public partial class Form1 : Form
    {
        string defaultpath = "D:\\wfa";
        string path = "D:\\wfa";
        List<Connection> connection = new List<Connection>();
        Connection requestTDT = new Connection();
        int columnRefreshFlag = 1;
        string c1;
        List<string> readtdts = new List<string>();
        string lastbuffer = "";
        string show;
        StringBuilder strB = new StringBuilder();
        string aa;
        string VG7 = "0",
               VG8 = "0",
               WHMP = "0",
               WHMB = "0",
               OPRA = "0",
               MCOS = "0",
               HBD = "0",
               CFD = "0",
               PUD = "0",
               GPLVD = "0",
               P5LVD = "0",
               P15LVD = "0",
               M15LVD = "0",
               P24LVD = "0",
               WDTD = "0",
               THD = "0",
               TROPRA = "0",
               LOFD = "0",

               PLVD = "0",
               BCFD = "0",
               BCOCD = "0",
               MMOCD = "0",
               P110LVD = "0",
               OVD1 = "0",
               OVD2 = "0",
               FCLVD = "0",
               ESLVD = "0",
               LGD = "0",
               PGD = "0",
               WSD = "0",
               BSD = "0",
               OBTD = "0",
               FCD = "0",
               LOND = "0",

               BCTHD = "0",
               BRTHD = "0",
               PGD1 = "0",
               PGD2 = "0",
               PGD3 = "0",
               PGD4 = "0",
               BCW = "0",
               TIG = "0",
               THR = "0",
               TIG1 = "0",
               TIG2 = "0",
               THR1 = "0",
               THR2 = "0";

        string VG7l = "0",
              VG8l = "0",
              WHMPl = "0",
              WHMBl = "0",
              OPRAl = "0",
              MCOSl = "0",
              HBDl = "0",
              CFDl = "0",
              PUDl = "0",
              GPLVDl = "0",
              P5LVDl = "0",
              P15LVDl = "0",
              M15LVDl = "0",
              P24LVDl = "0",
              WDTDl = "0",
              THDl = "0",
              TROPRAl = "0",
              LOFDl = "0",

              PLVDl = "0",
              BCFDl = "0",
              BCOCDl = "0",
              MMOCDl = "0",
              P110LVDl = "0",
              OVD1l = "0",
              OVD2l = "0",
              FCLVDl = "0",
              ESLVDl = "0",
              LGDl = "0",
              PGDl = "0",
              WSDl = "0",
              BSDl = "0",
              OBTDl = "0",
              FCDl = "0",
              LONDl = "0",

              BCTHDl = "0",
              BRTHDl = "0",
              PGD1l = "0",
              PGD2l = "0",
              PGD3l = "0",
              PGD4l = "0",
              BCWl = "0",
              TIGl = "0",
              THRl = "0",
              TIG1l = "0",
              TIG2l = "0",
              THR1l = "0",
              THR2l = "0";
        public Form1()
        {
            Form2 f2 = new Form2();
            f2.Show();//show出欢迎窗口
            System.Threading.Thread.Sleep(2000);//欢迎窗口停留时间2s
            f2.Close();//关闭欢迎窗口并开始运行主窗口
            InitializeComponent();
            System.Timers.Timer t = new System.Timers.Timer(100);
            t.Elapsed += new System.Timers.ElapsedEventHandler(refresh);
            t.AutoReset = true;
            t.Enabled = true;
        }
        public void refresh(object source, System.Timers.ElapsedEventArgs e)
        {
            if (File.Exists(defaultpath + "\\" + "connecting" + ".txt"))
            {
                string line = "";
                int i = 0;
                readtdts.Clear();
                FileStream fs = new FileStream(defaultpath + "\\" + "connecting" + ".txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs);
                while ((line = sr.ReadLine()) != null)
                {
                    readtdts.Add(line);
                    add(line);
                    i++;//计数,总共几行
                }
                sr.Close();
            }
            DirectoryInfo TheFolder = new DirectoryInfo(path);
            FileInfo[] tas = TheFolder.GetFiles();
            for (int j = 0; j < tas.Length; j++)
            {
                if (tas[j].Name.Length > 20)
                    addTypeAndAdd("类型 " + tas[j].Name.Substring(3, 1) + " 地址 " + tas[j].Name.Substring(8, 1));
            }
            for (int i = 0; i < comboBox1.Items.Count; i++)
            {
                bool co = false;
                foreach (string re in readtdts)
                {
                    if (string.Compare(re, comboBox1.GetItemText(comboBox1.Items[i])) == 0)
                        co = true;
                }
                if (!co)
                {
                    comboBox1.Items.Remove(comboBox1.Items[i]);
                }
            }

            string read = File.ReadAllText(defaultpath + "\\" + comboBox1.Text + "buffer" + ".txt");
            if (read != lastbuffer)
            {
                listBox1.Items.Add(read);
                lastbuffer = read;
                if (columnRefreshFlag == 1)
                {
                    ListBoxAutoCroll(listBox1);
                    clickRealtimeList(read);
                }
            }
        }

        private void ServerLoad(object sender, EventArgs e)
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
        }

        /*public static bool PortInUse(int port)
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
        }*/

        private void add(string text)
        {
            //Connection thisConnection = new Connection();
            //thisConnection.type = type;
            //thisConnection.addr = addr;
            //connection.Add(thisConnection);
            for (int i = 0; i < comboBox1.Items.Count; i++)
            {
                if (string.Compare(comboBox1.GetItemText(comboBox1.Items[i]), text) == 0)
                    return;
            }
            comboBox1.Items.Add(text);
        }

        private void addTypeAndAdd(string text)
        {

            for (int i = 0; i < comboBox2.Items.Count; i++)
            {
                if (string.Compare(comboBox2.GetItemText(comboBox2.Items[i]), text) == 0)
                    return;
            }
            comboBox2.Items.Add(text);
        }

        private void remove(string text)
        {
            comboBox1.Items.Remove(text);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                listBox1.SetSelected(0, false);
                columnRefreshFlag = 1;
            }
            show = comboBox1.Text;
            listBox1.Items.Clear();
        }

        private void SetText(string text)
        {
            listBox1.Items.Add(text);
            if (columnRefreshFlag == 1)
            {
                ListBoxAutoCroll(listBox1);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                clickRealtimeList(listBox1.SelectedItem.ToString());
                columnRefreshFlag = 0;
            }
        }

        private void convertHextoBin(string hex)
        {
            int b = Convert.ToInt32(hex, 16);
            string c = Convert.ToString(b, 2);
            int d = Convert.ToInt32(c);
            c1 = string.Format("{0:00000000}", d);
        }

        private void clickRealtimeList(string la)
        {
            aa = la;
            string a1, a2, a3, a4, a5;
            convertHextoBin(aa.Substring(0, 2));
            a1 = c1;
            convertHextoBin(aa.Substring(3, 2));
            a2 = c1;
            convertHextoBin(aa.Substring(6, 2));
            a3 = c1;
            convertHextoBin(aa.Substring(9, 2));
            a4 = c1;
            convertHextoBin(aa.Substring(12, 2));
            a5 = c1;

            byte m00 = Convert.ToByte(aa.Substring(15, 2), 16);
            byte m01 = Convert.ToByte(aa.Substring(18, 2), 16);

            byte m10 = Convert.ToByte(aa.Substring(21, 2), 16);
            byte m11 = Convert.ToByte(aa.Substring(24, 2), 16);

            TIG1 = (((int)((m00 >> 7) & 0x01) * (-0x8000)) + ((int)(m00 & 0x7F) * 0x0100) + ((int)m01)).ToString();
            TIG2 = (((int)((m10 >> 7) & 0x01) * (-0x8000)) + ((int)(m10 & 0x7F) * 0x0100) + ((int)m11)).ToString();

            VG7 = a1.Substring(0, 1);
            VG8 = a1.Substring(1, 1);
            WHMP = a1.Substring(2, 1);
            WHMB = a1.Substring(3, 1);
            OPRA = a1.Substring(4, 1);
            MCOS = a1.Substring(5, 1);
            HBD = a1.Substring(6, 1);
            CFD = a1.Substring(7, 1);
            PUD = a2.Substring(0, 1);
            GPLVD = a2.Substring(1, 1);

            P5LVD = a2.Substring(2, 1);
            P15LVD = a2.Substring(3, 1);
            M15LVD = a2.Substring(4, 1);
            P24LVD = a2.Substring(5, 1);
            WDTD = a2.Substring(6, 1);
            THD = a2.Substring(7, 1);
            TROPRA = a3.Substring(0, 1);
            LOFD = a3.Substring(1, 1);

            PLVD = a3.Substring(2, 1);

            BCFD = a3.Substring(3, 1);
            BCOCD = a3.Substring(4, 1);
            MMOCD = a3.Substring(5, 1);
            P110LVD = a3.Substring(6, 1);
            OVD1 = a3.Substring(7, 1);
            OVD2 = a4.Substring(0, 1);
            FCLVD = a4.Substring(1, 1);
            ESLVD = a4.Substring(2, 1);
            LGD = a4.Substring(3, 1);
            PGD = a4.Substring(4, 1);

            WSD = a4.Substring(5, 1);
            BSD = a4.Substring(6, 1);
            OBTD = a4.Substring(7, 1);
            FCD = a5.Substring(0, 1);
            LOND = a5.Substring(1, 1);

            BCTHD = a5.Substring(2, 1);
            BRTHD = a5.Substring(3, 1);
            PGD1 = a5.Substring(4, 1);
            PGD2 = a5.Substring(5, 1);
            PGD3 = a5.Substring(6, 1);
            PGD4 = a5.Substring(7, 1);

            label41.Text = VG7;
            label42.Text = VG8;
            label43.Text = WHMP;
            label44.Text = WHMB;
            label45.Text = OPRA;
            label46.Text = MCOS;
            label47.Text = HBD;
            label48.Text = CFD;
            label49.Text = PUD;
            label50.Text = GPLVD;

            label51.Text = P5LVD;
            label52.Text = P15LVD;
            label53.Text = M15LVD;
            label54.Text = P24LVD;
            label55.Text = WDTD;
            label56.Text = THD;
            label57.Text = TROPRA;
            label58.Text = LOFD;

            label59.Text = PLVD;

            label60.Text = BCFD;
            label61.Text = BCOCD;
            label62.Text = MMOCD;
            label63.Text = P110LVD;
            label64.Text = OVD1;
            label65.Text = OVD2;
            label66.Text = FCLVD;
            label67.Text = ESLVD;
            label68.Text = LGD;
            label69.Text = PGD;

            label70.Text = WSD;
            label71.Text = BSD;
            label72.Text = OBTD;
            label73.Text = FCD;
            label74.Text = LOND;

            label75.Text = BCTHD;
            label76.Text = BRTHD;
            label77.Text = PGD1;
            label78.Text = PGD2;
            label79.Text = PGD3;
            label80.Text = PGD4;

            label88.Text = BCW;
            label89.Text = TIG;
            label90.Text = THR;
            label91.Text = TIG1;
            label92.Text = TIG2;
            label93.Text = THR1;
            label94.Text = THR2;

            if (label41.Text == "1")
            {
                panel1.BackColor = Color.LightSkyBlue;
            }
            if (label42.Text == "1")
            {
                panel2.BackColor = Color.LightSkyBlue;
            }
            if (label43.Text == "1")
            {
                panel3.BackColor = Color.LightSkyBlue;
            }
            if (label44.Text == "1")
            {
                panel4.BackColor = Color.LightSkyBlue;
            }
            if (label45.Text == "1")
            {
                panel5.BackColor = Color.LightSkyBlue;
            }
            if (label46.Text == "1")
            {
                panel6.BackColor = Color.LightSkyBlue;
            }
            if (label47.Text == "1")
            {
                panel7.BackColor = Color.LightSkyBlue;
            }
            if (label48.Text == "1")
            {
                panel8.BackColor = Color.LightSkyBlue;
            }
            if (label49.Text == "1")
            {
                panel9.BackColor = Color.LightSkyBlue;
            }

            if (label50.Text == "1")
            {
                panel10.BackColor = Color.LightSkyBlue;
            }
            if (label51.Text == "1")
            {
                panel11.BackColor = Color.LightSkyBlue;
            }
            if (label52.Text == "1")
            {
                panel12.BackColor = Color.LightSkyBlue;
            }
            if (label53.Text == "1")
            {
                panel13.BackColor = Color.LightSkyBlue;
            }
            if (label54.Text == "1")
            {
                panel14.BackColor = Color.LightSkyBlue;
            }
            if (label55.Text == "1")
            {
                panel15.BackColor = Color.LightSkyBlue;
            }
            if (label56.Text == "1")
            {
                panel16.BackColor = Color.LightSkyBlue;
            }
            if (label57.Text == "1")
            {
                panel17.BackColor = Color.LightSkyBlue;
            }
            if (label58.Text == "1")
            {
                panel18.BackColor = Color.LightSkyBlue;
            }
            if (label59.Text == "1")
            {
                panel19.BackColor = Color.LightSkyBlue;
            }
            if (label60.Text == "1")
            {
                panel20.BackColor = Color.LightSkyBlue;
            }
            if (label61.Text == "1")
            {
                panel21.BackColor = Color.LightSkyBlue;
            }
            if (label62.Text == "1")
            {
                panel22.BackColor = Color.LightSkyBlue;
            }
            if (label63.Text == "1")
            {
                panel23.BackColor = Color.LightSkyBlue;
            } if (label64.Text == "1")
            {
                panel24.BackColor = Color.LightSkyBlue;
            }
            if (label65.Text == "1")
            {
                panel25.BackColor = Color.LightSkyBlue;
            }
            if (label66.Text == "1")
            {
                panel26.BackColor = Color.LightSkyBlue;
            }
            if (label67.Text == "1")
            {
                panel27.BackColor = Color.LightSkyBlue;
            }
            if (label68.Text == "1")
            {
                panel28.BackColor = Color.LightSkyBlue;
            }
            if (label69.Text == "1")
            {
                panel29.BackColor = Color.LightSkyBlue;
            }
            if (label70.Text == "1")
            {
                panel30.BackColor = Color.LightSkyBlue;
            } if (label71.Text == "1")
            {
                panel31.BackColor = Color.LightSkyBlue;
            }
            if (label72.Text == "1")
            {
                panel32.BackColor = Color.LightSkyBlue;
            }
            if (label73.Text == "1")
            {
                panel33.BackColor = Color.LightSkyBlue;
            }
            if (label74.Text == "1")
            {
                panel34.BackColor = Color.LightSkyBlue;
            }
            if (label75.Text == "1")
            {
                panel35.BackColor = Color.LightSkyBlue;
            }
            if (label76.Text == "1")
            {
                panel36.BackColor = Color.LightSkyBlue;
            }
            if (label77.Text == "1")
            {
                panel37.BackColor = Color.LightSkyBlue;
            }
            if (label78.Text == "1")
            {
                panel38.BackColor = Color.LightSkyBlue;
            }
            if (label79.Text == "1")
            {
                panel39.BackColor = Color.LightSkyBlue;
            }
            if (label80.Text == "1")
            {
                panel40.BackColor = Color.LightSkyBlue;
            }

            if (label41.Text == "0")
            {
                panel1.BackColor = Color.LightGray;
            }
            if (label42.Text == "0")
            {
                panel2.BackColor = Color.LightGray;
            }
            if (label43.Text == "0")
            {
                panel3.BackColor = Color.LightGray;
            }
            if (label44.Text == "0")
            {
                panel4.BackColor = Color.LightGray;
            }
            if (label45.Text == "0")
            {
                panel5.BackColor = Color.LightGray;
            }
            if (label46.Text == "0")
            {
                panel6.BackColor = Color.LightGray;
            }
            if (label47.Text == "0")
            {
                panel7.BackColor = Color.LightGray;
            }
            if (label48.Text == "0")
            {
                panel8.BackColor = Color.LightGray;
            }
            if (label49.Text == "0")
            {
                panel9.BackColor = Color.LightGray;
            }
            if (label50.Text == "0")
            {
                panel10.BackColor = Color.LightGray;
            }
            if (label51.Text == "0")
            {
                panel11.BackColor = Color.LightGray;
            }
            if (label52.Text == "0")
            {
                panel12.BackColor = Color.LightGray;
            }
            if (label53.Text == "0")
            {
                panel13.BackColor = Color.LightGray;
            }
            if (label54.Text == "0")
            {
                panel14.BackColor = Color.LightGray;
            }
            if (label55.Text == "0")
            {
                panel15.BackColor = Color.LightGray;
            }
            if (label56.Text == "0")
            {
                panel16.BackColor = Color.LightGray;
            }
            if (label57.Text == "0")
            {
                panel17.BackColor = Color.LightGray;
            }
            if (label58.Text == "0")
            {
                panel18.BackColor = Color.LightGray;
            }
            if (label59.Text == "0")
            {
                panel19.BackColor = Color.LightGray;
            }
            if (label60.Text == "0")
            {
                panel20.BackColor = Color.LightGray;
            }
            if (label61.Text == "0")
            {
                panel21.BackColor = Color.LightGray;
            }
            if (label62.Text == "0")
            {
                panel22.BackColor = Color.LightGray;
            }
            if (label63.Text == "0")
            {
                panel23.BackColor = Color.LightGray;
            } if (label64.Text == "0")
            {
                panel24.BackColor = Color.LightGray;
            }
            if (label65.Text == "0")
            {
                panel25.BackColor = Color.LightGray;
            }
            if (label66.Text == "0")
            {
                panel26.BackColor = Color.LightGray;
            }
            if (label67.Text == "0")
            {
                panel27.BackColor = Color.LightGray;
            }
            if (label68.Text == "0")
            {
                panel28.BackColor = Color.LightGray;
            }
            if (label69.Text == "0")
            {
                panel29.BackColor = Color.LightGray;
            }
            if (label70.Text == "0")
            {
                panel30.BackColor = Color.LightGray;
            } if (label71.Text == "0")
            {
                panel31.BackColor = Color.LightGray;
            }
            if (label72.Text == "0")
            {
                panel32.BackColor = Color.LightGray;
            }
            if (label73.Text == "0")
            {
                panel33.BackColor = Color.LightGray;
            }
            if (label74.Text == "0")
            {
                panel34.BackColor = Color.LightGray;
            }
            if (label75.Text == "0")
            {
                panel35.BackColor = Color.LightGray;
            }
            if (label76.Text == "0")
            {
                panel36.BackColor = Color.LightGray;
            }
            if (label77.Text == "0")
            {
                panel37.BackColor = Color.LightGray;
            }
            if (label78.Text == "0")
            {
                panel38.BackColor = Color.LightGray;
            }
            if (label79.Text == "0")
            {
                panel39.BackColor = Color.LightGray;
            }
            if (label80.Text == "0")
            {
                panel40.BackColor = Color.LightGray;
            }
        }


       /* void change_color(byte[] data1, byte[,] data2)
        {
            VG7 = ((data1[0] >> 7) & 0x01).ToString();
            VG8 = ((data1[0] >> 6) & 0x01).ToString();
            WHMP = ((data1[0] >> 5) & 0x01).ToString();
            WHMB = ((data1[0] >> 4) & 0x01).ToString();
            OPRA = ((data1[0] >> 3) & 0x01).ToString();
            MCOS = ((data1[0] >> 2) & 0x01).ToString();
            HBD = ((data1[0] >> 1) & 0x01).ToString();
            CFD = ((data1[0] >> 0) & 0x01).ToString();

            PUD = ((data1[1] >> 7) & 0x01).ToString();
            GPLVD = ((data1[1] >> 6) & 0x01).ToString();
            P5LVD = ((data1[1] >> 5) & 0x01).ToString();
            P15LVD = ((data1[1] >> 4) & 0x01).ToString();
            M15LVD = ((data1[1] >> 3) & 0x01).ToString();
            P24LVD = ((data1[1] >> 2) & 0x01).ToString();
            WDTD = ((data1[1] >> 1) & 0x01).ToString();
            THD = ((data1[1] >> 0) & 0x01).ToString();

            TROPRA = ((data1[2] >> 7) & 0x01).ToString();
            LOFD = ((data1[2] >> 6) & 0x01).ToString();

            PLVD = ((data1[2] >> 5) & 0x01).ToString();
            BCFD = ((data1[2] >> 4) & 0x01).ToString();
            BCOCD = ((data1[2] >> 3) & 0x01).ToString();
            MMOCD = ((data1[2] >> 2) & 0x01).ToString();
            P110LVD = ((data1[2] >> 1) & 0x01).ToString();

            OVD1 = ((data1[2] >> 0) & 0x01).ToString();
            OVD2 = ((data1[3] >> 7) & 0x01).ToString();
            FCLVD = ((data1[3] >> 6) & 0x01).ToString();
            ESLVD = ((data1[3] >> 5) & 0x01).ToString();
            LGD = ((data1[3] >> 4) & 0x01).ToString();
            PGD = ((data1[3] >> 3) & 0x01).ToString();
            WSD = ((data1[3] >> 2) & 0x01).ToString();
            BSD = ((data1[3] >> 1) & 0x01).ToString();

            OBTD = ((data1[3] >> 0) & 0x01).ToString();
            FCD = ((data1[4] >> 7) & 0x01).ToString();
            LOND = ((data1[4] >> 6) & 0x01).ToString();

            BCTHD = ((data1[4] >> 5) & 0x01).ToString();
            BRTHD = ((data1[4] >> 4) & 0x01).ToString();
            PGD1 = ((data1[4] >> 3) & 0x01).ToString();
            PGD2 = ((data1[4] >> 2) & 0x01).ToString();
            PGD3 = ((data1[4] >> 1) & 0x01).ToString();
            PGD4 = ((data1[4] >> 0) & 0x01).ToString();

            //BCW=
            //TIG=
            //THR =
            //TIG1=
            //TIG2=
            //THR1=
            //THR2=
            TIG1 = (((int)((data2[0, 0] >> 7) & 0x01) * (-0x8000)) + ((int)(data2[0, 0] & 0x7F) * 0x0100) + ((int)data2[0, 1])).ToString();
            TIG2 = (((int)((data2[1, 0] >> 7) & 0x01) * (-0x8000)) + ((int)(data2[1, 0] & 0x7F) * 0x0100) + ((int)data2[1, 1])).ToString();

            label41.Text = VG7;
            label42.Text = VG8;
            label43.Text = WHMP;
            label44.Text = WHMB;
            label45.Text = OPRA;
            label46.Text = MCOS;
            label47.Text = HBD;
            label48.Text = CFD;
            label49.Text = PUD;
            label50.Text = GPLVD;

            label51.Text = P5LVD;
            label52.Text = P15LVD;
            label53.Text = M15LVD;
            label54.Text = P24LVD;
            label55.Text = WDTD;
            label56.Text = THD;
            label57.Text = TROPRA;
            label58.Text = LOFD;

            label59.Text = PLVD;

            label60.Text = BCFD;
            label61.Text = BCOCD;
            label62.Text = MMOCD;
            label63.Text = P110LVD;
            label64.Text = OVD1;
            label65.Text = OVD2;
            label66.Text = FCLVD;
            label67.Text = ESLVD;
            label68.Text = LGD;
            label69.Text = PGD;

            label70.Text = WSD;
            label71.Text = BSD;
            label72.Text = OBTD;
            label73.Text = FCD;
            label74.Text = LOND;

            label75.Text = BCTHD;
            label76.Text = BRTHD;
            label77.Text = PGD1;
            label78.Text = PGD2;
            label79.Text = PGD3;
            label80.Text = PGD4;

            label91.Text = TIG1;
            label92.Text = TIG2;

            if (label41.Text == "1")
            {
                panel1.BackColor = Color.LightSkyBlue;
            }
            if (label42.Text == "1")
            {
                panel2.BackColor = Color.LightSkyBlue;
            }
            if (label43.Text == "1")
            {
                panel3.BackColor = Color.LightSkyBlue;
            }
            if (label44.Text == "1")
            {
                panel4.BackColor = Color.LightSkyBlue;
            }
            if (label45.Text == "1")
            {
                panel5.BackColor = Color.LightSkyBlue;
            }
            if (label46.Text == "1")
            {
                panel6.BackColor = Color.LightSkyBlue;
            }
            if (label47.Text == "1")
            {
                panel7.BackColor = Color.LightSkyBlue;
            }
            if (label48.Text == "1")
            {
                panel8.BackColor = Color.LightSkyBlue;
            }
            if (label49.Text == "1")
            {
                panel9.BackColor = Color.LightSkyBlue;
            }
            if (label50.Text == "1")
            {
                panel10.BackColor = Color.LightSkyBlue;
            }
            if (label51.Text == "1")
            {
                panel11.BackColor = Color.LightSkyBlue;
            }
            if (label52.Text == "1")
            {
                panel12.BackColor = Color.LightSkyBlue;
            }
            if (label53.Text == "1")
            {
                panel13.BackColor = Color.LightSkyBlue;
            }
            if (label54.Text == "1")
            {
                panel14.BackColor = Color.LightSkyBlue;
            }
            if (label55.Text == "1")
            {
                panel15.BackColor = Color.LightSkyBlue;
            }
            if (label56.Text == "1")
            {
                panel16.BackColor = Color.LightSkyBlue;
            }
            if (label57.Text == "1")
            {
                panel17.BackColor = Color.LightSkyBlue;
            }
            if (label58.Text == "1")
            {
                panel18.BackColor = Color.LightSkyBlue;
            }
            if (label59.Text == "1")
            {
                panel19.BackColor = Color.LightSkyBlue;
            }
            if (label60.Text == "1")
            {
                panel20.BackColor = Color.LightSkyBlue;
            }
            if (label61.Text == "1")
            {
                panel21.BackColor = Color.LightSkyBlue;
            }
            if (label62.Text == "1")
            {
                panel22.BackColor = Color.LightSkyBlue;
            }
            if (label63.Text == "1")
            {
                panel23.BackColor = Color.LightSkyBlue;
            } if (label64.Text == "1")
            {
                panel24.BackColor = Color.LightSkyBlue;
            }
            if (label65.Text == "1")
            {
                panel25.BackColor = Color.LightSkyBlue;
            }
            if (label66.Text == "1")
            {
                panel26.BackColor = Color.LightSkyBlue;
            }
            if (label67.Text == "1")
            {
                panel27.BackColor = Color.LightSkyBlue;
            }
            if (label68.Text == "1")
            {
                panel28.BackColor = Color.LightSkyBlue;
            }
            if (label69.Text == "1")
            {
                panel29.BackColor = Color.LightSkyBlue;
            }
            if (label70.Text == "1")
            {
                panel30.BackColor = Color.LightSkyBlue;
            } if (label71.Text == "1")
            {
                panel31.BackColor = Color.LightSkyBlue;
            }
            if (label72.Text == "1")
            {
                panel32.BackColor = Color.LightSkyBlue;
            }
            if (label73.Text == "1")
            {
                panel33.BackColor = Color.LightSkyBlue;
            }
            if (label74.Text == "1")
            {
                panel34.BackColor = Color.LightSkyBlue;
            }
            if (label75.Text == "1")
            {
                panel35.BackColor = Color.LightSkyBlue;
            }
            if (label76.Text == "1")
            {
                panel36.BackColor = Color.LightSkyBlue;
            }
            if (label77.Text == "1")
            {
                panel37.BackColor = Color.LightSkyBlue;
            }
            if (label78.Text == "1")
            {
                panel38.BackColor = Color.LightSkyBlue;
            }
            if (label79.Text == "1")
            {
                panel39.BackColor = Color.LightSkyBlue;
            }
            if (label80.Text == "1")
            {
                panel40.BackColor = Color.LightSkyBlue;
            }

            if (label41.Text == "0")
            {
                panel1.BackColor = Color.LightGray;
            }
            if (label42.Text == "0")
            {
                panel2.BackColor = Color.LightGray;
            }
            if (label43.Text == "0")
            {
                panel3.BackColor = Color.LightGray;
            }
            if (label44.Text == "0")
            {
                panel4.BackColor = Color.LightGray;
            }
            if (label45.Text == "0")
            {
                panel5.BackColor = Color.LightGray;
            }
            if (label46.Text == "0")
            {
                panel6.BackColor = Color.LightGray;
            }
            if (label47.Text == "0")
            {
                panel7.BackColor = Color.LightGray;
            }
            if (label48.Text == "0")
            {
                panel8.BackColor = Color.LightGray;
            }
            if (label49.Text == "0")
            {
                panel9.BackColor = Color.LightGray;
            }
            if (label50.Text == "0")
            {
                panel10.BackColor = Color.LightGray;
            }
            if (label51.Text == "0")
            {
                panel11.BackColor = Color.LightGray;
            }
            if (label52.Text == "0")
            {
                panel12.BackColor = Color.LightGray;
            }
            if (label53.Text == "0")
            {
                panel13.BackColor = Color.LightGray;
            }
            if (label54.Text == "0")
            {
                panel14.BackColor = Color.LightGray;
            }
            if (label55.Text == "0")
            {
                panel15.BackColor = Color.LightGray;
            }
            if (label56.Text == "0")
            {
                panel16.BackColor = Color.LightGray;
            }
            if (label57.Text == "0")
            {
                panel17.BackColor = Color.LightGray;
            }
            if (label58.Text == "0")
            {
                panel18.BackColor = Color.LightGray;
            }
            if (label59.Text == "0")
            {
                panel19.BackColor = Color.LightGray;
            }
            if (label60.Text == "0")
            {
                panel20.BackColor = Color.LightGray;
            }
            if (label61.Text == "0")
            {
                panel21.BackColor = Color.LightGray;
            }
            if (label62.Text == "0")
            {
                panel22.BackColor = Color.LightGray;
            }
            if (label63.Text == "0")
            {
                panel23.BackColor = Color.LightGray;
            } if (label64.Text == "0")
            {
                panel24.BackColor = Color.LightGray;
            }
            if (label65.Text == "0")
            {
                panel25.BackColor = Color.LightGray;
            }
            if (label66.Text == "0")
            {
                panel26.BackColor = Color.LightGray;
            }
            if (label67.Text == "0")
            {
                panel27.BackColor = Color.LightGray;
            }
            if (label68.Text == "0")
            {
                panel28.BackColor = Color.LightGray;
            }
            if (label69.Text == "0")
            {
                panel29.BackColor = Color.LightGray;
            }
            if (label70.Text == "0")
            {
                panel30.BackColor = Color.LightGray;
            } if (label71.Text == "0")
            {
                panel31.BackColor = Color.LightGray;
            }
            if (label72.Text == "0")
            {
                panel32.BackColor = Color.LightGray;
            }
            if (label73.Text == "0")
            {
                panel33.BackColor = Color.LightGray;
            }
            if (label74.Text == "0")
            {
                panel34.BackColor = Color.LightGray;
            }
            if (label75.Text == "0")
            {
                panel35.BackColor = Color.LightGray;
            }
            if (label76.Text == "0")
            {
                panel36.BackColor = Color.LightGray;
            }
            if (label77.Text == "0")
            {
                panel37.BackColor = Color.LightGray;
            }
            if (label78.Text == "0")
            {
                panel38.BackColor = Color.LightGray;
            }
            if (label79.Text == "0")
            {
                panel39.BackColor = Color.LightGray;
            }
            if (label80.Text == "0")
            {
                panel40.BackColor = Color.LightGray;
            }
        }*/

        public void ListBoxAutoCroll(ListBox lbox)
        {
            //lbox.Items.Add(" ");
            lbox.TopIndex = lbox.Items.Count - (int)(lbox.Height / lbox.ItemHeight);
            //lbox.TopIndex = lbox.Items.Count - 1;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                columnRefreshFlag = 1;

                listBox1.SetSelected(0, false);
            }
            catch
            {
                MessageBox.Show("无连接");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                listBox2.Items.Clear();
                string type = comboBox2.Text.Substring(3, 1),
                       num = comboBox2.Text.Substring(8, 1),
                       year = textBox3.Text,
                       month = textBox4.Text,
                       day = textBox5.Text;
                string a = path + "\\" + "类型 " + type + " 地址 " + num + "." + year + "年" + month + "月" + day + "日" + ".txt";
                AddTextToList(a, listBox2);
            }
            catch
            {
                MessageBox.Show("无数据");
            }
        }

        private void AddTextToList(string path, ListBox lst)
        {
            StreamReader file = new StreamReader(path, Encoding.Default);
            string s = "";
            while (s != null)
            {
                s = file.ReadLine();
                if (!string.IsNullOrEmpty(s))
                    lst.Items.Add(s);
            }
            file.Close();
        }

        private void clickHistoryList(string la)
        {
            aa = la;
            string a1, a2, a3, a4, a5;
            convertHextoBin(aa.Substring(0, 2));
            a1 = c1;
            convertHextoBin(aa.Substring(3, 2));
            a2 = c1;
            convertHextoBin(aa.Substring(6, 2));
            a3 = c1;
            convertHextoBin(aa.Substring(9, 2));
            a4 = c1;
            convertHextoBin(aa.Substring(12, 2));
            a5 = c1;

            byte m00 = Convert.ToByte(aa.Substring(15, 2), 16);
            byte m01 = Convert.ToByte(aa.Substring(18, 2), 16);

            byte m10 = Convert.ToByte(aa.Substring(21, 2), 16);
            byte m11 = Convert.ToByte(aa.Substring(24, 2), 16);

            TIG1 = (((int)((m00 >> 7) & 0x01) * (-0x8000)) + ((int)(m00 & 0x7F) * 0x0100) + ((int)m01)).ToString();
            TIG2 = (((int)((m10 >> 7) & 0x01) * (-0x8000)) + ((int)(m10 & 0x7F) * 0x0100) + ((int)m11)).ToString();

            VG7l = a1.Substring(0, 1);
            VG8l = a1.Substring(1, 1);
            WHMPl = a1.Substring(2, 1);
            WHMBl = a1.Substring(3, 1);
            OPRAl = a1.Substring(4, 1);
            MCOSl = a1.Substring(5, 1);
            HBDl = a1.Substring(6, 1);
            CFDl = a1.Substring(7, 1);
            PUDl = a2.Substring(0, 1);
            GPLVDl = a2.Substring(1, 1);

            P5LVDl = a2.Substring(2, 1);
            P15LVDl = a2.Substring(3, 1);
            M15LVDl = a2.Substring(4, 1);
            P24LVDl = a2.Substring(5, 1);
            WDTDl = a2.Substring(6, 1);
            THDl = a2.Substring(7, 1);
            TROPRAl = a3.Substring(0, 1);
            LOFDl = a3.Substring(1, 1);

            PLVDl = a3.Substring(2, 1);

            BCFDl = a3.Substring(3, 1);
            BCOCDl = a3.Substring(4, 1);
            MMOCDl = a3.Substring(5, 1);
            P110LVDl = a3.Substring(6, 1);
            OVD1l = a3.Substring(7, 1);
            OVD2l = a4.Substring(0, 1);
            FCLVDl = a4.Substring(1, 1);
            ESLVDl = a4.Substring(2, 1);
            LGDl = a4.Substring(3, 1);
            PGDl = a4.Substring(4, 1);

            WSDl = a4.Substring(5, 1);
            BSDl = a4.Substring(6, 1);
            OBTDl = a4.Substring(7, 1);
            FCDl = a5.Substring(0, 1);
            LONDl = a5.Substring(1, 1);

            BCTHDl = a5.Substring(2, 1);
            BRTHDl = a5.Substring(3, 1);
            PGD1l = a5.Substring(4, 1);
            PGD2l = a5.Substring(5, 1);
            PGD3l = a5.Substring(6, 1);
            PGD4l = a5.Substring(7, 1);

            label201.Text = VG7l;
            label202.Text = VG8l;
            label203.Text = WHMPl;
            label204.Text = WHMBl;
            label205.Text = OPRAl;
            label206.Text = MCOSl;
            label207.Text = HBDl;
            label208.Text = CFDl;
            label209.Text = PUDl;
            label210.Text = GPLVDl;

            label211.Text = P5LVDl;
            label212.Text = P15LVDl;
            label213.Text = M15LVDl;
            label214.Text = P24LVDl;
            label215.Text = WDTDl;
            label216.Text = THDl;
            label217.Text = TROPRAl;
            label218.Text = LOFDl;

            label219.Text = PLVDl;

            label220.Text = BCFDl;
            label221.Text = BCOCDl;
            label222.Text = MMOCDl;
            label223.Text = P110LVDl;
            label224.Text = OVD1l;
            label225.Text = OVD2l;
            label226.Text = FCLVDl;
            label227.Text = ESLVDl;
            label228.Text = LGDl;
            label229.Text = PGDl;

            label230.Text = WSDl;
            label231.Text = BSDl;
            label232.Text = OBTDl;
            label233.Text = FCDl;
            label234.Text = LONDl;

            label235.Text = BCTHDl;
            label236.Text = BRTHDl;
            label237.Text = PGD1l;
            label238.Text = PGD2l;
            label239.Text = PGD1l;
            label240.Text = PGD2l;

            label88.Text = BCW;
            label89.Text = TIG;
            label90.Text = THR;
            label187.Text = TIG1;
            label188.Text = TIG2;
            label93.Text = THR1;
            label94.Text = THR2;

            if (label201.Text == "1")
            {
                panel101.BackColor = Color.LightSkyBlue;
            }
            if (label202.Text == "1")
            {
                panel102.BackColor = Color.LightSkyBlue;
            }
            if (label203.Text == "1")
            {
                panel103.BackColor = Color.LightSkyBlue;
            }
            if (label204.Text == "1")
            {
                panel104.BackColor = Color.LightSkyBlue;
            }
            if (label205.Text == "1")
            {
                panel105.BackColor = Color.LightSkyBlue;
            }
            if (label206.Text == "1")
            {
                panel106.BackColor = Color.LightSkyBlue;
            }
            if (label207.Text == "1")
            {
                panel107.BackColor = Color.LightSkyBlue;
            }
            if (label208.Text == "1")
            {
                panel108.BackColor = Color.LightSkyBlue;
            }
            if (label209.Text == "1")
            {
                panel109.BackColor = Color.LightSkyBlue;
            }
            if (label210.Text == "1")
            {
                panel110.BackColor = Color.LightSkyBlue;
            }
            if (label211.Text == "1")
            {
                panel111.BackColor = Color.LightSkyBlue;
            }
            if (label212.Text == "1")
            {
                panel112.BackColor = Color.LightSkyBlue;
            }
            if (label213.Text == "1")
            {
                panel113.BackColor = Color.LightSkyBlue;
            }
            if (label214.Text == "1")
            {
                panel114.BackColor = Color.LightSkyBlue;
            }
            if (label215.Text == "1")
            {
                panel115.BackColor = Color.LightSkyBlue;
            }
            if (label216.Text == "1")
            {
                panel116.BackColor = Color.LightSkyBlue;
            }
            if (label217.Text == "1")
            {
                panel117.BackColor = Color.LightSkyBlue;
            }
            if (label218.Text == "1")
            {
                panel118.BackColor = Color.LightSkyBlue;
            }
            if (label219.Text == "1")
            {
                panel119.BackColor = Color.LightSkyBlue;
            }
            if (label220.Text == "1")
            {
                panel120.BackColor = Color.LightSkyBlue;
            }
            if (label221.Text == "1")
            {
                panel121.BackColor = Color.LightSkyBlue;
            }
            if (label222.Text == "1")
            {
                panel122.BackColor = Color.LightSkyBlue;
            }
            if (label223.Text == "1")
            {
                panel123.BackColor = Color.LightSkyBlue;
            } if (label224.Text == "1")
            {
                panel124.BackColor = Color.LightSkyBlue;
            }
            if (label225.Text == "1")
            {
                panel125.BackColor = Color.LightSkyBlue;
            }
            if (label226.Text == "1")
            {
                panel126.BackColor = Color.LightSkyBlue;
            }
            if (label227.Text == "1")
            {
                panel127.BackColor = Color.LightSkyBlue;
            }
            if (label228.Text == "1")
            {
                panel128.BackColor = Color.LightSkyBlue;
            }
            if (label229.Text == "1")
            {
                panel129.BackColor = Color.LightSkyBlue;
            }
            if (label230.Text == "1")
            {
                panel130.BackColor = Color.LightSkyBlue;
            } if (label231.Text == "1")
            {
                panel131.BackColor = Color.LightSkyBlue;
            }
            if (label232.Text == "1")
            {
                panel132.BackColor = Color.LightSkyBlue;
            }
            if (label233.Text == "1")
            {
                panel133.BackColor = Color.LightSkyBlue;
            }
            if (label234.Text == "1")
            {
                panel134.BackColor = Color.LightSkyBlue;
            }
            if (label235.Text == "1")
            {
                panel135.BackColor = Color.LightSkyBlue;
            }
            if (label236.Text == "1")
            {
                panel136.BackColor = Color.LightSkyBlue;
            }
            if (label237.Text == "1")
            {
                panel137.BackColor = Color.LightSkyBlue;
            }
            if (label238.Text == "1")
            {
                panel138.BackColor = Color.LightSkyBlue;
            }
            if (label239.Text == "1")
            {
                panel139.BackColor = Color.LightSkyBlue;
            }
            if (label240.Text == "1")
            {
                panel140.BackColor = Color.LightSkyBlue;
            }

            if (label201.Text == "0")
            {
                panel101.BackColor = Color.LightGray;
            }
            if (label202.Text == "0")
            {
                panel102.BackColor = Color.LightGray;
            }
            if (label203.Text == "0")
            {
                panel103.BackColor = Color.LightGray;
            }
            if (label204.Text == "0")
            {
                panel104.BackColor = Color.LightGray;
            }
            if (label205.Text == "0")
            {
                panel105.BackColor = Color.LightGray;
            }
            if (label206.Text == "0")
            {
                panel106.BackColor = Color.LightGray;
            }
            if (label207.Text == "0")
            {
                panel107.BackColor = Color.LightGray;
            }
            if (label208.Text == "0")
            {
                panel108.BackColor = Color.LightGray;
            }
            if (label209.Text == "0")
            {
                panel109.BackColor = Color.LightGray; ;
            }
            if (label210.Text == "0")
            {
                panel110.BackColor = Color.LightGray;
            }
            if (label211.Text == "0")
            {
                panel111.BackColor = Color.LightGray;
            }
            if (label212.Text == "0")
            {
                panel112.BackColor = Color.LightGray;
            }
            if (label213.Text == "0")
            {
                panel113.BackColor = Color.LightGray;
            }
            if (label214.Text == "0")
            {
                panel114.BackColor = Color.LightGray;
            }
            if (label215.Text == "0")
            {
                panel115.BackColor = Color.LightGray;
            }
            if (label216.Text == "0")
            {
                panel116.BackColor = Color.LightGray;
            }
            if (label217.Text == "0")
            {
                panel117.BackColor = Color.LightGray;
            }
            if (label218.Text == "0")
            {
                panel118.BackColor = Color.LightGray;
            }
            if (label219.Text == "0")
            {
                panel119.BackColor = Color.LightGray;
            }
            if (label220.Text == "0")
            {
                panel120.BackColor = Color.LightGray;
            }
            if (label221.Text == "0")
            {
                panel121.BackColor = Color.LightGray;
            }
            if (label222.Text == "0")
            {
                panel122.BackColor = Color.LightGray;
            }
            if (label223.Text == "0")
            {
                panel123.BackColor = Color.LightGray;
            } if (label224.Text == "0")
            {
                panel124.BackColor = Color.LightGray;
            }
            if (label225.Text == "0")
            {
                panel125.BackColor = Color.LightGray;
            }
            if (label226.Text == "0")
            {
                panel126.BackColor = Color.LightGray;
            }
            if (label227.Text == "0")
            {
                panel127.BackColor = Color.LightGray;
            }
            if (label228.Text == "0")
            {
                panel128.BackColor = Color.LightGray;
            }
            if (label229.Text == "0")
            {
                panel129.BackColor = Color.LightGray;
            }
            if (label230.Text == "0")
            {
                panel130.BackColor = Color.LightGray;
            } if (label231.Text == "0")
            {
                panel131.BackColor = Color.LightGray;
            }
            if (label232.Text == "0")
            {
                panel132.BackColor = Color.LightGray;
            }
            if (label233.Text == "0")
            {
                panel133.BackColor = Color.LightGray;
            }
            if (label234.Text == "0")
            {
                panel134.BackColor = Color.LightGray;
            }
            if (label235.Text == "0")
            {
                panel135.BackColor = Color.LightGray;
            }
            if (label236.Text == "0")
            {
                panel136.BackColor = Color.LightGray;
            }
            if (label237.Text == "0")
            {
                panel137.BackColor = Color.LightGray;
            }
            if (label238.Text == "0")
            {
                panel138.BackColor = Color.LightGray;
            }
            if (label239.Text == "0")
            {
                panel139.BackColor = Color.LightGray;
            }
            if (label240.Text == "0")
            {
                panel140.BackColor = Color.LightGray;
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string a = listBox2.SelectedItem.ToString();
            clickHistoryList(a);
        }

        private void panel41_Paint(object sender, PaintEventArgs e)
        {

        }

        [DllImport("user32")]

        public static extern int SetParent(int hWndChild, int hWndNewParent);

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            if (fbd.SelectedPath.Length > 0)
            {
                path = fbd.SelectedPath;
                StreamWriter writepath = new StreamWriter(defaultpath + "\\" + "pathfile.txt", false, Encoding.UTF8);
                writepath.Write(path);
                writepath.Flush();
                writepath.Close();
            }
        }
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


















