using System;
using System.Windows;

namespace Ekzamen_wpf_MSSQL
{
    public partial class AddBookMenu : Window
    {
        public AddBookMenu()
        {
            InitializeComponent();
            this.IsEnabled = true;
            // Устанавливаем текущую дату как значение по умолчанию
            var today = DateTime.Today;

            // Очищаем текстбоксы
            Name.Text = "";
            Pages_Count.Text = "";
            Price.Text = "";
            AuthorID.Text = "";
            Theme_ID.Text = "";

            // Устанавливаем фокус
            Name.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //если нажали добавить 
            if (sender == AddButt)
            {
                // Валидация данных
                if (!ValidateInputs())
                    return;

                this.DialogResult = true;
                this.Close();
            }
            //если нажали добавить (просто закрывает окошко)
            else if (sender == ExitBut)
            {
                this.Close();
            }
        }
        //проверки на вводимые данные в окне где мы книгу новую делаем
        private bool ValidateInputs()
        {
            // Проверка названия
            if (string.IsNullOrWhiteSpace(Name.Text))
            {
                MessageBox.Show("Введите название книги", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                Name.Focus();
                return false;
            }

            // Проверка количества страниц
            if (!int.TryParse(Pages_Count.Text, out int pages) || pages <= 0 || pages > 5000)
            {
                MessageBox.Show("Введите корректное количество страниц (1-5000)", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                Pages_Count.Focus();
                return false;
            }

            // Проверка цены 
            if (!decimal.TryParse(Price.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Введите корректную цену", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                Price.Focus();
                return false;
            }

            // Проверка ID автора
            if (!int.TryParse(AuthorID.Text, out int authorId) || authorId <= 0)
            {
                MessageBox.Show("Введите корректный ID автора (положительное число)", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                AuthorID.Focus();
                return false;
            }

            // Проверка ID темы
            if (!int.TryParse(Theme_ID.Text, out int themeId) || themeId <= 0)
            {
                MessageBox.Show("Введите корректный ID темы (положительное число)", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                Theme_ID.Focus();
                return false;
            }

            return true;
        }
    }
}