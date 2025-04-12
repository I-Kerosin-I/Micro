using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Micro.Models;

namespace Micro.Resources
{
    public class RamCellEntry : INotifyPropertyChanged
    {
        private RamMemory _memory;
        private ushort _address;

        public byte Value
        {
            get => _memory[_address];
            set
            {
                _memory[_address] = value;

                OnPropertyChanged();
            }
        }

        public RamCellEntry(RamMemory memory ,ushort addres)
        {
            _memory = memory;
            _address = addres;
            _memory.PropertyChanged += _memory_PropertyChanged;
        }

        private void _memory_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == $"RAM[{_address}]") OnPropertyChanged(nameof(Value));
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
