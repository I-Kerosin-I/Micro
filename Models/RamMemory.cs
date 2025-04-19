using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Models
{
    public class RamMemory : INotifyPropertyChanged
    {
        private readonly byte[] _memory;

        public int Size { get; }

        public RamMemory(int size = 1024) // Обязательно кратно 16
        {
            Size = size;
            _memory = new byte[Size];
            Word = new WordAccessor(this);
        }

        public byte this[int address]
        {
            get => _memory[address];
            set
            {
                _memory[address] = value;
                OnPropertyChanged($"RAM[{address}]");
            }
        }

        public WordAccessor Word { get; }

        public class WordAccessor
        {
            private readonly RamMemory _ram;

            public WordAccessor(RamMemory ram)
            {
                _ram = ram;
            }

            public ushort this[int address]
            {
                get
                {
                    int low = _ram._memory[address];
                    int high = _ram._memory[address + 1];
                    return (ushort)((high << 8) | low); // Little-endian
                }
                set
                {
                    _ram._memory[address] = (byte)(value & 0xFF);
                    _ram.OnPropertyChanged($"RAM[{address}]");

                    _ram._memory[address + 1] = (byte)((value >> 8) & 0xFF);
                    _ram.OnPropertyChanged($"RAM[{address + 1}]");
                }
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
