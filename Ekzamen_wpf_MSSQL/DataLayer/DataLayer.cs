using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P_320_CompanyBD_Dagirov.Models;

namespace P_320_CompanyBD_Dagirov.DataLayer
{
    public class DL
    {
        public static string ConnectionString { get; set; } = ConfigurationManager.ConnectionStrings["Company_db"].ConnectionString;

        // ============ Статические методы для работы с Book (используют BookModel) ============

        // Получение книги по ID (использует индексы массива)
        public static BookModel Book_ByID_Model(int bookId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("stp_BookById", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@bookId", bookId);

                SqlDataReader dr = cmd.ExecuteReader();
                BookModel bm = null;

                if (dr.Read())
                {
                    // Обращение через индексы массива КАК В CustomerModel
                    bm = new BookModel(
                        (int)dr[0],            // id
                        dr[1].ToString(),      // name
                        (int)dr[2],            // pages
                        Convert.ToInt32(dr[3]), // price
                        (DateTime)dr[4],       // publish_date
                        (int)dr[5],            // author_id
                        (int)dr[6]             // theme_id
                    );
                }
                dr.Close();
                return bm;
            }
        }

        // Получение всех книг (использует индексы массива)
        public static List<BookModel> Book_ALL_Model()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("stp_bookALL", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader dr = cmd.ExecuteReader();
                List<BookModel> list = new List<BookModel>();

                while (dr.Read())
                {
                    // Обращение через индексы массива КАК В CustomerModel
                    list.Add(new BookModel(
                        (int)dr[0],            // id
                        dr[1].ToString(),      // name
                        (int)dr[2],            // pages
                        Convert.ToInt32(dr[3]), // price
                        (DateTime)dr[4],       // publish_date
                        (int)dr[5],            // author_id
                        (int)dr[6]             // theme_id
                    ));
                }
                dr.Close();
                return list;
            }
        }

        // ============ Ваши существующие методы (оставляем для совместимости) ============

        public static void Book_Add(string connstr)
        {
            Console.WriteLine($"Введите через Enter:");
            Console.WriteLine("1. Название книги:");
            string name = Console.ReadLine();

            Console.WriteLine("2. Количество страниц:");
            int pages = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("3. Цену:");
            decimal price = Convert.ToDecimal(Console.ReadLine());

            Console.WriteLine("4. ID автора:");
            int authorId = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("5. ID темы:");
            int themeId = Convert.ToInt32(Console.ReadLine());

            int newBookId = 0;

            using (SqlConnection conn = new SqlConnection(connstr))
            {
                conn.Open();

                SqlCommand command = new SqlCommand("stp_BookAdd", conn);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@pages", pages);
                command.Parameters.AddWithValue("@price", price);
                command.Parameters.AddWithValue("@publish_date", DateTime.Today);
                command.Parameters.AddWithValue("@author_id", authorId);
                command.Parameters.AddWithValue("@theme_id", themeId);

                SqlParameter returnParam = new SqlParameter("@ReturnVal", SqlDbType.Int);
                returnParam.Direction = ParameterDirection.ReturnValue;
                command.Parameters.Add(returnParam);

                command.ExecuteNonQuery();
                newBookId = (int)returnParam.Value;
            }

            Console.WriteLine($"Книга добавлена с ID: {newBookId}");
            Console.ReadKey();
        }

        public static void Book_Delete(string connstr)
        {
            Console.WriteLine($"Введите ID книги для удаления:");
            int bookId = Convert.ToInt32(Console.ReadLine());

            int deletedRows = 0;

            using (SqlConnection conn = new SqlConnection(connstr))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("stp_BookDelete", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@id", bookId);
                deletedRows = command.ExecuteNonQuery();
            }

            if (deletedRows > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Книга с ID {bookId} успешно удалена. Удалено записей: {deletedRows}");
                Console.ReadKey();
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Книга с ID {bookId} не найдена");
                Console.ReadKey();
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        // Обновленный Book_ALL - теперь использует BookModel
        public static void Book_ALL(string connstr)
        {
            // Используем метод с BookModel
            List<BookModel> books = Book_ALL_Model();

            // Заголовок таблицы
            Console.WriteLine("ID   | Название книги                 | Страниц | Цена     | Дата публикации | Авт | Тем");


            // Выводим книги
            foreach (var book in books)
            {
                // Форматируем вывод как в CustomerModel
                string name = book.Name.Length > 30 ? book.Name.Substring(0, 27) + "..." : book.Name;
                Console.WriteLine($"{book.Id.ToString().PadRight(4)} | {name.PadRight(30)} | " +
                                  $"{book.Pages.ToString().PadRight(7)} | {book.Price.ToString().PadRight(8)} | " +
                                  $"{book.PublishDate.ToShortDateString().PadRight(15)} | " +
                                  $"{book.AuthorId.ToString().PadRight(3)} | {book.ThemeId.ToString().PadRight(6)}");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nДля продолжения нажмите любую клавишу");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
        }

        // Обновленный Book_ByID - теперь использует BookModel
        public static void Book_ByID(string connstr)
        {
            Console.WriteLine($"ID книги: ");
            int bookId = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine($"________________________________________________\n");

            // Используем метод с BookModel
            BookModel book = Book_ByID_Model(bookId);

            if (book == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nКнига с ID {bookId} не найдена.");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.WriteLine("ID   | Название книги                 | Страниц | Цена     | Дата публикации | Авт | Тем");

                string name = book.Name.Length > 30 ? book.Name.Substring(0, 27) + "..." : book.Name;
                Console.WriteLine($"{book.Id.ToString().PadRight(4)} | {name.PadRight(30)} | " +
                                  $"{book.Pages.ToString().PadRight(7)} | {book.Price.ToString().PadRight(8)} | " +
                                  $"{book.PublishDate.ToShortDateString().PadRight(15)} | " +
                                  $"{book.AuthorId.ToString().PadRight(3)} | {book.ThemeId.ToString().PadRight(3)}");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nДля продолжения нажмите любую клавишу");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
        }

        public static void Book_Update_Price(string connstr)
        {
            Console.WriteLine($"ID книги: ");
            int bookId = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine($"новая цена: ");
            decimal newPrice = Convert.ToDecimal(Console.ReadLine());

            Console.WriteLine($"________________________________________________\n");

            int rowsAffected = 0;

            using (SqlConnection conn = new SqlConnection(connstr))
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

            if (rowsAffected > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Цена книги с ID {bookId} успешно обновлена на {newPrice:C}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Книга с ID {bookId} не найдена");
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
        }

        public static void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("Выберите процедуру: ");
            Console.WriteLine("1. BookALL");
            Console.WriteLine("2. BookAdd");
            Console.WriteLine("3. BookDelete");
            Console.WriteLine("4. BookById");
            Console.WriteLine("5. BookUpdate");
            Console.WriteLine("0. выход");
            Console.Write("Ваш выбор: ");
        }
    }
}
