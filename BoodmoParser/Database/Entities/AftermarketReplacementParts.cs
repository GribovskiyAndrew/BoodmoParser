using Database.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoodmoParser.Database.Entities
{
    public class AftermarketReplacementParts : BaseEntity
    {
        public string PartsBrand { get; set; }

        public string Title { get; set; }

        public string SoldBy { get; set; }

        public double? Price { get; set; }

        public string PartNumber { get; set; }

        public int? Discount { get; set; }

        public double? OriginalPrice { get; set; }

        public string? StartDate { get; set; }

        public string? EndDate { get; set; }
    }
}
