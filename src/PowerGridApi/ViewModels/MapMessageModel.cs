﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace PowerGridEngine
{
    public class MapMessageModel: MessageModel
    {
        public Map Map { get; set;}
    }
}