using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Models
{
    public class MicroProgramMemory : INotifyPropertyChanged
    {
        public MicroCommand[] MicroCommands { get; set; }


        public MicroProgramMemory()
        {
            MicroCommands = new MicroCommand[64];
            for (byte i = 0; i < 64; i++) MicroCommands[i] = (new MicroCommand(i));
        }

        public MicroCommand this[int index]
        {
            get => MicroCommands[index];
            set
            {
                MicroCommands[index] = value;
                OnPropertyChanged($"MPM[{index}]");
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

    public class MicroCommand : INotifyPropertyChanged
    {
        private byte _address;
        private byte _a;
        private byte _b;
        private byte _ma;
        private byte _mb;
        private byte _mem;
        private byte _src;
        private byte _sh;
        private byte _n;
        private byte _alu;
        private byte _ccx;
        private byte _f;
        private byte _dst;
        private byte _wm;
        private byte _jfi;
        private byte _cc;
        private byte _cha;
        private ushort _const;

        public byte Address
        {
            get => _address;
            set => SetField(ref _address, value);
        }

        public byte A
        {
            get => _a;
            set => SetField(ref _a, value);
        }

        public byte B
        {
            get => _b;
            set => SetField(ref _b, value);
        }

        public byte MA
        {
            get => _ma;
            set => SetField(ref _ma, value);
        }

        public byte MB
        {
            set => SetField(ref _mb, value);
            get => _mb;
        }

        public byte MEM
        {
            get => _mem;
            set => SetField(ref _mem, value);
        }

        public byte SRC
        {
            get => _src;
            set => SetField(ref _src, value);
        }

        public byte SH
        {
            get => _sh;
            set => SetField(ref _sh, value);
        }

        public byte N
        {
            get => _n;
            set => SetField(ref _n, value);
        }

        public byte ALU
        {
            get => _alu;
            set => SetField(ref _alu, value);
        }

        public byte CCX
        {
            get => _ccx;
            set => SetField(ref _ccx, value);
        }

        public byte F
        {
            get => _f;
            set => SetField(ref _f, value);
        }

        public byte DST
        {
            get => _dst;
            set => SetField(ref _dst, value);
        }

        public byte WM
        {
            get => _wm;
            set => SetField(ref _wm, value);
        }

        public byte JFI
        {
            get => _jfi;
            set => SetField(ref _jfi, value);
        }

        public byte CC
        {
            get => _cc;
            set => SetField(ref _cc, value);
        }

        public byte CHA
        {
            get => _cha;
            set => SetField(ref _cha, value);
        }

        public ushort CONST
        {
            get => _const;
            set => SetField(ref _const, value);
        }

        public MicroCommand(byte Addr)
        {
            Address = Addr;
            A = 0;
            B = 0;
            MA = 0;
            MB = 0;
            MEM = 0;
            SRC = 1;
            SH = 0;
            N = 0;
            ALU = 6;
            CCX = 0;
            F = 0;
            DST = 0;
            WM = 0;
            JFI = 0;
            CC = 0;
            CHA = 7;
            CONST = 0;
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
