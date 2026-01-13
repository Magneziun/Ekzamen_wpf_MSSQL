using System;
using System.Windows;

namespace Ekzamen_wpf_MSSQL
{
    public partial class EditPrice : Window
    {
        public decimal NewPrice { get; private set; }
        private decimal _currentPrice;

        public EditPrice(decimal currentPrice)
        {
            InitializeComponent();
            _currentPrice = currentPrice;
            EditPriceTexBox.Text = currentPrice.ToString();
            EditPriceTexBox.Focus();
            EditPriceTexBox.SelectAll();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ValidatePrice())
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private bool ValidatePrice()
        {
            if (string.IsNullOrWhiteSpace(EditPriceTexBox.Text))
            {
                MessageBox.Show("Введите новую цену", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                EditPriceTexBox.Focus();
                return false;
            }

            if (!decimal.TryParse(EditPriceTexBox.Text, out decimal newPrice) || newPrice <= 0)
            {
                MessageBox.Show("Введите корректную цену (положительное число)", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                EditPriceTexBox.Focus();
                EditPriceTexBox.SelectAll();
                return false;
            }

            NewPrice = newPrice;
            return true;
        }

        private void EditPriceTexBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (ValidatePrice())
                {
                    this.DialogResult = true;
                    this.Close();
                }
            }
            else if (e.Key == System.Windows.Input.Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
            }
        }
    }
}