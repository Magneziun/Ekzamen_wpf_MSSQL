using System;
using System.Windows;

namespace Ekzamen_wpf_MSSQL
{
    public partial class AddBookMenu : Window
    {
        // Конструктор окна добавления книги
        public AddBookMenu()
        {
            InitializeComponent();
            // Включаем окно
            this.IsEnabled = true;
            // Устанавливаем фокус на поле ввода названия
            Name.Focus();
        }

        // Обработчик нажатия кнопок
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Если нажали "Добавить"
            if (sender == AddButt)
            {
                // Проверяем введенные данные
                if (!ValidateInputs())
                    return;

                // Закрываем окно с результатом true
                this.DialogResult = true;
                this.Close();
            }
            // Если нажали "Выйти" (отмена)
            else if (sender == ExitBut)
            {
                this.Close();
            }
        }

        // Проверка введенных данных на корректность
        private bool ValidateInputs()
        {
            // Проверка названия книги
            if (string.IsNullOrWhiteSpace(Name.Text))
            {
                MessageBox.Show("Введите название книги", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                Name.Focus();
                return false;
            }

            // Проверка количества страниц (должно быть положительное число)
            if (!int.TryParse(Pages_Count.Text, out int pages) || pages <= 0)
            {
                MessageBox.Show("Введите корректное количество страниц", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                Pages_Count.Focus();
                return false;
            }

            // Проверка цены (должна быть положительной)
            if (!decimal.TryParse(Price.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Введите корректную цену", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                Price.Focus();
                return false;
            }

            // Проверка ID автора (должен быть положительным числом)
            if (!int.TryParse(AuthorID.Text, out int authorId) || authorId <= 0)
            {
                MessageBox.Show("Введите корректный ID автора (положительное число)", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                AuthorID.Focus();
                return false;
            }

            // Проверка ID темы (должен быть положительным числом)
            if (!int.TryParse(Theme_ID.Text, out int themeId) || themeId <= 0)
            {
                MessageBox.Show("Введите корректный ID темы (положительное число)", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                Theme_ID.Focus();
                return false;
            }

            // Все проверки пройдены
            return true;
        }
    }
}