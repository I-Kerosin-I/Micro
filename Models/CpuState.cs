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
using System.Windows;
using System.Windows.Media.Media3D;


namespace Micro.Models 
{
    public class CpuState : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region CPU component fields
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

        public MicroProgramMemory MicroProgramMemory;
        public RegisterMemory Registers;
        public RamMemory Memory;
        public AddressTranslationTable AddressTranslationTable;
        private Stack<ushort> _stack = new(); 
        #endregion


        private bool _autoMode = true;
        public List<List<ushort>> Trace = [];


        #region Constructor
        public CpuState()
        {
            MicroProgramMemory = new MicroProgramMemory();
            Registers = new RegisterMemory();
            Memory = new RamMemory();
            AddressTranslationTable = new AddressTranslationTable();
        }
        #endregion

        public void RestartCpu() 
        {
            Registers["CMK"] = 0;
            Registers["RFI"] = 0;
            Registers["RFD"] = 0;
            Registers["RGA"] = 0;
            Registers["RGB"] = 0;
            Alu = 0;
            Sda = 0;
            Trace.Clear();
            Trace.Add(
            [
                Registers["CMK"], Registers["AX"], Registers["CX"], Registers["DX"], Registers["BX"],
                Registers["SP"], Registers["BP"], Registers["SI"], Registers["DI"], Registers["CS"],
                Registers["SS"], Registers["DS"], Registers["ES"], Registers["IP"], Registers["PSW"],
                Registers["RGK"], Registers["RW"], Registers["RGA"], Registers["RGB"], Alu, Sda,
                Registers["RGQ"], Registers["RFI"], Registers["ARAM"], Registers["RGR"],
                Registers["RGW"], Registers["RACT"]
            ]);


        }

        public void ExecuteMicroCommand()
        {
            //while(_autoMode)
            {
                var mk = MicroProgramMemory[Registers["CMK"]];
                

                Registers["RGA"] = mk.MA switch
                {
                    0 => Registers[mk.A],
                    1 => Registers[(Registers["RGK"] & 0b0000011100000000) >> 8],
                    2 => Registers[(Registers["RGK"] & 0b0000000000111000) >> 3],
                    3 => Registers[(Registers["RGK"] & 0b0000000000000111)],
                    _ => Registers["RGA"]
                };
                byte dst = mk.MB switch
                {
                    1 => (byte)((Registers["RGK"] & 0b0000011100000000) >> 8),
                    2 => (byte)((Registers["RGK"] & 0b0000000000111000) >> 3),
                    3 => (byte)(Registers["RGK"] & 0b0000011100000000),
                    _ => mk.B
                };
                Registers["RGB"] = Registers[dst];

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

                byte c0 = mk.CCX;
                if ((mk.CCX & 0b10) == 2) c0 = (byte)(Registers["RFI"] & 1);

                UInt32 _alu32 = mk.ALU switch
                {
                    0 => 0,
                    1 => (uint)(s - r - 1 + c0),
                    2 => (uint)(r - s - 1 + c0),
                    3 => (uint)(r + s + c0),
                    4 => (uint)(s + c0),
                    5 => (uint)(~s + c0),
                    6 => (uint)(r + c0),
                    7 => (uint)(~r + c0),
                    8 => s,
                    9 => (uint)(r & s),
                    10 => (uint)(r & ~s),
                    11 => (uint)~(r & s),
                    12 => (uint)(r | s),
                    13 => (uint)~(r | s),
                    14 => (uint)(r ^ s),
                    15 => (uint)~(r ^ s),
                    _ => throw new ArgumentException("Invalid ALU value")
                };

                Alu = (ushort)(_alu32 & 0x0000FFFF);

                #region Формирование флагов

                Registers["RFI"] = (ushort)((Alu & 0x8000) >> 8); // Формирование флага N (S)  - 7 бит RFI
                Registers["RFI"] |= (ushort)(Alu == 0 ? 0x40 : 0); // Формирование флага Z  - 6 бит RFI
                Registers["RFI"] |= (ushort)((_alu32 & 0x10000) >> 16); // Формирование флага C  - 0 бит RFI
                Registers["RFI"] |=
                    (ushort)(((Registers["RFI"] << 4) ^ (Alu >> 4)) & 0x800); // Формирование флага V  - 11 бит RFI
                /*byte p = 0;
                for (byte i = 0; i < 8; i++) p ^= (byte)((Alu >> i) & 1); // Формирование флага P  - 2 бит RFI*/
                ushort p = 0;
                for (ushort i = 0; i < 16; i++)
                    p ^= (ushort)((Alu >> i) & 1); // Формирование флага P (Как в микре)  - 2 бит RFI
                Registers["RFI"] |= (ushort)(p << 2);

                if (mk.F == 1) Registers["RFD"] = Registers["RFI"];

                #endregion

                //TODO: Узнать про формирование Pf и Vf

                switch (mk.SH)
                {
                    case 0: // Без сдвига
                        Sda = Alu;
                        break;
                    case 1: // АС АЛУ вправо
                        Sda = (ushort)((short)Alu >> mk.N);
                        break;
                    case 2: // ЛС АЛУ вправо
                        Sda = (ushort)(Alu >>> mk.N);
                        break;
                    case 3: // АС АЛУ, RGQ вправо
                        Sda = (ushort)((short)Alu >> mk.N);
                        Registers["RGQ"] = (ushort)((Registers["RGQ"] >>> mk.N) | Alu << (16 - mk.N));
                        break;
                    case 4: // ЛС АЛУ, RGQ вправо
                        Sda = (ushort)(Alu >>> mk.N);
                        Registers["RGQ"] = (ushort)((Registers["RGQ"] >>> mk.N) | Alu << (16 - mk.N));
                        break;
                    case 5: // TODO: ЛС RGQ вправо (в оригинальной микре не работает)
                        Sda = Alu;
                        break;
                    case 6: // RGQ <= ALU
                        Sda = Alu;
                        Registers["RGQ"] = Alu;
                        break;
                    case 8: // ЛС АЛУ влево
                        Sda = (ushort)(Alu << mk.N);
                        break;
                    case 10: // ЛС АЛУ, RGQ влево
                        Sda = (ushort)((Alu << mk.N) | Registers["RGQ"] >> (16 - mk.N));
                        Registers["RGQ"] <<= mk.N;
                        break;
                    case 14: 
                        if ((Alu & 0x80) == 0x80)
                        {
                            Sda = (ushort)(Alu | 0xff00);
                        }
                        else
                        {
                            Sda = (ushort)(Alu & 0x00FF);
                        }
                        break;
                    default:
                        throw new ArgumentException("Invalid SH value");
                }


                switch (mk.WM)
                {
                    case 1:
                        Registers["RGW"] = Sda;
                        break;
                    case 2:
                        Registers["ARAM"] = Sda;
                        break;
                    case 3:
                        Registers["ARAM"] = Registers["RGB"];
                        break;
                } 

                switch (mk.MEM)
                {
                    case 4:
                        Registers["RGR"] = Memory[Registers["ARAM"]];
                        break;
                    case 5:
                        Registers["RGR"] = Memory.Word[Registers["ARAM"]];
                        break;
                    case 6:
                        Memory[Registers["ARAM"]] = (byte)(Registers["RGW"] & 0xff);
                        break;
                    case 7:
                        Memory.Word[Registers["ARAM"]] = Registers["RGW"];
                        break;
                }

                Registers[dst] = mk.DST switch
                {
                    1 => Registers["RGR"],
                    2 => (ushort)((Registers[dst] & 0x00ff) | (Registers["RGR"] << 8)),
                    3 => (ushort)((Registers[dst] & 0xff00) | (Registers["RGR"] >>> 8)),
                    4 => Sda,
                    _ => Registers[dst]
                };

                var jmpCond = true;

                if ((mk.JFI & 4) == 0)
                {
                    String condFlagRegister = (mk.JFI & 2) == 0 ? "RFI" : "RFD";

                    jmpCond = mk.CC switch //TODO: Реализовать остальные условия
                    {
                        0 => (Registers[condFlagRegister] & 4) != 0, //P = 1   JP
                        1 => (Registers[condFlagRegister] & 64) != 0, //Z = 1   JZ
                        2 => (Registers[condFlagRegister] & 128) != 0, //N = 1   JS
                        _ => jmpCond
                    };

                    if ((mk.JFI & 1) != 0) jmpCond = !jmpCond;
                } // Формирование условия (преобразовано с учётом JFI)

                switch (mk.CHA) // TODO: Обработка некорректного ввода адреса перехода
                {
                    case 0:
                        Registers["CMK"] = 0;
                        break;
                    case 1:
                        if (jmpCond)
                        {
                            _stack.Push((ushort)(mk.Address + 1));
                            Registers["CMK"] = mk.CONST;
                        }
                        else Registers["CMK"]++;

                        break;
                    case 2:
                        var addressFound = false;
                        foreach (var row in AddressTranslationTable.Table)
                        {
                            var mask = Convert.ToUInt16(row.Opcode.Replace('0', '1').Replace('X', '0'), 2);
                            var result = Convert.ToUInt16(row.Opcode.Replace('X', '0'), 2);
                            if ((Registers["RGK"] & mask) == result)
                            {
                                Registers["CMK"] = row.Address;
                                addressFound = true;
                                break;
                            }
                        }

                        if (!addressFound)
                        {
                            MessageBox.Show("Не удалось дешифровать команду. В таблице преобразования нет указанного кода операции.");
                            Registers["CMK"] = 0;
                            return; // TODO: Полноценная остановка процессора
                        }
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
                        if (jmpCond)
                        {
                            if (_stack.Count != 0)
                                Registers["CMK"] =
                                    _stack.Pop();
                            else
                            {
                                // TODO: выдавать ошибку "Пустой стэк"
                                Registers["CMK"]++;
                            }
                        }
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
                
                Trace.Add(
                [
                    Registers["CMK"], Registers["AX"], Registers["CX"], Registers["DX"], Registers["BX"],
                    Registers["SP"], Registers["BP"], Registers["SI"], Registers["DI"], Registers["CS"],
                    Registers["SS"], Registers["DS"], Registers["ES"], Registers["IP"], Registers["PSW"],
                    Registers["RGK"], Registers["RW"], Registers["RGA"], Registers["RGB"], Alu, Sda,
                    Registers["RGQ"], Registers["RFI"], Registers["ARAM"], Registers["RGR"],
                    Registers["RGW"], Registers["RACT"]
                ]);
                
                if (mk.JFI == 5 || Registers["CMK"] > 0x3F) // TODO: Полноценная остановка процессора
                {
                    _autoMode = false;
                    Registers["CMK"] = 0;
                }


            }
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
