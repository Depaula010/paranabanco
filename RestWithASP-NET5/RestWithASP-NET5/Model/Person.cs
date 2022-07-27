using RestWithASP_NET5.Model.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RestWithASP_NET5.Model
{
    [Table("person")]
    public class Person : BaseEntity
    {
        [Column("complet_name")]
        public string CompletName { get; set; }
        [Column("email")]
        public string Email { get; set; }
 
    }
}
