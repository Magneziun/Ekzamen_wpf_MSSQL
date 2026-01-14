using Ekzamen_wpf_MSSQL.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace Ekzamen_wpf_MSSQL.DataLayer
{
    public class DL
    {
        // Строка подключения к БД из конфига
        private static string ConnectionString { get; set; } =
            ConfigurationManager.ConnectionStrings["Company_db"].ConnectionString;

        // ПОДКЛЮЧЕННЫЙ РЕЖИМ (через DbContext) ################################

        // Получаем книгу по ID через хранимую процедуру
        public static BookModel GetBookByIdConnected(int bookId)
        {
            using (var context = new BookShopContext())
            {
                // Вызов хранимой процедуры через Entity Framework
                var result = context.Database.SqlQuery<BookModel>( 
                    "EXEC stp_BookById @bookId",  
                    new SqlParameter("@bookId", bookId)
                ).FirstOrDefault();

                return result;
            }
        }

        // Добавляем книгу через хранимую процедуру
        public static int AddBookConnected(string name, int pages, decimal price,
                                          int authorId, int themeId)
        {
            using (var context = new BookShopContext())
            {
                context.Database.ExecuteSqlCommand(
                     "EXEC stp_BookAdd @name, @pages, @price, @publish_date, @author_id, @theme_id",
                     new SqlParameter("@name", name),
                     new SqlParameter("@pages", pages),
                     new SqlParameter("@price", price),
                     new SqlParameter("@publish_date", DateTime.Today),
                     new SqlParameter("@author_id", authorId),
                     new SqlParameter("@theme_id", themeId)
                 );

                // Получаем последний ID (максимальный в таблице)
                return context.Books.Max(b => b.Id); //LINQ пример с агрегатной функцией
            }
        }

        // Удаляем книгу через хранимую процедуру
        public static bool DeleteBookConnected(int bookId)
        {
            using (var context = new BookShopContext())
            {
                var rowsAffected = context.Database.ExecuteSqlCommand(
                    "EXEC stp_BookDelete @id",
                    new SqlParameter("@id", bookId)
                );

                // Возвращаем true если затронули хотя бы одну строку
                return rowsAffected > 0;
            }
        }

        // ОТКЛЮЧЕННЫЙ РЕЖИМ ###################################################

        // Получаем все книги в виде DataTable (отключенный режим)
        public static DataTable GetAllBooksDisconnected()
        {
            using (var context = new BookShopContext())
            {
                // AsNoTracking() не отслеживаем изменения
                var books = context.Books.AsNoTracking().ToList();

                // Создаем DataTable и добавляем колонки
                DataTable dt = new DataTable("Books");
                dt.Columns.Add("id", typeof(int));
                dt.Columns.Add("name", typeof(string));
                dt.Columns.Add("pages", typeof(int));
                dt.Columns.Add("price", typeof(decimal));
                dt.Columns.Add("publish_date", typeof(DateTime));
                dt.Columns.Add("author_id", typeof(int));
                dt.Columns.Add("theme_id", typeof(int));

                // Заполняем циклом DataTable данными из списка книг
                foreach (var book in books)
                {
                    dt.Rows.Add(
                        book.Id,
                        book.Name,
                        book.Pages,
                        book.Price,
                        book.PublishDate,
                        book.AuthorId,
                        book.ThemeId
                    );
                }

                return dt;
            }
        }

        // Обновление цены - подключенный режим через 
        public static bool UpdateBookPriceConnected(int bookId, decimal newPrice)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    // Вызываем хранимку обновления цены
                    using (SqlCommand cmd = new SqlCommand("stp_BookUpdate", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", bookId);
                        cmd.Parameters.AddWithValue("@Price", newPrice);

                        cmd.ExecuteNonQuery();

                        // Если не было исключения то считаем операцию успешной
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Показываем ошибку 
                MessageBox.Show($"Ошибка при обновлении цены книги: {ex.Message}",
                               "Ошибка БД",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
                return false;
            }
        }

        // Обновление всех полей книги - подключенный режим
        public static bool UpdateBookConnected(BookModel book)
        {
            int rowsAffected = 0;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                // SQL запрос для обновления всех полей книги
                string sql = @"
                    UPDATE books 
                    SET name = @name, 
                        pages = @pages, 
                        price = @price, 
                        publish_date = @publish_date,
                        author_id = @author_id, 
                        theme_id = @theme_id 
                    WHERE id = @id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // Добавляем параметры 
                    cmd.Parameters.AddWithValue("@id", book.Id);
                    cmd.Parameters.AddWithValue("@name", book.Name);
                    cmd.Parameters.AddWithValue("@pages", book.Pages);
                    cmd.Parameters.AddWithValue("@price", book.Price);
                    cmd.Parameters.AddWithValue("@publish_date", book.PublishDate);
                    cmd.Parameters.AddWithValue("@author_id", book.AuthorId);
                    cmd.Parameters.AddWithValue("@theme_id", book.ThemeId);

                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }

            // Возвращаем true если обновили запись
            return rowsAffected > 0;
        }

        // Поиск книг по названию - отключенный режим
        public static DataTable SearchBooksDisconnected(string keyword)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                // SQL запрос с LIKE для поиска по части названия
                string sql = "SELECT * FROM books WHERE name LIKE @keyword";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand.Parameters.AddWithValue("@keyword", $"%{keyword}%");

                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }
    }
}