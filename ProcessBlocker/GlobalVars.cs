using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

    public class GlobalVars
    {

        public static ProcessBlocker.frmPassword enterPassword = new ProcessBlocker.frmPassword();
        public static ProcessBlocker.Form1 frmOne;

        public static String AppdataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\MTT\\ProcessBlocker\\";
        
        public static string file_output = AppdataFolder + "block_list.json";

        public static string settings = AppdataFolder + "Settings.mtt";

        public static string[] excluded = {"explorer","System","wininit",
                                          "winlogon","spoolsv","dwm",
                                          "mmc","regedit","msconfig",
                                          "dxdiag","taskmgr","cmd","osk"};

    }
