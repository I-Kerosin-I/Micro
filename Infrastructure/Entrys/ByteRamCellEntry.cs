using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Micro.Models;

namespace Micro.Infrastructure.Entrys
{
    public class ByteRamCellEntry : INotifyPropertyChanged
    {
        private readonly RamMemory _memory;
        private readonly ushort _address;

        public byte Value
        {
            get => _memory[_address];
            set
            {
                _memory[_address] = value;

                OnPropertyChanged();
            }
        }

        public ByteRamCellEntry(RamMemory memory ,ushort addres)
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
