using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Management;
namespace ProcessBlocker
{
    public partial class Form1 : Form
    {
       /* NOTE:
        * System.Diagnostics.Process
        * and
        * ProcessBlocker.Process
        * are different classes so please use a Namespace.
        */
        
        Hotkey hk_Hide = new Hotkey();
        Hotkey hk_Show = new Hotkey();
        #region Forms
        public static Form1 test = new Form1();

        frmAdd frAdd;
        private frmPassword myform;
        frmPassword frmPass;
        #endregion
        #region Variable
        public List<ProcessBlocker.Process> blocked_processes;
        public List<ProcessBlocker.Process> excluded_processes = new List<ProcessBlocker.Process>();


        bool found = false, sysFound = false;
        #endregion

        public Form1()
        {
            blocked_processes = new List<ProcessBlocker.Process>();     
            //Add Excluded Processes
            for (int i = 0; i < GlobalVars.excluded.Length; i++)
            {
                excluded_processes.Add(new ProcessBlocker.Process(GlobalVars.excluded[i]));
            }
            
            InitializeComponent();

            //
            frmPass = new frmPassword();
        }
        
        #region ホットキー [Hotkey]
        void RegisterHotkey(Keys hide, Keys show)
        {
            hk_Hide.KeyCode = hide;
            hk_Hide.Alt = true;
            hk_Hide.Pressed +=new HandledEventHandler(hk_Hide_Pressed);
            if (hk_Hide.GetCanRegister(this))
            {
                hk_Hide.Register(this);
            }

            hk_Show.KeyCode = show;
            hk_Show.Alt = true;
            hk_Show.Pressed +=new HandledEventHandler(hk_Show_Pressed);
            if(hk_Show.GetCanRegister(this))
            {
                hk_Show.Register(this);
            }
        }

        void hk_Show_Pressed(object sender, HandledEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Show Pressed");
            //this.Visible = true;

            if (Properties.Settings.Default.enablePass == true && this.Visible == false)
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(GlobalVars.settings);
                String data = reader.ReadLine();
                String[] d = data.Split('=');
                reader.Close();
                if (d[1].Trim() == "")
                {
                    //dont show Enter Password
                    System.Diagnostics.Debug.WriteLine("do not show");
                }
                else
                {
                    
                    DialogResult dr = new DialogResult();
                    if (myform == null)
                    {
                        myform = new frmPassword();
                        myform.FormClosing +=new FormClosingEventHandler(myform_FormClosing);
                        dr = myform.ShowDialog(this);
                        if (dr == DialogResult.OK)
                        {

                            this.Visible = true;
                        }
                    }
                }
            }
            else
            {
                this.Show();
            }

        }
        void myform_FormClosing(object sender, FormClosingEventArgs e)
        {
            myform = null;
        }
        void hk_Hide_Pressed(object sender, HandledEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Hide Pressed");
            this.Visible = false;
        }
        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            RegisterHotkey(Keys.F10, Keys.F11);
            createProcess_Block_List_File();
            
            //Dump 'blocklist file' in the AppDataFolder.
            //Other system might require administrator privelages for
            //(reading and writing to a file).
            if (!System.IO.Directory.Exists(GlobalVars.AppdataFolder)) 
            {
                System.IO.Directory.CreateDirectory(GlobalVars.AppdataFolder); 
            }
            if (!System.IO.File.Exists(GlobalVars.file_output))
            {
                System.IO.File.Create(GlobalVars.file_output).Close();
            }
            this.enablePasswordtoolStripMenuItem.Checked = Properties.Settings.Default.enablePass;
            ToolSetting();
            /* Register Show and Hide Hotkey
             * Show = Alt + F11
             * Hide = Alt + F10s
             * See #region, ホットキー [Hotkey]
             */
            
        }
        private void LoadSetting()
        {
            if (System.IO.File.Exists(GlobalVars.settings))
            {
                String[] A = System.IO.File.ReadAllLines(GlobalVars.settings);
               
                foreach(var item in A)
                {

                }
            }
        }
        private void createProcess_Block_List_File()
        {
            try
            {
                // do the 'block list file' exist ?
                //String datas = "";
                if (System.IO.File.Exists(GlobalVars.file_output))
                {
                    //Yes, Read the Block List file.
                    System.IO.StreamReader reader = new System.IO.StreamReader(GlobalVars.file_output);
                    while (reader.Peek() != -1)
                    {
                        //Add the contents of 'BlockListFile' to a List<T>
                        blocked_processes.Add(new ProcessBlocker.Process(reader.ReadLine()));
                    }
                    reader.Close();

                    foreach (var blockedProcess in blocked_processes)
                    {
                        //Add each processes in the ListView
                        addProcessesToList(blockedProcess.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Add();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            removeProcess();
        }


        /// <summary>
        /// Starts or Stops the blocking of processes.
        /// </summary>
        /// <param name="status">Start blocking if status is True, Stop if status is false</param>
        void Enable_Process_Blocking(bool status)
        {
            switch (status)
            {
                case true:
                    try
                    {
                        foreach (var proc in System.Diagnostics.Process.GetProcesses())
                        {
                            foreach (var blocked in blocked_processes)
                            {
                                if (proc.ProcessName.Equals(blocked.Name))
                                {
                                    proc.Kill();
                                    proc.WaitForExit();
                                    System.Diagnostics.Debug.WriteLine("'" + proc.ProcessName + "' has been slain");
                                    //MessageBox.Show("is not a valid win32 application","error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;
                case false:
                    timer1.Stop();
                    break;
            }
        }

        void Add()
        {
            found = false;
            sysFound = false;

            frAdd = new frmAdd();
            if (frAdd.ShowDialog() == DialogResult.OK)
            {
                foreach (var p in frAdd.newProcess)
                {
                    foreach (var b in blocked_processes)
                    {
                        if (p.Name == b.Name)
                        {
                            System.Diagnostics.Debug.WriteLine("ERROR: " + p.Name + " == " + b.Name);
                            found = true;
                        }
                    }

                    foreach (var e in excluded_processes)
                    {
                        if (p.Name == e.Name)
                        {
                            sysFound = true;
                        }
                    }
                }

                if (sysFound == true)
                {
                    MessageBox.Show("Unable to add a SYSTEM Process.");
                }
                if (found == false && sysFound == false)
                {
                    foreach (var p in frAdd.newProcess)
                    {
                        addProcessesToList(p.Name);
                    }
                }
                if(found == true)
                {
                    MessageBox.Show("The Process is already in the list.");
                }

                Save();
            }
        }

        public void Save()
        {
            blocked_processes.Clear();
            System.IO.StreamWriter writer = new System.IO.StreamWriter(GlobalVars.file_output);
            foreach (ListViewItem itm in listView1.Items)
            {
                try
                {
                    //System.Diagnostics.Debug.WriteLine(itm.Text);
                    writer.WriteLine(itm.Text);
                    
                    blocked_processes.Add(new ProcessBlocker.Process(itm.Text));
                    
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            writer.Close();
            System.Diagnostics.Debug.WriteLine("Save End");
        }

        /// <summary>
        /// Removes a process from the list and update the 'block list file'
        /// </summary>
        public void removeProcess()
        {
            if (listView1.Items.Count <= 0)
            {
                MessageBox.Show("There are no processes in the list.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                foreach (ListViewItem itm in listView1.SelectedItems)
                {
                    
                    listView1.Items.Remove(itm);
                    System.Diagnostics.Debug.WriteLine("remove: " + itm.Text);
                    Save();
                    //blocked_processes.Remove(new Process(itm.Text));
                }
                

            }

        }
        /// <summary>
        /// Add an item in listView1
        /// </summary>
        /// <param name="proc">process name or the item you want to add in the listView1</param>
        public void addProcessesToList(String proc)
        {
            this.listView1.Items.Add(proc);
        }

        void ToolSetting()
        {
            
            if (enablePasswordtoolStripMenuItem.Checked == true)
            {
                setPasswordToolStripMenuItem.Enabled = true;
                Properties.Settings.Default.enablePass = true;

            }
            if (enablePasswordtoolStripMenuItem.Checked == false)
            {
                setPasswordToolStripMenuItem.Enabled = false;
                Properties.Settings.Default.enablePass = false;
            }
            Properties.Settings.Default.Save();
        }

        //Debug =>
        bool isOn = true;
        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count <= 0)
            {
                MessageBox.Show("Processes list is empty","",MessageBoxButtons.OK,MessageBoxIcon.Information);
                timer1.Stop();
            }
            else
            {
                if (isOn == true)
                {
                    timer1.Start();
                    button1.Text = "stop";
                    isOn = false;
                }
                else if (isOn == false)
                {
                    timer1.Stop();
                    button1.Text = "start";
                    isOn = true;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //foreach (var items in blocked_processes)
            //{
            //    Console.WriteLine(items.Name);
            //}

            foreach (var p in System.Diagnostics.Process.GetProcesses())
            {
                if(p.ProcessName.Equals("notepad"))
                {
                    Console.WriteLine("");
                }
            }
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            Enable_Process_Blocking(true);
            Random rng = new Random();
            label1.Text = rng.Next(0, 100).ToString();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
            hk_Hide.Unregister();
            hk_Show.Unregister();
        }

        private void enablePasswordtoolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            ToolSetting();
        }

        private void setPasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSetPassword frmSet = new frmSetPassword();
            frmSet.ShowDialog();
        }
    }
}
