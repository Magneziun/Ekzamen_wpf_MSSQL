using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekzamen_wpf_MSSQL.Models
{
    [Table("authors")]
    public class Author
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name", TypeName = "nvarchar")]
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public virtual ICollection<BookModel> Books { get; set; }
    }
}
