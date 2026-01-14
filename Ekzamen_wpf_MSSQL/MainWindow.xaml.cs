using System;
using System.Windows;
using System.Windows.Controls;
using Ekzamen_wpf_MSSQL.Services;
using System.Data.Linq; // Добавляем для LINQ to SQL
using Ekzamen_wpf_MSSQL.Models;
using System.Linq;

namespace Ekzamen_wpf_MSSQL
{
    public partial class MainWindow : Window
    {
        // Сервис для работы с данными книг
        private BookDataService _bookService;
        // Выбранная книга в DataGrid
        private BookModel _selectedBook;

        // Конструктор главного окна
        public MainWindow()
        {
            InitializeComponent();
            // Создаем экземпляр сервиса для работы с книгами
            _bookService = new BookDataService();
            // Загружаем книги при запуске (отключенный режим)
            LoadBooks();
            // Обновляем статистику
            UpdateStats();
            // Устанавливаем фокус на окно
            this.Focus();
        }

        // ОТКЛЮЧЕННЫЙ РЕЖИМ
        // Загрузка книг для просмотра (без постоянного соединения)
        private void LoadBooks()
        {
            // Обработка исключений при загрузке
            try
            {
                // Показываем текущий режим работы
                ConnectionStatus.Text = "Отключенный режим";
                // Получаем все книги в виде списка
                var books = _bookService.GetAllBooksList();
                // Заполняем DataGrid книгами
                DataGridBooks.ItemsSource = books;
                // Обновляем статус в статус-баре
                StatusBarText.Text = $"Загружено {books.Count} книг (отключенный режим)";

                // Если книги есть, выделяем первую
                if (books.Count > 0)
                {
                    DataGridBooks.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                // Показываем ошибку если чтото пошло не так
                ShowError($"Ошибка загрузки книг: {ex.Message}");
            }
        }

        // Обновление статистики в правой панели
        private void UpdateStats()
        {
            try
            {
                // Получаем данные для статистики
                var books = _bookService.GetAllBooksList();
                var allPrice = _bookService.GetAllPrice();
                var totalPages = _bookService.GetTotalPages();
                var avgPrice = _bookService.GetAvgPrice();

                // Обновляем текстовые блоки со статистикой
                TxtTotalBooks.Text = $"Всего книг: {books.Count}";
                TxtAallPrice.Text = $"Общая стоимость: {allPrice:C}";
                TxtTotalPages.Text = $"Всего страниц: {totalPages:#,##0}";
                TxtAvgPrice.Text = $"Средняя цена: {avgPrice:C}";
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка обновления статистики: {ex.Message}");
            }
        }

        // Поиск книг по ключевому слову
        private void SearchBooks(string keyword)
        {
            try
            {
                // Фильтруем книги по названию или ID
                var books = _bookService.GetAllBooksList()
                    .Where(b => b.Name.ToLower().Contains(keyword.ToLower()) ||
                                b.Id.ToString().Contains(keyword)) //LINQ to Objects
                    .ToList();

                // Обновляем DataGrid отфильтрованными книгами
                DataGridBooks.ItemsSource = books;
                StatusBarText.Text = $"Найдено {books.Count} книг по запросу '{keyword}'";
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка поиска: {ex.Message}");
            }
        }

        // ПОДКЛЮЧЕННЫЙ РЕЖИМ 
        // Обработчик изменения выбранной книги в DataGrid
        private void DataGridBooks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Если выбрана книга, сохраняем ее и показываем детали
            if (DataGridBooks.SelectedItem is BookModel selectedBook)
            {
                _selectedBook = selectedBook;
                ShowBookDetails(selectedBook);
            }
        }

        // Показ детальной информации о книге в правой панели
        private void ShowBookDetails(BookModel book)
        {
            TxtBookDetails.Text =
                $"Детальная информация:\n\n" +
                $"ID: {book.Id}\n" +
                $"Название: {book.Name}\n" +
                $"Страниц: {book.Pages}\n" +
                $"Цена: {book.Price:C}\n" +
                $"Дата публикации: {book.PublishDate:dd.MM.yyyy}\n" +
                $"ID автора: {book.AuthorId}\n" +
                $"ID темы: {book.ThemeId}\n\n" +
                $"Цена за страницу: {(book.Price / book.Pages):C}";
        }

        // ОБРАБОТЧИКИ КНОПОК 

        // Кнопка Обновит - перезагружает список книг
        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadBooks();
            UpdateStats();
            StatusBarText.Text = "Список книг обновлен";
        }

        // Кнопка Добавить - открывает окно добавления новой книги
        private void BtnAddBook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Переходим в подключенный режим для добавления
                ConnectionStatus.Text = "Подключенный режим";

                // Открываем окно добавления книги
                var addWindow = new AddBookMenu();
                if (addWindow.ShowDialog() == true)
                {
                    // Создаем новую книгу из данных окна
                    var newBook = new BookModel
                    {
                        Name = addWindow.Name.Text,
                        Pages = int.Parse(addWindow.Pages_Count.Text),
                        Price = decimal.Parse(addWindow.Price.Text),
                        PublishDate = DateTime.Today,
                        AuthorId = int.Parse(addWindow.AuthorID.Text),
                        ThemeId = int.Parse(addWindow.Theme_ID.Text)
                    };

                    // Проверяем валидность данных
                    if (!newBook.Validate(out string error))
                    {
                        ShowError(error);
                        return;
                    }

                    // Добавляем книгу в БД (подключенный режим)
                    int newId = _bookService.AddBook(newBook);

                    // Обновляем список книг и статистику
                    LoadBooks();
                    UpdateStats();

                    StatusBarText.Text = $"Книга добавлена с ID: {newId}";
                    ShowMessage($"Книга '{newBook.Name}' успешно добавлена!");
                }
            }
            catch (FormatException)
            {
                ShowError("Ошибка формата данных. Проверьте введенные значения.");
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка добавления книги: {ex.Message}");
            }
            finally
            {
                // Возвращаемся в отключенный режим
                ConnectionStatus.Text = "Отключенный режим";
            }
        }

        // Кнопка Удалить - удаляет выбранную книгу
        private void BtnDeleteBook_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбрана ли книга
            if (_selectedBook == null)
            {
                ShowError("Выберите книгу для удаления");
                return;
            }

            // Подтверждение удаления через MessageBox
            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить книгу:\n'{_selectedBook.Name}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Переходим в подключенный режим для удаления
                    ConnectionStatus.Text = "Подключенный режим";

                    // Пытаемся удалить книгу
                    bool success = _bookService.DeleteBook(_selectedBook.Id);

                    if (success)
                    {
                        // Обновляем интерфейс после удаления
                        LoadBooks();
                        UpdateStats();
                        StatusBarText.Text = $"Книга '{_selectedBook.Name}' удалена";
                        ShowMessage("Книга успешно удалена!");
                        _selectedBook = null;
                        TxtBookDetails.Text = "Выберите книгу для просмотра деталей";
                    }
                    else
                    {
                        ShowError("Не удалось удалить книгу");
                    }
                }
                catch (Exception ex)
                {
                    ShowError($"Ошибка удаления: {ex.Message}");
                }
                finally
                {
                    ConnectionStatus.Text = "Отключенный режим";
                }
            }
        }

        // Кнопка Изменить цену - открывает окно редактирования цены
        private void BtnUpdatePrice_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedBook == null)
            {
                ShowError("Выберите книгу для изменения цены");
                return;
            }

            // Открываем окно редактирования цены с текущей ценой
            var editPriceWindow = new EditPrice(_selectedBook.Price);
            editPriceWindow.Title = $"Изменение цены: {_selectedBook.Name}";

            if (editPriceWindow.ShowDialog() == true)
            {
                decimal newPrice = editPriceWindow.NewPrice;

                // Проверяем, что цена изменилась и корректна
                if (newPrice > 0 && newPrice != _selectedBook.Price)
                {
                    try
                    {
                        ConnectionStatus.Text = "Подключенный режим";

                        // Обновляем цену в БД
                        bool success = _bookService.UpdateBookPrice(_selectedBook.Id, newPrice);

                        if (success)
                        {
                            // Обновляем интерфейс
                            LoadBooks();
                            UpdateStats();
                            StatusBarText.Text = $"Цена книги '{_selectedBook.Name}' обновлена на {newPrice:C}";
                            ShowMessage($"Цена успешно изменена с {_selectedBook.Price:C} на {newPrice:C}");
                        }
                        else
                        {
                            ShowError("Не удалось обновить цену");
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowError($"Ошибка обновления цены: {ex.Message}");
                    }
                    finally
                    {
                        ConnectionStatus.Text = "Отключенный режим";
                    }
                }
                else
                {
                    ShowError("Новая цена должна отличаться от текущей");
                }
            }
        }

        // Обработчик изменения текста в поле поиска
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Если поле поиска пустое, загружаем все книги
            if (string.IsNullOrWhiteSpace(TxtSearch.Text))
            {
                LoadBooks();
            }
        }

        // Кнопка Поиск - выполняет поиск по введенному тексту
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtSearch.Text))
            {
                SearchBooks(TxtSearch.Text);
            }
        }

        //ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ
        // Показ ошибки в статус-баре и MessageBox
        private void ShowError(string message)
        {
            StatusBarText.Text = $"{message}";
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        // Показ сообщения в статус-баре
        private void ShowMessage(string message)
        {
            StatusBarText.Text = $"{message}";
        }
    }
}