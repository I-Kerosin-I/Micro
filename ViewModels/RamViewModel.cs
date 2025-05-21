using Micro.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Micro.Resources;
using System.Runtime.CompilerServices;
using Micro.Infrastructure.Entrys;
using System.Windows;
using System.IO;
using System.Windows.Input;
using Micro.Infrastructure.Commands;

namespace Micro.ViewModels
{
    class RamViewModel : INotifyPropertyChanged
    {
        #region Commands

        #region LoadRAM
        public ICommand LoadRamCommand { get; set; }
        private bool CanLoadRamCommandExecute(object p) => true;
        private void OnLoadRamCommandExecuted(object p) => LoadRam();
        #endregion

        #region SaveRAM
        public ICommand SaveRamCommand { get; set; }
        private bool CanSaveRamCommandExecute(object p) => true;
        private void OnSaveRamCommandExecuted(object p) => SaveRam();
        #endregion

        #endregion

        private readonly FileDialogService _fileDialogService;

        private void LoadRam()
        {
            var path = _fileDialogService.OpenFile("Оперативная память(*.RAM) | *.RAM");
            if (path == null) return;
            var mpmString = File.ReadAllText(path);
            string[] rawRam = mpmString.Split(['\u0002'], StringSplitOptions.RemoveEmptyEntries); // Раздеоение на поля по символу STX


            { //TODO: Добавить валидацию RAM
                for (var i = 0; i < rawRam.Length; i++)
                {
                    var a = Convert.ToByte(rawRam[i], 16);
                    _cpuState.Memory[i] = a;
                }

            }
        }
        private void SaveRam()
        {
            string ramString = _fileDialogService.SaveFile("Микропрограммная память(*.RAM) | *.RAM");
        }

        private readonly CpuState _cpuState;
        public ObservableCollection<MemoryRow> Memory { get; set; }
        public enum RamViewModes {Byte, Word}
        private RamViewModes _ramViewMode = RamViewModes.Byte;
        public RamViewModes RamViewMode
        {
            get => _ramViewMode;
            set => SetField(ref _ramViewMode, value);
        }

        public RamViewModel(CpuState cpuState)
        {
            LoadRamCommand = new LambdaCommand(OnLoadRamCommandExecuted, CanLoadRamCommandExecute);
            SaveRamCommand = new LambdaCommand(OnSaveRamCommandExecuted, CanSaveRamCommandExecute);

            _cpuState = cpuState;
            _fileDialogService = new FileDialogService();
            Memory = new ObservableCollection<MemoryRow>();

            for (var i = 0; i < _cpuState.Memory.Size; i+=16)
            {
                Memory.Add(new MemoryRow(_cpuState.Memory, i));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
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
    public class MemoryRow
    {
        public string Address { get; set; }  // HEX-адрес
        public ObservableCollection<ByteRamCellEntry> Cells { get; set; }    // 16 байтов строки

        public MemoryRow(RamMemory memory, int address)
        {
            Address = (address / 16).ToString("X2");
            Cells = new ObservableCollection<ByteRamCellEntry>(); // Заполняем нулями по умолчанию
            for (int i = 0; i < 16; i++) Cells.Add(new ByteRamCellEntry(memory, (ushort)(i + address)));
        }
    }
}
