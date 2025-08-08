using Micro.Infrastructure.Commands;
using Micro.Infrastructure.Entrys;
using Micro.Models;
using Micro.Resources;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Input;


namespace Micro.ViewModels
{
    internal class CpuViewModel : INotifyPropertyChanged
    {

        #region Commands

        #region RestartCpuCommand
        public ICommand RestartCpuCommand {get; set; }
        private bool CanRestartCpuCommandExecute(object p) => true;
        private void OnRestartCpuCommandExecuted(object p) => _cpuState.RestartCpu();
        #endregion

        #region ExecuteMicrocommandCommand
        public ICommand ExecuteMicrocommandCommand {get; set; }
        private bool CanExecuteMicrocommandCommandExecute(object p) => CpuExecutionState != CpuState.ExecutionState.Stopped && CpuExecutionState != CpuState.ExecutionState.Running;
        private void OnExecuteMicrocommandCommandExecuted(object p) => _cpuState.Step();
        #endregion

        #region ExecuteInstructionCommand

        public ICommand ExecuteInstructionCommand { get; set; }
        private bool CanExecuteInstructionCommandExecute(object p) => CpuExecutionState != CpuState.ExecutionState.Stopped && CpuExecutionState != CpuState.ExecutionState.Running;
        private void OnExecuteInstructionCommandExecuted(object p) => _cpuState.ExecuteInstruction();

        #endregion

        #region RunMicroprogrammCommand

        public ICommand RunMicroprogrammCommand { get; set; }
        private bool CanRunMicroprogrammCommandExecute(object p) => CpuExecutionState != CpuState.ExecutionState.Stopped;
        private void OnRunMicroprogrammCommandExecuted(object p) => _cpuState.RunMicroprogramm();

        #endregion

        #region SaveTraceCommand
        public ICommand SaveTraceCommand { get; set; }
        private bool CanSaveTraceCommandExecute(object p) => true;
        private void OnSaveTraceCommandExecuted(object p) => this.SaveTrace(_cpuState.Trace);
        #endregion

        #endregion

        
        private readonly FileDialogService _fileDialogService;
        private readonly CpuState _cpuState;
        public ObservableCollection<RegisterEntry> Registers { get; }
        public CpuState.ExecutionState CpuExecutionState => _cpuState.CpuExecutionState;

        public ObservableCollection<FlagRow> FlagRows { get; }
        #region Fields
        public string Alu => _cpuState.Alu.ToString("X4");
        public string Sda => _cpuState.Sda.ToString("X4");
        #endregion

        public CpuViewModel(CpuState cpuState, ObservableCollection<RegisterEntry> registers) {

            #region Commands

            RestartCpuCommand = new LambdaCommand(OnRestartCpuCommandExecuted, CanRestartCpuCommandExecute);
            ExecuteMicrocommandCommand = new LambdaCommand(OnExecuteMicrocommandCommandExecuted, CanExecuteMicrocommandCommandExecute);
            ExecuteInstructionCommand = new LambdaCommand(OnExecuteInstructionCommandExecuted, CanExecuteInstructionCommandExecute);
            SaveTraceCommand = new LambdaCommand(OnSaveTraceCommandExecuted, CanSaveTraceCommandExecute);
            RunMicroprogrammCommand = new LambdaCommand(OnRunMicroprogrammCommandExecuted, CanRunMicroprogrammCommandExecute);

            #endregion

            _fileDialogService = new FileDialogService();
            _cpuState = cpuState;
            _cpuState.PropertyChanged += (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case nameof(_cpuState.Alu):
                        OnPropertyChanged(nameof(_cpuState.Alu));
                        
                        break;
                    case nameof(_cpuState.Sda):
                        OnPropertyChanged(nameof(_cpuState.Sda));
                        break;
                    case nameof(_cpuState.CpuExecutionState):
                        OnPropertyChanged(nameof(_cpuState.CpuExecutionState));
                        break;

                }
            };
            Registers = registers;
            FlagRows =
            [
                new FlagRow(_cpuState.Registers,"RFI"),
                new FlagRow(_cpuState.Registers, "RFD")
            ];
        }

        private void SaveTrace(List<List<ushort>> trace)
        {
            var path = _fileDialogService.SaveFile("CSV UTF-8 (Разделитель — точка с запятой) | *.csv");
            if (path == null) return;
            try
            {

                using (var writer = new System.IO.StreamWriter(path, false, Encoding.UTF8))
                {
                    // Заголовки — соответствуют полям
                    string[] headers =
                    [
                        "CMK", "AX", "CX", "DX", "BX",
                        "SP", "BP", "SI", "DI", "CS",
                        "SS", "DS", "ES", "IP", "PSW",
                        "RGK", "RW", "RGA", "RGB", "Alu", "Sda",
                        "RGQ", "N", "Z", "V", "C", "P", "ARAM", "RGR",
                        "RGW", "RACT"
                    ];

                    writer.WriteLine(string.Join(";", headers));

                    foreach (var row in trace)
                    {
                        var line = new List<string>();
                        line.Add(row[0].ToString("X2")); // Добавление CMK
                        
                        for (var i = 1; i < 22; i++)
                        {
                            line.Add(row[i].ToString("X4")); // Добавление регистров, ALU, SDA
                        }
                        
                        line.Add(((row[22] >>> 7) & 1).ToString()); // N
                        line.Add(((row[22] >>> 6) & 1).ToString()); // Z
                        line.Add(((row[22] >>> 11) & 1).ToString()); // V
                        line.Add((row[22] & 1).ToString()); // C
                        line.Add(((row[22] >>> 2) & 1).ToString()); // P
                        for (var i = 23; i < 27; i++)
                        {
                            line.Add(row[i].ToString("X4")); // Добавление регистров, ALU, SDA
                        }

                        writer.WriteLine(string.Join(";", line));
                    }
                }
            }
            catch 
            {
                MessageBox.Show("Ошибка при сохрании файла. Вероятно файл занят другим процессом.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region NotifyPropertyChanged
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
        #endregion
    }

    internal class FlagRow : INotifyPropertyChanged
    {
        public string Register { get; }
        private readonly RegisterMemory _registers;
        public string Sf => ((_registers[Register] >> 7) & 1).ToString();
        public string Zf => ((_registers[Register] >> 6) & 1).ToString();
        public string Vf => ((_registers[Register] >> 11) & 1).ToString();
        public string Cf => (_registers[Register] & 1).ToString();
        public string Pf => ((_registers[Register]>> 2) & 1).ToString();

        public FlagRow(RegisterMemory registers, string register)
        {
            Register = register;
            _registers = registers;
            registers.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName != "RFI" && args.PropertyName != "RFD") return;
                OnPropertyChanged(nameof(Sf));
                OnPropertyChanged(nameof(Zf));
                OnPropertyChanged(nameof(Vf));
                OnPropertyChanged(nameof(Cf));
                OnPropertyChanged(nameof(Pf));
            };

        }

        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
