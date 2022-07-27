using System.ComponentModel.DataAnnotations.Schema;

namespace RestWithASP_NET5.Model.Base
{
    public class BaseEntity
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("email")]
        public string Email { get; set; }
    }
}
