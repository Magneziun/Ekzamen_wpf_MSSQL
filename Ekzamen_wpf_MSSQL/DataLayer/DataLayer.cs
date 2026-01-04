using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Ekzamen_wpf_MSSQL.Models;

namespace Ekzamen_wpf_MSSQL.DataLayer
{
    public class DL
    {
        private static string ConnectionString { get; set; } =
            ConfigurationManager.ConnectionStrings["Company_db"].ConnectionString;

        // ============ ПОДКЛЮЧЕННЫЙ РЕЖИМ ============
        // (SqlDataReader с открытым соединением)

        // Получение книги по ID - подключенный режим
        public static BookModel GetBookByIdConnected(int bookId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open(); // Подключаемся к БД

                SqlCommand cmd = new SqlCommand("stp_BookById", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@bookId", bookId);

                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                BookModel bm = null;

                if (dr.Read())
                {
                    bm = new BookModel
                    {
                        Id = (int)dr[0],
                        Name = dr[1].ToString(),
                        Pages = (int)dr[2],
                        Price = Convert.ToDecimal(dr[3]),
                        PublishDate = (DateTime)dr[4],
                        AuthorId = (int)dr[5],
                        ThemeId = (int)dr[6]
                    };
                }
                dr.Close();
                return bm;
            }
        }

        // ============ ОТКЛЮЧЕННЫЙ РЕЖИМ ============
        // (DataTable/DataSet без постоянного соединения)

        // Получение всех книг - отключенный режим
        public static DataTable GetAllBooksDisconnected()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("stp_bookALL", conn);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;

                DataTable dt = new DataTable("Books");
                da.Fill(dt); // Заполняем DataTable и закрываем соединение

                return dt;
            }
        }

        // Добавление книги - подключенный режим (транзакция)
        public static int AddBookConnected(string name, int pages, decimal price,
                                          int authorId, int themeId)
        {
            int newBookId = 0;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand("stp_BookAdd", conn, transaction);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@pages", pages);
                        cmd.Parameters.AddWithValue("@price", price);
                        cmd.Parameters.AddWithValue("@publish_date", DateTime.Today);
                        cmd.Parameters.AddWithValue("@author_id", authorId);
                        cmd.Parameters.AddWithValue("@theme_id", themeId);

                        SqlParameter returnParam = new SqlParameter("@ReturnVal", SqlDbType.Int);
                        returnParam.Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add(returnParam);

                        cmd.ExecuteNonQuery();
                        newBookId = (int)returnParam.Value;

                        transaction.Commit(); // Подтверждаем транзакцию
                    }
                    catch (Exception)
                    {
                        transaction.Rollback(); // Откатываем при ошибке
                        throw;
                    }
                }
            }

            return newBookId;
        }

        // Удаление книги - подключенный режим
        public static bool DeleteBookConnected(int bookId)
        {
            int deletedRows = 0;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("stp_BookDelete", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", bookId);
                deletedRows = cmd.ExecuteNonQuery();
            }

            return deletedRows > 0;
        }

        // Обновление цены - подключенный режим
        public static bool UpdateBookPriceConnected(int bookId, decimal newPrice)
        {
            int rowsAffected = 0;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("stp_BookUpdate", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", bookId);
                    cmd.Parameters.AddWithValue("@Price", newPrice);

                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }

            return rowsAffected > 0;
        }

        // Обновление всей книги - подключенный режим
        public static bool UpdateBookConnected(BookModel book)
        {
            int rowsAffected = 0;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

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

            return rowsAffected > 0;
        }

        // Поиск книг - отключенный режим
        public static DataTable SearchBooksDisconnected(string keyword)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
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