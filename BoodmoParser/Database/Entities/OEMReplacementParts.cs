using Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoodmoParser.Database.Entities
{
    public class OEMReplacementParts : BaseEntity
    {
        public string PartsBrand { get; set; }

        public string Title { get; set; }

        public string SoldBy { get; set; }

        public double? Price { get; set; }
    }
}
