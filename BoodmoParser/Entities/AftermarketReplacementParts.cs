using Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoodmoParser.Entities
{
    public class AftermarketReplacementParts : BaseEntity
    {
        public string PartsBrand { get; set; }

        public string Title { get; set; }

        public double? Price { get; set; }

        public string ShortNumber { get; set; }

        public int? Discount { get; set; }

        public double? OriginalPrice { get; set; }

        public Guid ItemId { get; set; }
    }
}
