using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Micro.Resources;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Windows.Media.Media3D;


namespace Micro.Models 
{
    public class CpuState : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ushort _alu;
        public ushort Alu {
            get => _alu;
            set => SetField(ref _alu, value);
        }
        private ushort _sda;
        public ushort Sda {
            get => _sda;
            set
            {
                _sda = value;
                OnPropertyChanged(nameof(Sda));
            }
        }

        private Stack<ushort> _stack = new Stack<ushort>(); 
        public MicroProgramMemory MicroProgramMemory;
        public RegisterMemory Registers;
        public RamMemory Memory;

        public CpuState()
        {
            MicroProgramMemory = new MicroProgramMemory();
            Registers = new RegisterMemory();
            Memory = new RamMemory();
        }

        public ushort this[string register]
        {
            get => Registers[register];
            set
            {
                if (Registers[register] != value)
                {
                    Registers[register] = value;
                    OnPropertyChanged(register);
                }
            }
        }

        public void RestartCpu() //TODO: Убрать тесты ОЗУ
        {
            Registers["CMK"] = Memory[0];
            Memory[1] = 35;
            Memory.Word[6] = 0x1fda;
            Registers["RGR"] = Memory.Word[6];
            MicroProgramMemory[0].A = 1;
            

        }

        public void ExecuteMicroCommand()
        {
            var mk = MicroProgramMemory[Registers["CMK"]];
            Registers["RGA"] = Registers[mk.A];
            Registers["RGB"] = Registers[mk.B];
            switch (mk.MEM) // TODO: Реализовать взаимодействие с памятью
            {
                case 4:
                    Registers["RGR"] = 0;
                    break;
                case 5:
                    Registers["RGR"] = 0;
                    break;
                case 6:
                    Registers["RGR"] = 0;
                    break;
                case 7:
                    Registers["RGR"] = 0;
                    break;
            }
            
            ushort r, s;
            switch (mk.SRC)
            {
                case 0:
                    r = 0;
                    s = 0;
                    break;
                case 1:
                    r = Registers["RGA"];
                    s = Registers["RGB"];
                    break;
                case 2:
                    r = Registers["RGA"];
                    s = Registers["RGQ"];
                    break;
                case 3:
                    r = Registers["RGA"];
                    s = Registers["RGR"];
                    break;
                case 4:
                    r = (ushort)(Registers["RGA"] << 1);
                    s = Registers["RGB"];
                    break;
                case 5:
                    r = mk.CONST;
                    s = Registers["RGB"];
                    break;
                case 6:
                    r = mk.CONST;
                    s = Registers["RGR"];
                    break;
                case 7:
                    r = mk.CONST;
                    s = Registers["RGQ"];
                    break;
                default:
                    throw new Exception("Invalid SRC value");
            }
            // TODO: реализовать поведение C0 в зависимости от CCX
            ushort c0 = mk.CCX;
            switch (mk.ALU) //Готово, не протестировано
            {
                case 0:
                    Alu = 0;
                    break;
                case 1:
                    Alu = (ushort)(s - r - 1 + c0);
                    break;
                case 2:
                    Alu = (ushort)(r - s - 1 + c0);
                    break;
                case 3:
                    Alu = (ushort)(r + s + c0);
                    break;
                case 4:
                    Alu = (ushort)(s + c0);
                    break;
                case 5:
                    Alu = (ushort)(~s + c0);
                    break;
                case 6:
                    Alu = (ushort)(r + c0);
                    break;
                case 7:
                    Alu = (ushort)(~r + c0);
                    break;
                case 8:
                    Alu = (ushort)(r + c0); //TODO: Умножение на 2 бита
                    break;
                case 9:
                    Alu = (ushort)(r & s);
                    break;
                case 10:
                    Alu = Alu = (ushort)(r & ~s); ;
                    break;
                case 11:
                    Alu = (ushort)~(r & s); 
                    break;
                case 12:
                    Alu = (ushort)(r | s);
                    break;
                case 13:
                    Alu = (ushort)~(r | s);
                    break;
                case 14:
                    Alu = (ushort)(r ^ s);
                    break;
                case 15:
                    Alu = (ushort)~(r ^ s);
                    break;
                default:
                    throw new ArgumentException("Invalid ALU value");
            }

            Registers["RFI"] = (ushort)(Alu & 0x8000); // Формирование флага N
            Registers["RFI"] += (ushort)(Alu == 0 ? 0 : 2); // Формирование флага Z
            //TODO: Формирование остальных флагов

            switch (mk.SH) // TODO: Написать логику сдвигателей
            {
                case 0:
                    Sda = Alu;
                    break;
                case 1:
                    Sda = Alu;
                    break;
                case 2:
                    Sda = (ushort)(Alu >> mk.N);
                    break;
                case 3:
                    Sda = Alu;
                    break;
                case 4:
                    Sda = Alu;
                    break;
                case 5:
                    Sda = Alu;
                    break;
                case 6:
                    Sda = Alu;
                    Registers["RGQ"] = Alu;
                    break;
                case 8:
                    Sda = (ushort)(Alu << mk.N);
                    break;
                case 10:
                    Sda = Alu;
                    break;
                case 14:
                    Sda = Alu;
                    break;
                default:
                    throw new ArgumentException("Invalid SH value");
            }

            switch (mk.DST) //TODO: Дописать DST=2, 3
            {
                case 0:
                    break;
                case 1:
                    Registers[mk.B] = Registers["RGR"];
                    break;
                case 2:
                    Registers[mk.B] = Registers["RGR"];
                    break;
                case 3:
                    Registers[mk.B] = Registers["RGR"];
                    break;
                case 4:
                    Registers[mk.B] = Sda;
                    break;
            }

            switch (mk.WM) // Готово, протестировано
            {
                case 0:
                    break;
                case 1:
                    Registers["RGW"] = Sda;
                    break;
                case 2:
                    Registers["ARAM"] = Sda;
                    break;
                case 3:
                    Registers["ARAM"] = Registers["RGB"];
                    break;
                default:
                    throw new ArgumentException("Invalid WM value");
            }

            bool jmpCond = true;

            if ((mk.JFI & 4) == 0)
            {
                String condFlagRegister = (mk.JFI & 2) == 0 ? "RFI" : "RFD";

                switch (mk.CC) //TODO: Реализовать остальные условия, и поддержку RFD
                {
                    case 0:
                        jmpCond = (Registers[condFlagRegister] & 16) != 0; //P = 1   JP
                        break;
                    case 1:
                        jmpCond = (Registers[condFlagRegister] & 2) != 0; //Z = 1   JZ
                        break;
                    case 2:
                        jmpCond = (Registers[condFlagRegister] & 1) != 0; //N = 1   JS
                        break;
                }

                if ((mk.JFI & 1) != 0) jmpCond = !jmpCond;
            } // Формирование условия (преобразовано с учётом JFI)
            
            switch (mk.CHA)
            {
                case 0:
                    Registers["CMK"] = 0;
                    break;
                case 1:
                    _stack.Push((ushort)(mk.Address+1));
                    Registers["CMK"] = mk.CONST;
                    break;
                case 2: //TODO: Дешифрация RGK
                    break;
                case 3:
                    if (jmpCond) Registers["CMK"] = mk.CONST;
                    else Registers["CMK"]++;
                    break;
                case 4: // Если RACT != 0, декремент, переход по CONST иначе CONT
                    if (Registers["RACT"] != 0)
                    {
                        Registers["RACT"]--;
                        Registers["CMK"] = mk.CONST;
                    }
                    else Registers["CMK"]++;
                    break;
                case 5:
                    if (_stack.Count != 0)
                        Registers["CMK"] =
                            _stack.Pop(); //TODO:Проверить в оригинальной микре поведение при невыполнении условия
                    else Registers["CMK"]++;
                        break;
                case 6:
                    Registers["RACT"] = mk.CONST;
                    Registers["CMK"]++;
                    break;
                case 7:
                    Registers["CMK"]++;
                    break;
            }

            //MicroProgramMemory[0].CCX = 1; 
            
        }


        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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
