﻿using Entities;

namespace BoodmoParser.Entities
{
    public class OEMReplacementParts : BaseEntity
    {
        public string PartsBrand { get; set; }

        public string Title { get; set; }

        public string PartNumber { get; set; }

        public double? Price { get; set; }

        public int ItemId { get; set; }

        public virtual Item Item { get; set; }
    }
}
