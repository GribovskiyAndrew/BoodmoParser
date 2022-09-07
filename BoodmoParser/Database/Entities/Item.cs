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
    public class Item : BaseEntity
    {
        public virtual ICollection<AftermarketReplacementParts> Aftermarket { get; set; }
        public virtual ICollection<OEMReplacementParts> OEM { get; set; }
        public virtual ICollection<OffersProvided> Offers { get; set; }

        public string PartsBrand { get; set; }

        public string Title { get; set; }

        public string SoldBy { get; set; }

        public double Price { get; set; }

        public string PartNumber { get; set; }

        public string Origin { get; set; }

        public string Class { get; set; }

        public string Description { get; set; }

        public string? StartDate { get; set; }

        public string? EndDate { get; set; }
    }
}