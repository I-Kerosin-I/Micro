using Micro.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Micro.Infrastructure.Commands;
using Micro.Infrastructure.Entrys;
using Micro.Resources;

namespace Micro.ViewModels
{
    

    internal class MicroProgramMemoryViewModel : INotifyPropertyChanged
    {
        #region Commands

        #region LoadMPM
        public ICommand LoadMpmCommand { get; set; }
        private bool CanLoadMpmCommandExecute(object p) => true;
        private void OnLoadMpmCommandExecuted(object p) => LoadMpm();
        #endregion

        #region SaveMPM
        public ICommand SaveMpmCommand { get; set; }
        private bool CanSaveMpmCommandExecute(object p) => true;
        private void OnSaveMpmCommandExecuted(object p) => SaveMpm();
        #endregion

        #endregion

        private readonly FileDialogService _fileDialogService;
        private readonly CpuState _cpuState;
        private readonly MicroProgramMemory _microProgramMemory;
        public ObservableCollection<MicroCommandEntry> MicroProgramMemory { get; set; }

        private void LoadMpm()
        {
            var path = _fileDialogService.OpenFile("Микропрограммная память (*.MEM, *.XMEM)|*.MEM;*.XMEM|Микропрограммная память .MEM (*.MEM)|*.MEM|Микропрограммная память .XMEM (*.XMEM)|*.XMEM");
            if (path is null) return;
            if (path.EndsWith(".MEM", StringComparison.OrdinalIgnoreCase)) //TODO: Сделать нормальную поддержку .XMEM
            {
                var mpmString = File.ReadAllText(path)
                    .Replace('\u0001', ' ')
                    .Replace('\u0004', ' '); // Открыть, считать весь текст, заменить SEP и EOT на пробел
                string[] rawMpmFields =
                    mpmString.Split([' '], StringSplitOptions.RemoveEmptyEntries); // Разделение на поля по пробелу

                if (rawMpmFields.Length % 17 != 0)
                {
                    MessageBox.Show("Файл микропрограммной памяти повреждён");
                    return;
                }
                //TODO: Добавить валидацию
                for (int i = 0; i < rawMpmFields.Length / 17; i++)
                {
                    _microProgramMemory[i].A = Convert.ToByte(rawMpmFields[i * 17], 16);
                    _microProgramMemory[i].B = Convert.ToByte(rawMpmFields[i * 17 + 1], 16);
                    _microProgramMemory[i].MA = Convert.ToByte(rawMpmFields[i * 17 + 2], 16);
                    _microProgramMemory[i].MB = Convert.ToByte(rawMpmFields[i * 17 + 3], 16);
                    _microProgramMemory[i].MEM = Convert.ToByte(rawMpmFields[i * 17 + 4], 16);
                    _microProgramMemory[i].SRC = Convert.ToByte(rawMpmFields[i * 17 + 5], 16);
                    _microProgramMemory[i].SH = Convert.ToByte(rawMpmFields[i * 17 + 6], 16);
                    _microProgramMemory[i].N = Convert.ToByte(rawMpmFields[i * 17 + 7], 16);
                    _microProgramMemory[i].ALU = Convert.ToByte(rawMpmFields[i * 17 + 8], 16);
                    _microProgramMemory[i].CCX = Convert.ToByte(rawMpmFields[i * 17 + 9], 16);
                    _microProgramMemory[i].F = Convert.ToByte(rawMpmFields[i * 17 + 10], 16);
                    _microProgramMemory[i].DST = Convert.ToByte(rawMpmFields[i * 17 + 11], 16);
                    _microProgramMemory[i].WM = Convert.ToByte(rawMpmFields[i * 17 + 12], 16);
                    _microProgramMemory[i].JFI = Convert.ToByte(rawMpmFields[i * 17 + 13], 16);
                    _microProgramMemory[i].CC = Convert.ToByte(rawMpmFields[i * 17 + 14], 16);
                    _microProgramMemory[i].CHA = Convert.ToByte(rawMpmFields[i * 17 + 15], 16);
                    _microProgramMemory[i].CONST = Convert.ToUInt16(rawMpmFields[i * 17 + 16], 16);

                }

                
            }
            else if(path.EndsWith(".XMEM", StringComparison.OrdinalIgnoreCase))
            {
                var mpmString = File.ReadAllText(path);
                string[] rawMpmFields = mpmString.Split(['\u0001','\n']); // Разделение на поля по SEP

                //if (rawMpmFields.Length % 18 != 0) MessageBox.Show("Файл микропрограммной памяти повреждён");
                //else
                {
                    //TODO: Добавить валидацию
                    for (int i = 0; i < rawMpmFields.Length / 18; i++)
                    {
                        _microProgramMemory[i].A = Convert.ToByte(rawMpmFields[i * 18], 16);
                        _microProgramMemory[i].B = Convert.ToByte(rawMpmFields[i * 18 + 1], 16);
                        _microProgramMemory[i].MA = Convert.ToByte(rawMpmFields[i * 18 + 2], 16);
                        _microProgramMemory[i].MB = Convert.ToByte(rawMpmFields[i * 18 + 3], 16);
                        _microProgramMemory[i].MEM = Convert.ToByte(rawMpmFields[i * 18 + 4], 16);
                        _microProgramMemory[i].SRC = Convert.ToByte(rawMpmFields[i * 18 + 5], 16);
                        _microProgramMemory[i].SH = Convert.ToByte(rawMpmFields[i * 18 + 6], 16);
                        _microProgramMemory[i].N = Convert.ToByte(rawMpmFields[i * 18 + 7], 16);
                        _microProgramMemory[i].ALU = Convert.ToByte(rawMpmFields[i * 18 + 8], 16);
                        _microProgramMemory[i].CCX = Convert.ToByte(rawMpmFields[i * 18 + 9], 16);
                        _microProgramMemory[i].F = Convert.ToByte(rawMpmFields[i * 18 + 10], 16);
                        _microProgramMemory[i].DST = Convert.ToByte(rawMpmFields[i * 18 + 11], 16);
                        _microProgramMemory[i].WM = Convert.ToByte(rawMpmFields[i * 18 + 12], 16);
                        _microProgramMemory[i].JFI = Convert.ToByte(rawMpmFields[i * 18 + 13], 16);
                        _microProgramMemory[i].CC = Convert.ToByte(rawMpmFields[i * 18 + 14], 16);
                        _microProgramMemory[i].CHA = Convert.ToByte(rawMpmFields[i * 18 + 15], 16);
                        _microProgramMemory[i].CONST = Convert.ToUInt16(rawMpmFields[i * 18 + 16], 16);
                        _microProgramMemory[i].Comment = rawMpmFields[i * 18 + 17];

                    }

                }
            }

        }
        private void SaveMpm()
        {
            var path = _fileDialogService.SaveFile("Микропрограммная память (*.MEM, *.XMEM)|*.MEM;*.XMEM|Микропрограммная память .MEM (*.MEM)|*.MEM|Микропрограммная память .XMEM (*.XMEM)|*.XMEM");
            if (path is null) return;
            
            if (path.EndsWith(".MEM", StringComparison.OrdinalIgnoreCase))
            {
                var sb = new StringBuilder();
                for (var i = 0; i < 64; i++) // TODO: заменить на _microProgramMemory.Length
                {
                    sb.Append((char)1).Append(_microProgramMemory[i].A.ToString("X1"));
                    sb.Append((char)1).Append(_microProgramMemory[i].B.ToString("X1"));
                    sb.Append((char)1).Append(_microProgramMemory[i].MA.ToString("X1"));
                    sb.Append((char)1).Append(_microProgramMemory[i].MB.ToString("X1"));
                    sb.Append((char)1).Append(_microProgramMemory[i].MEM.ToString("X1"));
                    sb.Append((char)1).Append(_microProgramMemory[i].SRC.ToString("X1"));
                    sb.Append((char)1).Append(_microProgramMemory[i].SH.ToString("X1"));
                    sb.Append((char)1).Append(_microProgramMemory[i].N.ToString("X1"));
                    sb.Append((char)1).Append(_microProgramMemory[i].ALU.ToString("X1"));
                    sb.Append((char)1).Append(_microProgramMemory[i].CCX.ToString("X1"));
                    sb.Append((char)1).Append(_microProgramMemory[i].F.ToString("X1"));
                    sb.Append((char)1).Append(_microProgramMemory[i].DST.ToString("X1"));
                    sb.Append((char)1).Append(_microProgramMemory[i].WM.ToString("X1"));
                    sb.Append((char)1).Append(_microProgramMemory[i].JFI.ToString("X1"));
                    sb.Append((char)1).Append(_microProgramMemory[i].CC.ToString("X1"));
                    sb.Append((char)1).Append(_microProgramMemory[i].CHA.ToString("X1"));
                    sb.Append((char)4).Append(_microProgramMemory[i].CONST.ToString("X4"));
                }

                File.WriteAllText(path, sb.ToString());
            }
            else if (path.EndsWith(".XMEM", StringComparison.OrdinalIgnoreCase))
            {
                var sb = new StringBuilder();
                for (var i = 0; i < 64; i++) // TODO: заменить на _microProgramMemory.Length
                {
                    sb.Append(_microProgramMemory[i].A.ToString("X1")).Append((char)1);
                    sb.Append(_microProgramMemory[i].B.ToString("X1")).Append((char)1);
                    sb.Append(_microProgramMemory[i].MA.ToString("X1")).Append((char)1);
                    sb.Append(_microProgramMemory[i].MB.ToString("X1")).Append((char)1);
                    sb.Append(_microProgramMemory[i].MEM.ToString("X1")).Append((char)1);
                    sb.Append(_microProgramMemory[i].SRC.ToString("X1")).Append((char)1);
                    sb.Append(_microProgramMemory[i].SH.ToString("X1")).Append((char)1);
                    sb.Append(_microProgramMemory[i].N.ToString("X1")).Append((char)1);
                    sb.Append(_microProgramMemory[i].ALU.ToString("X1")).Append((char)1);
                    sb.Append(_microProgramMemory[i].CCX.ToString("X1")).Append((char)1);
                    sb.Append(_microProgramMemory[i].F.ToString("X1")).Append((char)1);
                    sb.Append(_microProgramMemory[i].DST.ToString("X1")).Append((char)1);
                    sb.Append(_microProgramMemory[i].WM.ToString("X1")).Append((char)1);
                    sb.Append(_microProgramMemory[i].JFI.ToString("X1")).Append((char)1);
                    sb.Append(_microProgramMemory[i].CC.ToString("X1")).Append((char)1);
                    sb.Append(_microProgramMemory[i].CHA.ToString("X1")).Append((char)1);
                    sb.Append(_microProgramMemory[i].CONST.ToString("X4")).Append((char)1);
                    sb.Append(_microProgramMemory[i].Comment).Append('\n');
                }

                File.WriteAllText(path, sb.ToString());
            }
        }

        
        public MicroProgramMemoryViewModel(CpuState cpuState)
        {
            #region Commands
            LoadMpmCommand = new LambdaCommand(OnLoadMpmCommandExecuted, CanLoadMpmCommandExecute);
            SaveMpmCommand = new LambdaCommand(OnSaveMpmCommandExecuted, CanSaveMpmCommandExecute);
            #endregion

            _fileDialogService = new FileDialogService();
            _cpuState = cpuState;
            _microProgramMemory = _cpuState.MicroProgramMemory;

            MicroProgramMemory = new ObservableCollection<MicroCommandEntry>();
            for (int i = 0; i < _cpuState.MicroProgramMemory.MicroCommands.Length; i++)
            {
                MicroProgramMemory.Add(new MicroCommandEntry(_cpuState.MicroProgramMemory, (byte)i));
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
