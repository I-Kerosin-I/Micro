using Micro.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Memory = new ObservableCollection<MemoryRow>(_cpuState.Memory);
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
    

}
