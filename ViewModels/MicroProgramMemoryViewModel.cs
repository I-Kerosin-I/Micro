using Micro.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Micro.Infrastructure.Commands;
using Micro.Infrastructure.Entrys;
using Micro.Resources;

namespace Micro.ViewModels
{
    

    internal class MicroProgramMemoryViewModel : INotifyPropertyChanged
    {
        #region Commands

        #region LoadMPM
        public ICommand LoadMpmCommand { get; set; }
        private bool CanLoadMpmCommandExecute(object p) => true;
        private void OnLoadMpmCommandExecuted(object p) => LoadMpm();
        #endregion

        #region SaveMPM
        public ICommand SaveMpmCommand { get; set; }
        private bool CanSaveMpmCommandExecute(object p) => true;
        private void OnSaveMpmCommandExecuted(object p) => SaveMpm();
        #endregion

        #endregion

        private readonly FileDialogService _fileDialogService;
        private readonly CpuState _cpuState;
        private readonly MicroProgramMemory _microProgramMemory;
        public ObservableCollection<MicroCommandEntry> MicroProgramMemory { get; set; }

        private void LoadMpm()
        {
            var mpmString = File.ReadAllText(_fileDialogService.OpenFile("Микропрограммная память(*.MEM) | *.MEM"))
                .Replace('\u0001', ' ')
                .Replace('\u0004', ' '); // Открыть, считать весь текст, заменить SEP и EOT на пробел
            string[] rawMpmFields = mpmString.Split([' '], StringSplitOptions.RemoveEmptyEntries); // Раздеоение на поля по пробелу

            if (rawMpmFields.Length % 17 != 0) MessageBox.Show("Файл микропрограммной памяти повреждён");
            else
            { //TODO: Добавить валидацию
                for (int i = 0; i < rawMpmFields.Length / 17; i++)
                {
                    _microProgramMemory[i].A     = Convert.ToByte(rawMpmFields[i * 17], 16);
                    _microProgramMemory[i].B     = Convert.ToByte(rawMpmFields[i * 17 + 1], 16);
                    _microProgramMemory[i].MA    = Convert.ToByte(rawMpmFields[i * 17 + 2], 16);
                    _microProgramMemory[i].MB    = Convert.ToByte(rawMpmFields[i * 17 + 3], 16);
                    _microProgramMemory[i].MEM   = Convert.ToByte(rawMpmFields[i * 17 + 4], 16);
                    _microProgramMemory[i].SRC   = Convert.ToByte(rawMpmFields[i * 17 + 5], 16);
                    _microProgramMemory[i].SH    = Convert.ToByte(rawMpmFields[i * 17 + 6], 16);
                    _microProgramMemory[i].N     = Convert.ToByte(rawMpmFields[i * 17 + 7], 16);
                    _microProgramMemory[i].ALU   = Convert.ToByte(rawMpmFields[i * 17 + 8], 16);
                    _microProgramMemory[i].CCX   = Convert.ToByte(rawMpmFields[i * 17 + 9], 16);
                    _microProgramMemory[i].F     = Convert.ToByte(rawMpmFields[i * 17 + 10], 16);
                    _microProgramMemory[i].DST   = Convert.ToByte(rawMpmFields[i * 17 + 11], 16);
                    _microProgramMemory[i].WM    = Convert.ToByte(rawMpmFields[i * 17 + 12], 16);
                    _microProgramMemory[i].JFI   = Convert.ToByte(rawMpmFields[i * 17 + 13], 16);
                    _microProgramMemory[i].CC    = Convert.ToByte(rawMpmFields[i * 17 + 14], 16);
                    _microProgramMemory[i].CHA   = Convert.ToByte(rawMpmFields[i * 17 + 15], 16);
                    _microProgramMemory[i].CONST = Convert.ToByte(rawMpmFields[i * 17 + 16], 16);

                }
                
            }
        }
        private void SaveMpm()
        {
            string mpmString = _fileDialogService.SaveFile("Микропрограммная память(*.MEM) | *.MEM");
        }

        
        public MicroProgramMemoryViewModel(CpuState cpuState)
        {
            #region Commands
            LoadMpmCommand = new LambdaCommand(OnLoadMpmCommandExecuted, CanLoadMpmCommandExecute);
            SaveMpmCommand = new LambdaCommand(OnSaveMpmCommandExecuted, CanSaveMpmCommandExecute);
            #endregion

            _fileDialogService = new FileDialogService();
            _cpuState = cpuState;
            _microProgramMemory = _cpuState.MicroProgramMemory;

            MicroProgramMemory = new ObservableCollection<MicroCommandEntry>();
            for (int i = 0; i < _cpuState.MicroProgramMemory.MicroCommands.Length; i++)
            {
                MicroProgramMemory.Add(new MicroCommandEntry(_cpuState.MicroProgramMemory, (byte)i));
            }
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
