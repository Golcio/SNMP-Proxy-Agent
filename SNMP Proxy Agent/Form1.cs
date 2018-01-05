using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SNMP_Proxy_Agent
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox1.Text = Program.GetLocalIPAddress();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Program.address = textBox1.Text;
            try
            {
                Program.port = Int32.Parse(textBox2.Text);
                if (Program.port < 0 || Program.port > 65535)
                {
                    MessageBox.Show("Port has to be a number from 0 to 65535");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Port has to be a number");
                return;
            }

            try
            {
                Program.initializeSocket();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't bind this port");
                return;
            }

            this.Hide();
            new Connection().Show();
        }
    }
}
