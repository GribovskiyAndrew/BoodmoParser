using Entities;

namespace BoodmoParser.Entities
{
    public class Item : BaseEntity
    {
        public virtual IList<AftermarketReplacementPart> AftermarketReplacementParts { get; set; }

        public virtual IList<OEMReplacementParts> OEMReplacementParts { get; set; }

        public virtual IList<OffersProvided> Offers { get; set; }

        public string PartsBrand { get; set; }

        public string Title { get; set; }

        public string? SoldBy { get; set; }

        public double? Price { get; set; }

        public string PartNumber { get; set; }

        public string Origin { get; set; }

        public string Class { get; set; }

        public string? Description { get; set; }
    }
}