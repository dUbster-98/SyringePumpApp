using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using System.IO.Ports;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows;
using SyringePumpTest1.Services;
using SyringePumpTest1.Models;
using SyringePumpTest1.Service;

namespace SyringePumpTest1.ViewModels
{
    public class SerialSetViewModel : ObservableObject
    {
        private readonly IIniSetService _iniSetService;
        private readonly ISerialService _serialService;

        public event Action CloseRequested;

        public ObservableCollection<string> SerialPortList { get; set; }

        private SerialSetModel _selectedItem;
        public SerialSetModel SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public SerialSetViewModel(IIniSetService iniService, ISerialService serialService)
        {
            _iniSetService = iniService;
            _serialService = serialService;

            SerialPortList = new ObservableCollection<string>(SerialPort.GetPortNames());

            SelectedItem = new SerialSetModel()
            {
                PumpSerialPort = _iniSetService.GetIni("Pump", "Com", IniSetService.path1),
                PumpBoudRate = int.Parse(_iniSetService.GetIni("Pump", "BR", IniSetService.path1)),
            };
        }

        public IList<int> BaudList { get; set; } = new List<int>()
        {
            9600, 19200, 38400, 57600, 115200
        };

        private ICommand _serialSaveCommand;
        public ICommand SerialSaveCommand
        {
            get
            {
                return _serialSaveCommand ??
                    (_serialSaveCommand = new RelayCommand(this.SaveButtonClick));
            }
        }
        private ICommand _serialCancelCommand;
        public ICommand SerialCancelCommand
        {
            get
            {
                return _serialCancelCommand ??
                    (_serialCancelCommand = new RelayCommand(this.CancelButtonClick));
            }
        }

        private void SaveButtonClick()
        {
            _iniSetService.SetIni("Pump", "Com", SelectedItem.PumpSerialPort, IniSetService.path1);
            _iniSetService.SetIni("Pump", "BR", SelectedItem.PumpBoudRate.ToString(), IniSetService.path1);

            ShowConfirmMsgExecute();

            CloseRequested?.Invoke();
        }

        private void CancelButtonClick()
        {
            CloseRequested?.Invoke();
        }

        private void ShowConfirmMsgExecute()
        {
            MessageBox.Show("저장되었습니다");            
        }
    }
}
