using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoodmoParser.Entities
{
    public class Number : BaseEntity
    {
        public string Name { get; set; }

        public Guid ItemId { get; set; }
    }
}
