﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoodmoParser.Parsers
{
    public interface IParser
    {
        public Task Run();
    }
}
