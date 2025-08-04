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
    /// Логика взаимодействия для RamBytesView.xaml
    /// </summary>
    public partial class RamBytesView : UserControl
    {
        public RamBytesView()
        {
            InitializeComponent();
        }
        
        private void RamByteTextChangedValidator(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            // Разрешаем только 0-9, A-F, a-f

            if (!Regex.IsMatch(textBox.Text, "^[0-9A-Fa-f]*$")) // В поле не 16-ричное число
            {
                textBox.Text = (string)textBox.Tag; // Блокируем ввод, вставляем заведомо валидное значение
                return;
            }

            if (textBox.Text.Length == 1)
            {
                textBox.Text = textBox.Text.ToUpper(); // Отдельная обработка первого символа
            }

            if (textBox.Text.Length > 2) // При переполнении поля содержимое сдвигается новым символом влево если он вставлен в конец или вправо, если в середину (Ctrl + V обрабатывается)
            {
                if (textBox.CaretIndex - textBox.Text.Length + textBox.Tag.ToString().Length == 2)
                {
                    textBox.Text = textBox.Text.Substring(textBox.Text.Length - 2, 2);
                    textBox.CaretIndex = 2;
                }
                else
                {
                    var index = textBox.CaretIndex;
                    textBox.Text = textBox.Text.Substring(0, 2);
                    textBox.CaretIndex = index;
                }
            }

            textBox.Tag = textBox.Text;
        }

        private void RamByteTextInputValidator(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;

            if (!Regex.IsMatch(e.Text, "^[0-9A-Fa-f]+$")) // Введено не 16-ричная цифра
            {
                e.Handled = true;
            }
        }
    }
}
