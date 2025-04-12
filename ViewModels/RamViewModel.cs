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

namespace Micro.ViewModels
{
    class RamViewModel
    {
        #region Commands



        #endregion
        private readonly CpuState _cpuState;
        public ObservableCollection<MemoryRow> Memory { get; set; }
        private bool _isWordMode;   // Режим: Байты или Слова

       

        public bool IsWordMode
        {
            get => _isWordMode;
            set
            {
                _isWordMode = value;
                OnPropertyChanged(nameof(IsWordMode));
                LoadMemory(); // Перезагружаем данные
            }
        }

        public RamViewModel(CpuState cpuState)
        {
            _cpuState = cpuState;
            Memory = new ObservableCollection<MemoryRow>();

            for (int i = 0; i < _cpuState.Memory.Size; i+=16)
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
    }
    public class MemoryRow
    {
        public string Address { get; set; }  // HEX-адрес
        public ObservableCollection<RamCellEntry> Cells { get; set; }    // 16 байтов строки

        public MemoryRow(RamMemory memory, int address)
        {
            Address = (address / 16).ToString("X2");
            Cells = new ObservableCollection<RamCellEntry>(); // Заполняем нулями по умолчанию
            for (int i = 0; i < 16; i++) Cells.Add(new RamCellEntry(memory, (ushort)(i + address)));
        }
    }
}
