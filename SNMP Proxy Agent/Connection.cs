using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SNMP_Proxy_Agent
{
    public partial class Connection : Form
    {
        Thread ifConnectedThread;
        Form1 form1;
        public Connection(Form1 form1)
        {
            this.form1 = form1;
            InitializeComponent();
            label2.Text = Program.address + ":" + Program.port;
            ifConnectedThread = new Thread(() => ifConnected());
            ifConnectedThread.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
            Environment.Exit(0);
        }

        void ifConnected()
        {
            Thread.Sleep(100);
            while (true)
            {
                if (Program.connected == false)
                    Invoke((MethodInvoker)(() => this.label3.Text = "Waiting for connection...")); 
                else
                    Invoke((MethodInvoker)(() => this.label3.Text = "Connected! Listening for SNMP calls."));
                Thread.Sleep(100);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ifConnectedThread.Abort();
            Program.stopServerThread();
            this.Hide();
            new Form1().Show();
        }
    }
}
