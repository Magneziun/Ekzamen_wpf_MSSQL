using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ekzamen_wpf_MSSQL.Models
{
    [Table("books")]
    public class BookModel
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        [Required(ErrorMessage = "Название книги обязательно")]
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
        public int AuthorId { get; set; }

        [Column("theme_id")]
        [Required(ErrorMessage = "ID темы обязателен")]
        public int ThemeId { get; set; }

        // Конструктор  для DataGrid
        public BookModel() { }

        // тут короче определяем через конструктор но можно и без
        public BookModel(string name, int pages, decimal price,
                        int authorId, int themeId, DateTime? publishDate = null)
        {
            Name = name;
            Pages = pages;
            Price = price;
            AuthorId = authorId;
            ThemeId = themeId;
            PublishDate = DateTime.Today; 
        }

        // Создаем метод чтобы потом проверять на валидность данных которые введем
        public bool Validate(out string errorMessage)
        {
            errorMessage = string.Empty;

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

            return string.IsNullOrEmpty(errorMessage);
        }

        // клонируем чтобы использовать вне
        public BookModel Clone()
        {
            return new BookModel
            {
                Id = this.Id,
                Name = this.Name,
                Pages = this.Pages,
                Price = this.Price,
                PublishDate = this.PublishDate,
                AuthorId = this.AuthorId,
                ThemeId = this.ThemeId
            };
        }
    }
}