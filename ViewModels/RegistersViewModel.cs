using Micro.Infrastructure.Commands;
using Micro.Infrastructure.Entrys;
using Micro.Models;
using Micro.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Micro.ViewModels
{
    internal class RegistersViewModel
    {
        #region Commands

        #region LoadRegisters
        public ICommand LoadRegistersCommand { get; set; }
        private bool CanLoadRegistersCommandExecute(object p) => true;
        private void OnLoadRegistersCommandExecuted(object p) => LoadRegisters();
        #endregion

        #region SaveRegisters
        public ICommand SaveRegistersCommand { get; set; }
        private bool CanSaveRegistersCommandExecute(object p) => true;
        private void OnSaveRegistersCommandExecuted(object p) => SaveRegisters();
        #endregion

        #region ResetRegisters
        public ICommand ResetRegistersCommand { get; set; }
        private bool CanResetRegistersCommandExecute(object p) => true;
        private void OnResetRegistersCommandExecuted(object p) => ResetRegisters();
        #endregion

        #endregion


        private readonly FileDialogService _fileDialogService;
        private void LoadRegisters()
        {
            var path = _fileDialogService.OpenFile("Значения регистров (*.RG)|*.RG");
            if (path is null) return;
            var registersString = File.ReadAllText(path); // Читаем файл
            if (registersString.Length != 154) // TODO: Вот это позорище при загрузке регистров на норм валидацию поменять
            {
                MessageBox.Show("Файл значений регистров повреждён!", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            _cpuState.Registers["AX"] = ushort.Parse(registersString.Substring(1,4), NumberStyles.AllowHexSpecifier);
            _cpuState.Registers["BX"] = ushort.Parse(registersString.Substring(6,4), NumberStyles.AllowHexSpecifier);
            _cpuState.Registers["CX"] = ushort.Parse(registersString.Substring(11,4), NumberStyles.AllowHexSpecifier);
            _cpuState.Registers["DX"] = ushort.Parse(registersString.Substring(16, 4), NumberStyles.AllowHexSpecifier);

            for (var i = 4; i < 16; i++) // Заполняем РОНы
            {
                _cpuState.Registers[i] = ushort.Parse(registersString.Substring(i*5 + 1, 4), NumberStyles.AllowHexSpecifier);
            }
            _cpuState.Registers["RFI"] = 0;
            _cpuState.Registers["RFD"] = 0;
            /*
            ВНИМАНИЕ, МИКРОПИЗДОС: сохранение флагов в файл не обратно совместимо с классической микрой,
            т.к. здесь расположение флагов в регистре соответствует реальной x86,
            нужно оно почти никогда, поэтому фиксить не буду.
            Загрузка RFI и RFD отключена во избежание непредсказуемого поведения
            */
            _cpuState.Registers["RGQ"] = ushort.Parse(registersString.Substring(87,4), NumberStyles.AllowHexSpecifier);
            _cpuState.Registers["CMK"] = ushort.Parse(registersString.Substring(92,4), NumberStyles.AllowHexSpecifier);
            _cpuState.Registers["MUAD"] = ushort.Parse(registersString.Substring(97,4), NumberStyles.AllowHexSpecifier);
            _cpuState.Registers["STP"] = ushort.Parse(registersString.Substring(102,4), NumberStyles.AllowHexSpecifier);
            _cpuState.Registers["ERROR"] = ushort.Parse(registersString.Substring(107,4), NumberStyles.AllowHexSpecifier);
            _cpuState.Registers["RACT"] = ushort.Parse(registersString.Substring(112,4), NumberStyles.AllowHexSpecifier);
            _cpuState.Registers["ARAM"] = ushort.Parse(registersString.Substring(120,4), NumberStyles.AllowHexSpecifier);
            _cpuState.Registers["RGR"] = ushort.Parse(registersString.Substring(125,4), NumberStyles.AllowHexSpecifier);
            _cpuState.Registers["RGW"] = ushort.Parse(registersString.Substring(130,4), NumberStyles.AllowHexSpecifier);


        }

        private void SaveRegisters()
        {
            var path = _fileDialogService.SaveFile("Значения регистров (*.RG)|*.RG");
            if (path is null) return;
            var sb = new StringBuilder();
            sb
                .Append((char)4).Append(_cpuState.Registers["AX"].ToString("X4"))
                .Append((char)4).Append(_cpuState.Registers["BX"].ToString("X4"))
                .Append((char)4).Append(_cpuState.Registers["CX"].ToString("X4"))
                .Append((char)4).Append(_cpuState.Registers["DX"].ToString("X4"))
                ;

            for (int i = 4; i < 16; i++) // РОНы
            {
                sb.Append((char)4).Append(_cpuState.Registers[i].ToString("X4")); // EOT + значение
            }
            /*
            ВНИМАНИЕ, МИКРОПИЗДОС: сохранение флагов в файл не обратно совместимо с классической микрой,
            т.к. здесь расположение флагов в регистре соответствует реальной x86,
            нужно оно почти никогда, поэтому фиксить не буду.
            Сохранение RFI и RFD отключено во избежание непредсказуемого поведения
            */
            sb
                .Append((char)2).Append("00")
                .Append((char)2).Append("00")
                .Append((char)4).Append(_cpuState.Registers["RGQ"].ToString("X4"))
                .Append((char)4).Append(_cpuState.Registers["CMK"].ToString("X4"))
                .Append((char)4).Append(_cpuState.Registers["MUAD"].ToString("X4"))
                .Append((char)4).Append(_cpuState.Registers["STP"].ToString("X4"))
                .Append((char)4).Append(_cpuState.Registers["ERROR"].ToString("X4"))
                .Append((char)4).Append(_cpuState.Registers["RACT"].ToString("X4"))
                .Append("\0\x1\x1") // TODO: добавить сохранение режима работы
                .Append((char)4).Append(_cpuState.Registers["ARAM"].ToString("X4"))
                .Append((char)4).Append(_cpuState.Registers["RGR"].ToString("X4"))
                .Append((char)4).Append(_cpuState.Registers["RGW"].ToString("X4"))
                .Append("\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0")
                ;
            File.WriteAllText(path, sb.ToString());
        }

        private void ResetRegisters()
        {
            for (var i = 0; i < 16; i++)
            {
                _cpuState.Registers[i] = 0;
            }
            _cpuState.Registers["RFI"] = 0;
            _cpuState.Registers["RFD"] = 0;
            _cpuState.Registers["RGQ"] = 0;
            _cpuState.Registers["ARAM"] = 0;
            _cpuState.Registers["RGR"] = 0;
            _cpuState.Registers["RGW"] = 0;
            _cpuState.Registers["RACT"] = 0;
            _cpuState.Registers["CMK"] = 0;
            _cpuState.Registers["MUAD"] = 0;
            _cpuState.Registers["STP"] = 0;
            _cpuState.Registers["ERROR"] = 0;
            _cpuState.Registers["RGA"] = 0;
            _cpuState.Registers["RGB"] = 0;
        }


        private readonly CpuState _cpuState;
        public ObservableCollection<RegisterEntry> Registers { get; }

        public RegistersViewModel(CpuState cpuState, ObservableCollection<RegisterEntry> registers) {
            
            _cpuState = cpuState;
            Registers = registers;
            _fileDialogService = new FileDialogService();
            #region Commands
            LoadRegistersCommand = new LambdaCommand(OnLoadRegistersCommandExecuted, CanLoadRegistersCommandExecute);
            SaveRegistersCommand = new LambdaCommand(OnSaveRegistersCommandExecuted, CanSaveRegistersCommandExecute);
            ResetRegistersCommand = new LambdaCommand(OnResetRegistersCommandExecuted, CanResetRegistersCommandExecute);
            #endregion
        }
    }
}
