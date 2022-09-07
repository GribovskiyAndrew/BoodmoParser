using Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoodmoParser.Database.Entities
{
    public class OffersProvided : BaseEntity
    {
        public string SoldBy { get; set; }

        public double? Price { get; set; }

        public double DeliveryCharge { get; set; }
    }
}
