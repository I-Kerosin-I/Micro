using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Micro.Views
{
    /// <summary>
    /// Логика взаимодействия для RamView.xaml
    /// </summary>
    public partial class RamView : UserControl
    {
        public RamView()
        {
            InitializeComponent();
        }
        private void HexInputValidator(object sender, TextCompositionEventArgs e)
        {

            TextBox textBox = sender as TextBox;
            // Разрешаем только 0-9, A-F, a-f
            if (!Regex.IsMatch(e.Text, "^[0-9A-Fa-f]+$") || (textBox.Text.Length >= 2 && textBox.SelectionLength == 0))
            {
                e.Handled = true; // Блокируем ввод
            }
        }
    }
}
