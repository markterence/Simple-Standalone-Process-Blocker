using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProcessBlocker
{
    public partial class frmAdd : Form
    {
        public List<ProcessBlocker.Process> newProcess = new List<ProcessBlocker.Process>();
        
        public frmAdd()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                if (textBox1.Text.Trim() == "")
                {
                    MessageBox.Show("Unable to add empty string.","Error",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                }
                else
                {
                    Form1 frm1 = new Form1();
                    //Add processes to the 'Block list File'
                    //System.IO.StreamWriter writer = new System.IO.StreamWriter(GlobalVars.file_output,true);
                    String data = "";
                    StringBuilder sb = new StringBuilder();
                    foreach (string line in textBox1.Lines)
                    {
                        //removes the newline;
                        line.Replace(Environment.NewLine, "");
                        sb.Append(line);
                    }
                    data = sb.ToString();
                    string[] d = data.Split(',');
                    for (int i = 0; i < d.Length; i++)
                    {
                        //writer.WriteLine(d[i]); // + writer.NewLine);
                        DialogResult = DialogResult.OK;
                        newProcess.Add(new ProcessBlocker.Process(d[i]));
                    }
                    //writer.Close();
                    this.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("frmAdd : " + ex.Message);
            }
        }

        private void frmAdd_Load(object sender, EventArgs e)
        {

        }
    }
}
