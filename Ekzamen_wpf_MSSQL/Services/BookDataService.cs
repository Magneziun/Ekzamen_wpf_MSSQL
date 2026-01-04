using System;
using System.Collections.Generic;
using System.Data;
using Ekzamen_wpf_MSSQL.DataLayer;
using Ekzamen_wpf_MSSQL.Models;

namespace Ekzamen_wpf_MSSQL.Services
{
    public class BookDataService
    {
        // ============ ОТКЛЮЧЕННЫЙ РЕЖИМ (для просмотра) ============

        // Получить все книги как DataTable (отключенный режим)
        public DataTable GetAllBooksTable()
        {
            return DL.GetAllBooksDisconnected();
        }

        // Получить все книги как список (отключенный режим)
        public List<BookModel> GetAllBooksList()
        {
            var books = new List<BookModel>();
            var table = DL.GetAllBooksDisconnected();

            foreach (DataRow row in table.Rows)
            {
                books.Add(new BookModel
                {
                    Id = Convert.ToInt32(row["id"]),
                    Name = row["name"].ToString(),
                    Pages = Convert.ToInt32(row["pages"]),
                    Price = Convert.ToDecimal(row["price"]),
                    PublishDate = Convert.ToDateTime(row["publish_date"]),
                    AuthorId = Convert.ToInt32(row["author_id"]),
                    ThemeId = Convert.ToInt32(row["theme_id"])
                });
            }

            return books;
        }

        // Поиск книг (отключенный режим)
        public DataTable SearchBooks(string keyword)
        {
            return DL.SearchBooksDisconnected(keyword);
        }

        // ============ ПОДКЛЮЧЕННЫЙ РЕЖИМ (для изменений) ============

        // Получить книгу по ID (подключенный режим)
        public BookModel GetBookById(int id)
        {
            return DL.GetBookByIdConnected(id);
        }

        // Добавить книгу (подключенный режим)
        public int AddBook(BookModel book)
        {
            return DL.AddBookConnected(
                book.Name,
                book.Pages,
                book.Price,
                book.AuthorId,
                book.ThemeId
            );
        }

        // Добавить книгу с параметрами (подключенный режим)
        public int AddBook(string name, int pages, decimal price,
                          int authorId, int themeId)
        {
            return DL.AddBookConnected(name, pages, price, authorId, themeId);
        }

        // Удалить книгу (подключенный режим)
        public bool DeleteBook(int id)
        {
            return DL.DeleteBookConnected(id);
        }

        // Обновить цену книги (подключенный режим)
        public bool UpdateBookPrice(int id, decimal newPrice)
        {
            return DL.UpdateBookPriceConnected(id, newPrice);
        }

        // Обновить всю книгу (подключенный режим)
        public bool UpdateBook(BookModel book)
        {
            return DL.UpdateBookConnected(book);
        }

        // ============ СТАТИСТИКА И АНАЛИТИКА ============

        public decimal GetAveragePrice()
        {
            using (var table = DL.GetAllBooksDisconnected())
            {
                decimal total = 0;
                foreach (DataRow row in table.Rows)
                {
                    total += Convert.ToDecimal(row["price"]);
                }
                return table.Rows.Count > 0 ? total / table.Rows.Count : 0;
            }
        }

        public int GetTotalPages()
        {
            using (var table = DL.GetAllBooksDisconnected())
            {
                int total = 0;
                foreach (DataRow row in table.Rows)
                {
                    total += Convert.ToInt32(row["pages"]);
                }
                return total;
            }
        }

        public DataTable GetBooksByAuthor(int authorId)
        {
            using (var table = DL.GetAllBooksDisconnected())
            {
                table.DefaultView.RowFilter = $"author_id = {authorId}";
                return table.DefaultView.ToTable();
            }
        }
    }
}