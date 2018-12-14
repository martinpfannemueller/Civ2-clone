﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class NuclearPwr : BaseTech
    {
        public NuclearPwr() : base(3, 0, TechType.NuclearFiss, TechType.Electronics, 3, 3)
        {
            Type = TechType.NuclearPwr;
            Name = "Nuclear Power";
        }
    }
}