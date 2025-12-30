using System;

namespace P_320_CompanyBD_Dagirov.Models
{
    public class BookModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Pages { get; set; }
        public int Price { get; set; }
        public DateTime PublishDate { get; set; }
        public int AuthorId { get; set; }
        public int ThemeId { get; set; }

        // Конструктор для использования с индексами массива
        public BookModel(int id, string name, int pages, int price, DateTime publishDate, int authorId, int themeId)
        {
            Id = id;
            Name = name;
            Pages = pages;
            Price = price;
            PublishDate = publishDate;
            AuthorId = authorId;
            ThemeId = themeId;
        }

        public override string ToString()
        {
            return $"{Id,4} {Name,30} {Pages,6} {Price,8:C} {PublishDate.ToShortDateString(),15} {AuthorId,3} {ThemeId,3}";
        }
    }
}
