using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Micro.Models;

namespace Micro.Infrastructure.Entrys
{
    public class MicroCommandEntry : INotifyPropertyChanged
    {

        private MicroProgramMemory _mpm;
        private byte _address;
        public MicroCommand MicroCommand
        {
            get => _mpm[_address];
            set
            {
                _mpm[_address] = value;
                OnPropertyChanged();
            }
        }

        public MicroCommandEntry(MicroProgramMemory Mpm, byte address)
        {
            _mpm = Mpm;
            _address = address;
            _mpm.PropertyChanged += _mpm_PropertyChanged;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void _mpm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == $"MPM[{_address}]") OnPropertyChanged(nameof(MicroCommand));
        }
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
