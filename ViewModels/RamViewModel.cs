using Micro.Infrastructure.Commands;
using Micro.Infrastructure.Entrys;
using Micro.Models;
using Micro.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

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
            var rawRam = mpmString.Split(['\u0002'], StringSplitOptions.RemoveEmptyEntries); // Разделение на поля по символу STX


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
            var path = _fileDialogService.SaveFile("Микропрограммная память(*.RAM) | *.RAM");
            if (path == null) return;
            var sb = new StringBuilder();
            for (var i = 0; i < _cpuState.Memory.Size; i++)
            {
                sb.Append((char)2).Append(_cpuState.Memory[i].ToString("X2"));
            }
            File.WriteAllText(path, sb.ToString());
        }

        private readonly CpuState _cpuState;
        public List<MemoryByteRow> MemoryBytes { get; set; }
        public List<MemoryWordRow> MemoryWords { get; set; }
       
        #region View modes
        public enum RamViewModes {Bytes, Words}
        private RamViewModes _ramViewMode = RamViewModes.Bytes;
        public RamViewModes RamViewMode
        {
            get => _ramViewMode;
            set => SetField(ref _ramViewMode, value);
        }
        #endregion

        public RamViewModel(CpuState cpuState)
        {
            #region Commands
            LoadRamCommand = new LambdaCommand(OnLoadRamCommandExecuted, CanLoadRamCommandExecute);
            SaveRamCommand = new LambdaCommand(OnSaveRamCommandExecuted, CanSaveRamCommandExecute);
            #endregion

            _cpuState = cpuState;
            _fileDialogService = new FileDialogService();
            MemoryBytes = new List<MemoryByteRow>();
            MemoryWords = new List<MemoryWordRow>();
            for (var i = 0; i < _cpuState.Memory.Size; i+=16)
            {
                MemoryBytes.Add(new MemoryByteRow(_cpuState.Memory, i));
                MemoryWords.Add(new MemoryWordRow(_cpuState.Memory, i));
            }
        }

        #region INotifyPropertyChanged
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
        #endregion
    }
    public class MemoryByteRow
    {
        public string Address { get; set; }  // HEX-адрес
        public List<ByteRamCellEntry> Cells { get; set; }    // 16 байтов строки

        public MemoryByteRow(RamMemory memory, int address)
        {
            Address = (address / 16).ToString("X2");
            Cells = new List<ByteRamCellEntry>(); 
            for (var i = 0; i < 16; i++) Cells.Add(new ByteRamCellEntry(memory, (ushort)(i + address)));
        }

    }
    public class MemoryWordRow
    {
        public class WordRamCellEntry : INotifyPropertyChanged
        {
            private readonly RamMemory _memory;
            private readonly ushort _address;
            public ushort Value
            {
                get => _memory.Word[_address];
                set
                {
                    _memory.Word[_address] = value;
                    OnPropertyChanged();
                }
            }

            public WordRamCellEntry(RamMemory memory, ushort address)
            {
                _memory = memory;
                _address = address;
                _memory.PropertyChanged += _memory_PropertyChanged;
            }
            private void _memory_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == $"RAM[{_address}]" || e.PropertyName == $"RAM[{_address+1}]") OnPropertyChanged(nameof(Value));
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
        public string Address { get; set; }  // HEX-адрес
        public List<WordRamCellEntry> Cells { get; set; }

        public MemoryWordRow(RamMemory memory, int address)
        {
            Address = (address / 16).ToString("X2");
            Cells = new List<WordRamCellEntry>();
            for (int i = 0; i < 16; i+=2) Cells.Add(new WordRamCellEntry(memory, (ushort)(i + address)));
        }
    }
}
