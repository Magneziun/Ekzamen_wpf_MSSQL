using System;
using System.Collections.Generic;
using System.Data;
using Ekzamen_wpf_MSSQL.DataLayer;
using Ekzamen_wpf_MSSQL.Models;

namespace Ekzamen_wpf_MSSQL.Services
{
    // Сервисный класс для работы с книгами (фасад для DL)
    public class BookDataService
    {
        // ОТКЛЮЧЕННЫЙ РЕЖИМ (для просмотра) __________________________________

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

            // Конвертируем DataRow в объекты BookModel
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

        // Поиск книг по ключевому слову (отключенный режим)
        public DataTable SearchBooks(string keyword)
        {
            return DL.SearchBooksDisconnected(keyword);
        }

        // ПОДКЛЮЧЕННЫЙ РЕЖИМ (для изменений) __________________________________

        // Получить книгу по ID (подключенный режим)
        public BookModel GetBookById(int id)
        {
            return DL.GetBookByIdConnected(id);
        }

        // Добавить книгу через объект (подключенный режим)
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

        // Удалить книгу по ID (подключенный режим)
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

        // Получить книги по автору (фильтрация в памяти)
        public DataTable GetBooksByAuthor(int authorId)
        {
            using (var table = DL.GetAllBooksDisconnected())
            {
                // Применяем фильтр по author_id
                table.DefaultView.RowFilter = $"author_id = {authorId}";
                return table.DefaultView.ToTable();
            }
        }

        // СТАТИСТИКА ##########################################################

        // Общая стоимость всех книг
        public decimal GetAllPrice()
        {
            using (var table = DL.GetAllBooksDisconnected())
            {
                decimal total = 0;
                foreach (DataRow row in table.Rows)
                {
                    total += Convert.ToDecimal(row["price"]);
                }
                return total;
            }
        }

        // Общее количество страниц во всех книгах
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

        // Средняя цена книги
        public decimal GetAvgPrice()
        {
            using (var table = DL.GetAllBooksDisconnected())
            {
                if (table.Rows.Count == 0)
                    return 0;

                decimal total = 0;
                foreach (DataRow row in table.Rows)
                {
                    total += Convert.ToDecimal(row["price"]);
                }

                // Делим общую сумму на количество книг
                return total / table.Rows.Count;
            }
        }
    }
}