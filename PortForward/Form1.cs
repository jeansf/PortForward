using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PortForward
{
    public partial class Form1 : Form
    {
        private List<PortForward> lista = new List<PortForward> { };

        private Regex netshRegex = new Regex(@"^([\*\d\.]+)(?: +)([\d]+)(?: +)([\*\d\.]+)(?: +)([\d]+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private Regex ipRegex = new Regex(@"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private Regex portRegex = new Regex(@"^([1-9][0-9]{0,3}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])(-([1-9][0-9]{0,3}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5]))?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public Form1()
        {
            InitializeComponent();
            this.loadData();
        }

        public void loadData()
        {
            ProcessStartInfo procStartInfo = new ProcessStartInfo("netsh", "interface portproxy show all");
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;

            Process proc = Process.Start(procStartInfo);

            listBox1.Items.Clear();
            while (!proc.StandardOutput.EndOfStream)
            {
                MatchCollection matches = this.netshRegex.Matches(proc.StandardOutput.ReadLine());

                if (matches.Count > 0)
                {
                    Match match = matches[0];
                    string text = String.Format("IP: {0}:{1} => {2}:{3}",
                            match.Groups[1].Value,
                            match.Groups[2].Value,
                            match.Groups[3].Value,
                            match.Groups[4].Value
                        );
                    this.addPortForward(new PortForward(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, match.Groups[4].Value));
                }


            }

        }

        private void addPortForward(PortForward pf)
        {
            this.lista.Add(pf);
            this.listBox1.Items.Add(pf.__toString());
        }

        private void removePortForward(int index)
        {
            this.lista.RemoveAt(index);
            this.listBox1.Items.RemoveAt(index);
        }

        public void reset()
        {
            ProcessStartInfo procStartInfo = new ProcessStartInfo("netsh", "interface portproxy reset");
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;

            Process proc = Process.Start(procStartInfo);
            System.Threading.Thread.Sleep(100);
        }

        public void apply()
        {
            this.reset();
            foreach (PortForward pf in this.lista)
            {
                string forward = String.Format("listenport={1} listenaddress={0} connectport={3} connectaddress={2}",
                        pf.FromIP,
                        pf.FromPort,
                        pf.ToIP,
                        pf.ToPort
                    );
                Console.WriteLine(String.Format("interface portproxy add v4tov4 {0}", forward));
                ProcessStartInfo procStartInfo = new ProcessStartInfo("netsh", String.Format("interface portproxy add v4tov4 {0}", forward));
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = true;

                Process proc = Process.Start(procStartInfo);
                Console.WriteLine(proc.StandardOutput.ReadLine());
                System.Threading.Thread.Sleep(100);
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {

            this.apply();
            this.loadData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<string> err = new List<string> { };

            if (!this.ipRegex.IsMatch(this.textBox1.Text) && this.textBox1.Text != "*" && this.textBox1.Text != "")
            {
                err.Add("From IP:\t\tInvalid");
            }

            if (!this.portRegex.IsMatch(this.textBox2.Text))
            {
                err.Add("From Port:\tInvalid");
            }

            if (!this.ipRegex.IsMatch(this.textBox3.Text) && this.textBox3.Text != "*" && this.textBox3.Text != "")
            {
                err.Add("To IP:\tInvalid");
            }

            if (!this.portRegex.IsMatch(this.textBox4.Text))
            {
                err.Add("To Port:\t\tInvalid");
            }

            if (err.Count > 0)
            {
                MessageBox.Show(String.Join("\n", err.ToArray()), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.addPortForward(new PortForward(this.textBox1.Text, this.textBox2.Text, this.textBox3.Text, this.textBox4.Text));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.loadData();
        }

        private void button4_Click(object sender, EventArgs e)
        {

            this.listBox1.Enabled = false;

            int[] selectedIndices = listBox1.SelectedIndices.Cast<int>().ToArray();

            Array.Reverse(selectedIndices);

            foreach(int i in selectedIndices)
            {
                this.removePortForward(i);
            }
          
            this.listBox1.Enabled = true;

        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                this.button4_Click(sender, e);
            }
        }
    }
}
