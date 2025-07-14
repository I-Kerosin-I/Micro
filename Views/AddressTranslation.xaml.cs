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
using static System.Net.Mime.MediaTypeNames;

namespace Micro.Views
{
    /// <summary>
    /// Логика взаимодействия для AddressTranslation.xaml
    /// </summary>
    public partial class AddressTranslation : UserControl
    {
        public AddressTranslation()
        {
            InitializeComponent();
        }
        // TODO: Вытащить валидаторы в отдельный файл
        private void AddressTextChangedValidator(object sender, TextChangedEventArgs e) // Основной валидатор, отслеживает первый символ и поле в целом
        {
            TextBox textBox = sender as TextBox;
            textBox.CharacterCasing = CharacterCasing.Upper; // Все символы в верхний регистр
            // Разрешаем только 0-9, A-F, a-f
            
            if (!Regex.IsMatch(textBox.Text, "^[0-9A-Fa-f]*$")) // В поле не 16-ричное число
            {
                textBox.Text = (string)textBox.Tag; // Блокируем ввод, вставляем заведомо валидное значение
                return;
            }
            
            
            //MessageBox.Show(updatedText);
            if (Convert.ToInt16("0"+textBox.Text, 16) > 63) // Адрес > 3F
            {
                textBox.Text = "3F";
                textBox.CaretIndex = 2;
            }

            if (textBox.Text.Length > 2)
            {
                textBox.Text = textBox.Text.Substring(textBox.Text.Length-2, 2);
                textBox.CaretIndex = 2;
            }
            
            textBox.Tag = textBox.Text;
        }
        private void AddressTextInputValidator(object sender, TextCompositionEventArgs e) // Валидирует введённый символ, чтобы не ебаться с кареткой в TextChanged
        {
            TextBox textBox = sender as TextBox;
            
            if (!Regex.IsMatch(e.Text, "^[0-9A-Fa-f]+$")) // Введено не 16-ричное число
            {
                e.Handled = true;
            }
        }
        
        private void OpcodeTextChangedValidator(object sender, TextChangedEventArgs e) // Нужен только для первого символа и вставки
        {
            TextBox textBox = sender as TextBox;
            textBox.CharacterCasing = CharacterCasing.Upper; 
            // Разрешаем только 0 1 X x, длина не больше 16 символов
            if (!Regex.IsMatch(textBox.Text, "^[01Xx]*$")) // Для первого символа
            {
                textBox.Text = (string)textBox.Tag; // Блокируем ввод, вставляем заведомо валидное значение
                return;
            }

            if (textBox.Text.Length > 16) // Для длинной вставки, обрезает до 16 символов
            {
                textBox.Text = textBox.Text.Substring(0,16);
                textBox.CaretIndex = textBox.Text.Length;
                return;
            }

            textBox.Tag = textBox.Text;
        }
        private void OpcodeTextInputValidator(object sender, TextCompositionEventArgs e) // Валидирует введённый символ, чтобы не ебаться с кареткой в TextChanged
        {
            TextBox textBox = sender as TextBox;

            if ((!Regex.IsMatch(e.Text, "^[01Xx]*$")) || (textBox.Text.Length >= 16 && textBox.SelectionLength == 0)) // Введено не 16-ричное число
            {
                e.Handled = true; // Блокируем ввод
            }
        }
        private void OpcodeLostFocusValidator(object sender, RoutedEventArgs e) // Дополняет нулями
        {
            TextBox textBox = sender as TextBox;

            if (textBox.Text.Length < 16)
            {
                textBox.Text = textBox.Text.PadRight(16, '0').ToUpper();
            }
        }

    }

}
