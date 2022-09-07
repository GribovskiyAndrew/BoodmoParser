using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entities
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public bool Done { get; set; }
    }
}
