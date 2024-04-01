﻿using Octopus.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Layers.Entities
{
    internal class Departamento
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public Estado estado { get; set; }
    }
}
