using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekzamen_wpf_MSSQL.Models
{
    // Модель автора для Entity Framework
    [Table("authors")]
    public class Author
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name", TypeName = "nvarchar")]
        [Required] // Обязательное поле
        [MaxLength(100)] // Максимальная длина
        public string Name { get; set; }

        // Навигационное свойство для связи с книгами
        public virtual ICollection<BookModel> Books { get; set; }
    }
}