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
    public partial class frmSetPassword : Form
    {
        

        public frmSetPassword()
        {
            InitializeComponent();
        }

        private void frmSetPassword_Load(object sender, EventArgs e)
        {
            if (!System.IO.File.Exists(GlobalVars.settings))
            {
                lblOldPass.Visible = false;
                txtOldPass.Visible = false;
            }
            else
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(GlobalVars.settings);
                String data = reader.ReadLine();
                String[] d = data.Split('=');
                if (d[1].Trim() == "")
                {
                    lblOldPass.Visible = false;
                    txtOldPass.Visible = false;
                }
                else
                {
                    label2.Text = "New " + label2.Text.Substring(0);
                    lblOldPass.Visible = true;
                    txtOldPass.Visible = true;

                }
                reader.Close();
            }
        }
        bool isValid()
        {

                if (txtOldPass.Visible == true)
                {
                    
                    System.IO.StreamReader reader = new System.IO.StreamReader(GlobalVars.settings);
                    String data = reader.ReadLine();
                    String[] d = data.Split('=');
                    reader.Close();
                    if (d[1] != txtOldPass.Text)
                    {
                        label1.Text = "Wrong Password.";
                        return false;

                    }
                }
                if (txtPass.Text.Trim() == "")
                {
                    if (txtOldPass.Visible == true)
                    {
                        label1.Text = "Please enter new password.";
                    }
                    else
                    {
                        label1.Text = "Invalid Password.";
                    }
                    return false;
                }
                if (txtConfirm.Text.Trim() == "")
                {
                    label1.Text = "Password didnt match";
                    return false;
                }
                if (txtPass.Text != txtConfirm.Text)
                {
                    label1.Text = "Password didnt match.";
                    return false;
                }
                else
                {
                    label1.Text = "OK";
                    return true;
                }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (isValid() == true)
            {
                SavePassword();
                MessageBox.Show("Save");
                this.Dispose();
            }

        }
        private void SavePassword()
        {
            try
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(GlobalVars.settings);
                writer.WriteLine("Password="+txtPass.Text);
                writer.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
