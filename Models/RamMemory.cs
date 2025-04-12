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

     /*   public ushort ReadWord(int address)
        {
            return (ushort)(_memory[address] | (_memory[address + 1] << 8));
        }

        public void WriteWord(int address, ushort value)
        {
            _memory[address] = (byte)(value & 0xFF);
            _memory[address + 1] = (byte)((value >> 8) & 0xFF);
        }*/

        //public Span<byte> GetSpan(int start, int length) => new Span<byte>(_memory, start, length);
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
