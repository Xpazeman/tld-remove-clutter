using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoveClutter
{
    internal class BreakDownYield
    {
        public String item;
        public int num;
    }

    class BreakDownDefinition
    {
        public String filter = "";
        public String sound = "";
        public float minutesToHarvest = 1f;

        public bool requireTool = false;
        public String[] tools;
        public BreakDownYield[] yield;

    }
}
