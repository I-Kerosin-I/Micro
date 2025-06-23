using Micro.Infrastructure.Commands;
using Micro.Infrastructure.Entrys;
using Micro.Models;
using Micro.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Micro.ViewModels
{
    
    class AddressTranslationViewModel : INotifyPropertyChanged
    {
        #region Commands

        #region LoadMPM
        public ICommand LoadAddressTranslationTableCommand { get; set; }
        private bool CanLoadAddressTranslationTableCommandExecute(object p) => true;
        private void OnLoadAddressTranslationTableCommandExecuted(object p) => LoadAddressTranslationTable();
        #endregion

        #region SaveMPM
        public ICommand SaveAddressTranslationTableCommand { get; set; }
        private bool CanSaveAddressTranslationTableCommandExecute(object p) => true;
        private void OnSaveAddressTranslationTableCommandExecuted(object p) => SaveAddressTranslationTable();
        #endregion

        #endregion

        private readonly FileDialogService _fileDialogService;
        private readonly CpuState _cpuState;
        public AddressTranslationTable AddressTranslationTable { get; set; }
        
        void LoadAddressTranslationTable()
        {
            var path = _fileDialogService.OpenFile("Таблица преобразования адресов (*.TTA)|*.TTA");
            if (path is null) return;
            var tableString = File.ReadAllText(path); // Читаем файл

            if (tableString.Length != 480) // Стандартная длина TTA файла - 480 символов. TODO: Добавить валидацию
            {
                MessageBox.Show("Файл микропрограммной памяти повреждён");
                return;
            }
            for (var i = 0; i < 20; i++)
            {
                AddressTranslationTable.Table[i].Address = Convert.ToByte(tableString.Substring(i*24 + 1, 2), 16); // Выделение подстроки с 1 по 2 символ для каждой строки таблицы
                AddressTranslationTable.Table[i].Opcode = tableString.Substring(i*24 + 4, 16); // Выделение подстроки с 4 по 20 символ для каждой строки таблицы
            }

            OnPropertyChanged($"{nameof(AddressTranslationTable)}.Table"); // Обновляем весь DataGrid, привязанный к таблице
        }

        void SaveAddressTranslationTable()
        {
            var path = _fileDialogService.SaveFile("Таблица преобразования адресов (*.TTA)|*.TTA");
            if (path is null) return;
            var sb = new StringBuilder();
            foreach (var row in AddressTranslationTable.Table)
            {
                sb.Append((char)2).Append(row.Address.ToString("X2")).Append((char)16).Append(row.Opcode).Append("MaxA"); // STX_адрес_DLE_опкод_4 рандомных символа
            }
            File.WriteAllText(path, sb.ToString());
        }

        public AddressTranslationViewModel(CpuState cpuState)
        {
            _cpuState = cpuState;
            _fileDialogService = new FileDialogService();
            AddressTranslationTable = cpuState.AddressTranslationTable;
            #region Commands
            LoadAddressTranslationTableCommand = new LambdaCommand(OnLoadAddressTranslationTableCommandExecuted, CanLoadAddressTranslationTableCommandExecute);
            SaveAddressTranslationTableCommand = new LambdaCommand(OnSaveAddressTranslationTableCommandExecuted, CanSaveAddressTranslationTableCommandExecute);
            #endregion
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
