using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcessBlocker
{
    public class Process
    {
        public String Name { get; set; }
        public Process(String processName)
        {
            this.Name = processName;
        }

    }
}
