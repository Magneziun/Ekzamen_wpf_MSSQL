using System;
using System.Windows;
using System.Windows.Controls;
using Ekzamen_wpf_MSSQL.Services;
using Ekzamen_wpf_MSSQL.Models;
using System.Linq;

namespace Ekzamen_wpf_MSSQL
{
    public partial class MainWindow : Window
    {
        private BookDataService _bookService;
        private BookModel _selectedBook;

        public MainWindow()
        {
            InitializeComponent();
            _bookService = new BookDataService();
            LoadBooks(); // Отключенный режим для просмотра
            UpdateStats(); // вывод статуса
            this.Focus(); //фокус на окно
        }

        // ============ ОТКЛЮЧЕННЫЙ РЕЖИМ ============
        // Загрузка книг для просмотра (без соединения в откл режиме)
        private void LoadBooks()
        {
            //обработка исключения
            try
            {
                // Используем отключенный режим
                ConnectionStatus.Text = "Отключенный режим";
                var books = _bookService.GetAllBooksList();
                // заполняем ДатуГрид книгами и отоброжаем статус
                DataGridBooks.ItemsSource = books;
                StatusBarText.Text = $"Загружено {books.Count} книг (отключенный режим)";


                //если книг нет чтобы ничего не выделялось
                if (books.Count > 0)
                {
                    DataGridBooks.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка загрузки книг: {ex.Message}");
            }
        }

        // Обновление статистики
        private void UpdateStats()
        {
            try
            {
                var books = _bookService.GetAllBooksList();
                var avgPrice = _bookService.GetAllPrice();
                var totalPages = _bookService.GetTotalPages();

                TxtTotalBooks.Text = $"Всего книг: {books.Count}";
                TxtAvgPrice.Text = $"Средняя цена: {avgPrice:C}";
                TxtTotalPages.Text = $"Всего страниц: {totalPages:#,##0}";
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка обновления статистики: {ex.Message}");
            }
        }

        // Поиск книг
        private void SearchBooks(string keyword)
        {
            try
            {
                var books = _bookService.GetAllBooksList()
                    .Where(b => b.Name.ToLower().Contains(keyword.ToLower()) ||
                                b.Id.ToString().Contains(keyword))
                    .ToList();

                DataGridBooks.ItemsSource = books;
                StatusBarText.Text = $"Найдено {books.Count} книг по запросу '{keyword}'";
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка поиска: {ex.Message}");
            }
        }

        // ============ ПОДКЛЮЧЕННЫЙ РЕЖИМ ============
        // Для операций изменения данных

        private void DataGridBooks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridBooks.SelectedItem is BookModel selectedBook)
            {
                _selectedBook = selectedBook;
                ShowBookDetails(selectedBook);
            }
        }

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

        // ============ ОБРАБОТЧИКИ КНОПОК ============

        //кнопка обновить
        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadBooks();
            UpdateStats();
            StatusBarText.Text = "Список книг обновлен";
        }
        //кнопка добавить
        private void BtnAddBook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Используем подключенный режим для добавления
                ConnectionStatus.Text = "Подключенный режим";

                var addWindow = new AddBookMenu();
                if (addWindow.ShowDialog() == true)
                {
                    // Создаем новую книгу переменый new book и заполняем все поля из окна добавления книги
                    var newBook = new BookModel
                    {
                        Name = addWindow.Name.Text,
                        Pages = int.Parse(addWindow.Pages_Count.Text),
                        Price = decimal.Parse(addWindow.Price.Text),
                        PublishDate = DateTime.Today,
                        AuthorId = int.Parse(addWindow.AuthorID.Text),
                        ThemeId = int.Parse(addWindow.Theme_ID.Text)
                    };

                    // проверка на правильность

                    if (!newBook.Validate(out string error))
                    {
                        ShowError(error);
                        return;
                    }

                    // Добавляем книгу (подключенный режим)
                    int newId = _bookService.AddBook(newBook);

                    // Обновляем список (отключенный режим)
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
                ConnectionStatus.Text = "Отключенный режим";
            }
        }

        //кнопка удалить
        private void BtnDeleteBook_Click(object sender, RoutedEventArgs e)
        {
            
            if (_selectedBook == null)
            {
                ShowError("Выберите книгу для удаления");
                return;
            }

            //подтверждение удаления
            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить книгу:\n'{_selectedBook.Name}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Используем подключенный режим для удаления
                    ConnectionStatus.Text = "Подключенный режим";

                    bool success = _bookService.DeleteBook(_selectedBook.Id);

                    if (success)
                    {
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

        //кнопка измененние цены
        private void BtnUpdatePrice_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedBook == null)
            {
                ShowError("Выберите книгу для изменения цены");
                return;
            }

            var editPriceWindow = new EditPrice(_selectedBook.Price);
            editPriceWindow.Title = $"Изменение цены: {_selectedBook.Name}";

            if (editPriceWindow.ShowDialog() == true)
            {
                decimal newPrice = editPriceWindow.NewPrice;

                if (newPrice > 0 && newPrice != _selectedBook.Price)
                {
                    try
                    {
                        // Используем подключенный режим для обновления
                        ConnectionStatus.Text = "Подключенный режим";

                        bool success = _bookService.UpdateBookPrice(_selectedBook.Id, newPrice);

                        if (success)
                        {
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

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtSearch.Text))
            {
                LoadBooks();
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtSearch.Text))
            {
                SearchBooks(TxtSearch.Text);
            }
        }

        // ============ ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ============

        private void ShowError(string message)
        {
            StatusBarText.Text = $"{message}";
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ShowMessage(string message)
        {
            StatusBarText.Text = $"{message}";
        }
    }

    // Класс для диалога ввода изменения цены (не работает)
    public class InputDialog
    {
        public string Answer { get; set; }

        public InputDialog(string title, string prompt, string defaultValue = "")
        {
            var dialog = new Window
            {
                Title = title,
                Width = 300,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var stackPanel = new StackPanel { Margin = new Thickness(10) };

            stackPanel.Children.Add(new TextBlock
            {
                Text = prompt,
                Margin = new Thickness(0, 0, 0, 10)
            });

            var textBox = new TextBox
            {
                Text = defaultValue,
                Margin = new Thickness(0, 0, 0, 10)
            };
            stackPanel.Children.Add(textBox);

            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };

            var okButton = new Button
            {
                Content = "OK",
                Width = 75,
                Margin = new Thickness(0, 0, 10, 0),
                IsDefault = true
            };
            okButton.Click += (sender, e) =>
            {
                Answer = textBox.Text;
                dialog.DialogResult = true;
            };

            var cancelButton = new Button
            {
                Content = "Cancel",
                Width = 75,
                IsCancel = true
            };

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);
            stackPanel.Children.Add(buttonPanel);

            dialog.Content = stackPanel;
            dialog.ShowDialog();
        }

        public bool ShowDialog()
        {
            return false; // Реализация в конструкторе
        }
    }
}