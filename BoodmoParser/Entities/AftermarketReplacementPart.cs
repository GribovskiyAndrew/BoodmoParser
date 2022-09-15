using Entities;

namespace BoodmoParser.Entities
{
    public class AftermarketReplacementPart : BaseEntity
    {
        public string PartsBrand { get; set; }

        public string Title { get; set; }

        public double? Price { get; set; }

        public string PartNumber { get; set; }

        public int? Discount { get; set; }

        public double? OriginalPrice { get; set; }

        public int ItemId { get; set; }

        public virtual Item Item { get; set; }
    }
}
