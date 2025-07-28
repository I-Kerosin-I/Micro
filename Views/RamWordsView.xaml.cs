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
    /// Логика взаимодействия для RamWordsView.xaml
    /// </summary>
    public partial class RamWordsView : UserControl
    {
        public RamWordsView()
        {
            InitializeComponent();
        }
        // TODO: Вытащить валидаторы в отдельный файл
        
        private void RamWordTextChangedValidator(object sender, TextChangedEventArgs e)
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

            if (textBox.Text.Length > 4) // При переполнении поля содержимое сдвигается новым символом влево если он вставлен в конец или вправо, если в середину (Ctrl + V обрабатывается)
            {
                if (textBox.CaretIndex - textBox.Text.Length + textBox.Tag.ToString().Length == 4)
                {
                    textBox.Text = textBox.Text.Substring(textBox.Text.Length - 4, 4);
                    textBox.CaretIndex = 4;
                }
                else
                {
                    var index = textBox.CaretIndex;
                    textBox.Text = textBox.Text.Substring(0, 4);
                    textBox.CaretIndex = index;
                }
            }

            textBox.Tag = textBox.Text;
        }

        private void RamWordTextInputValidator(object sender, TextCompositionEventArgs e) // Валидирует введённый символ, чтобы не ебаться с кареткой в TextChanged
        {
            var textBox = sender as TextBox;

            if (!Regex.IsMatch(e.Text, "^[0-9A-Fa-f]+$")) // Введено не 16-ричная цифра
            {
                e.Handled = true;
            }
        }
    }
}
