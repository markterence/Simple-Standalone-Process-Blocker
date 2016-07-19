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
    public partial class frmPassword : Form
    {

        public frmPassword()
        {
            InitializeComponent();
            //this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            //this.DialogResult = System.Windows.Forms.DialogResult.None;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            checkPassword();
        }
        void checkPassword()
        {
            
            try
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(GlobalVars.settings);
                String data = reader.ReadLine();
                String[] d =data.Split('=');


                if (textBox1.Text.Trim() == "")
                {
                    label2.Text = "Error";
                }
                else
                {
                    if (d[1] == textBox1.Text)
                    {
                        this.DialogResult = DialogResult.OK;
                        label2.Text = "";
                       
                    }
                    else
                    {

                        label2.Text = "Wrong Password.";
                        
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private void frmPassword_Load(object sender, EventArgs e)
        {

        }

        private void frmPassword_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            this.Parent = null;    
            //e.Cancel = true;
        }
    }
}
