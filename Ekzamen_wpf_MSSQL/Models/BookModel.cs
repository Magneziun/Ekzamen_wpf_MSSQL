using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ekzamen_wpf_MSSQL.Models
{
    // Модель книги для Entity Framework
    [Table("books")]
    public class BookModel
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name", TypeName = "nvarchar")]
        [Required(ErrorMessage = "Название книги обязательно")]
        [MaxLength(255)]
        public string Name { get; set; }

        [Column("pages")]
        public int Pages { get; set; }

        [Column("price")]
        [Required(ErrorMessage = "Цена обязательна")]
        public decimal Price { get; set; }

        [Column("publish_date")]
        [DataType(DataType.Date)]
        public DateTime PublishDate { get; set; }

        [Column("author_id")]
        [Required(ErrorMessage = "ID автора обязателен")]
        [ForeignKey("Author")] // Внешний ключ к таблице authors
        public int AuthorId { get; set; }

        [Column("theme_id")]
        [Required(ErrorMessage = "ID темы обязателен")]
        [ForeignKey("Theme")] // Внешний ключ к таблице themes
        public int ThemeId { get; set; }

        // Навигационные свойства для Code First (ленивая загрузка)
        public virtual Author Author { get; set; }
        public virtual Theme Theme { get; set; }

        // Конструктор по умолчанию для DataGrid и EF
        public BookModel() { }

        // Конструктор с параметрами для удобного создания объектов
        public BookModel(string name, int pages, decimal price,
                        int authorId, int themeId, DateTime? publishDate = null)
        {
            Name = name;
            Pages = pages;
            Price = price;
            AuthorId = authorId;
            ThemeId = themeId;
            PublishDate = DateTime.Today; // По умолчанию сегодняшняя дата
        }

        // Метод валидации данных книги
        public bool Validate(out string errorMessage)
        {
            errorMessage = string.Empty;

            // Проверяем все поля на корректность
            if (string.IsNullOrWhiteSpace(Name))
                errorMessage = "Название книги обязательно";
            else if (Pages <= 0)
                errorMessage = "Количество страниц должно быть положительным";
            else if (Price <= 0)
                errorMessage = "Цена должна быть положительной";
            else if (AuthorId <= 0)
                errorMessage = "ID автора должен быть положительным";
            else if (ThemeId <= 0)
                errorMessage = "ID темы должен быть положительным";

            // Возвращаем true если ошибок нет
            return string.IsNullOrEmpty(errorMessage);
        }
    }
}