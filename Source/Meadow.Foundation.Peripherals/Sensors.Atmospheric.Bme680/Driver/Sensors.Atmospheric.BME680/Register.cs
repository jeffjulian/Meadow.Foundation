﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensors.Atmospheric.Bme680
{
    public struct Register
    {
        public Register(byte address, byte length)
        {
            Address = address;
            Length = length;
        }

        public byte Address {get;}
        public byte Length {get;}
    }
}
