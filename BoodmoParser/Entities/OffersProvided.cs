using Entities;

namespace BoodmoParser.Entities
{
    public class OffersProvided : BaseEntity
    {
        public string SoldBy { get; set; }

        public double? Price { get; set; }

        public double DeliveryCharge { get; set; }

        public int ItemId { get; set; }

        public virtual Item Item { get; set; }
    }
}
