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

namespace Micro.ViewModels
{
    class RamViewModel : INotifyPropertyChanged
    {
        #region Commands



        #endregion
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
            _cpuState = cpuState;
            Memory = new ObservableCollection<MemoryRow>();

            for (var i = 0; i < _cpuState.Memory.Size; i+=16)
            {
                Memory.Add(new MemoryRow(_cpuState.Memory, i));
            }
        }

        private void LoadMemory()
        {

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
