using Entities;

namespace BoodmoParser.Entities
{
    public class Number : BaseEntity
    {
        public string Name { get; set; }

        public int? ItemId { get; set; }

        public virtual Item Item { get; set; }

        public bool Done { get; set; }
    }
}
