using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoodmoParser.Entities
{
    public class OEMReplacementParts : BaseEntity
    {
        public string PartsBrand { get; set; }

        public string Title { get; set; }

        public string ShortNumber { get; set; }

        public double? Price { get; set; }

        public Guid ItemId { get; set; }
    }
}
