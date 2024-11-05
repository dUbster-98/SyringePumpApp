using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyringePumpTest1.Service;
using SyringePumpTest1.Views;
using SyringePumpTest1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SyringePumpTest1.Services;
using CommunityToolkit.Mvvm.Messaging;
using System.IO.Ports;

namespace syringePumpTest1.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly IIniSetService _iniSetService;
        private readonly ISerialService _serialService;
        private readonly ITextBoxService _textBoxService;

        SerialPort pumpSerial = new SerialPort();

        private string _serialLog;
        public string SerialLog
        {
            get => _serialLog;
            set => SetProperty(ref _serialLog, value);
        }

        public string InputString
        {
            get => _textBoxService.InputString;
            set
            {
                _textBoxService.InputString = value;
                OnPropertyChanged(nameof(InputString));
            }
        }

        public string TextBoxContext
        {
            get => _textBoxService.TextBoxContext;
            set
            {
                _textBoxService.TextBoxContext = value;
                OnPropertyChanged(nameof(TextBoxContext));
            }
        }

        private int _movePara;
        public int MovePara
        {
            get => _movePara;
            set => SetProperty(ref _movePara, value);
        }

        private ICommand _openWindowCommand;
        public ICommand OpenWindowCommand
        {
            get
            {
                return _openWindowCommand ?? (_openWindowCommand = new RelayCommand(OpenWindow));
            }
        }

        private ICommand _reconnectCommand;
        public ICommand ReconnectCommand
        {
            get
            {
                return _reconnectCommand ?? (_reconnectCommand = new RelayCommand(SerialReconnect));
            }
        }

        private ICommand _keyDownCommand;
        public ICommand KeyDownCommand
        {
            get
            {
                return _keyDownCommand ?? (_keyDownCommand = new RelayCommand(EnterKeyDown));
            }
        }

        private ICommand _moveCommand;
        public ICommand MoveCommand
        {
            get
            {
                return _moveCommand ?? (_moveCommand = new RelayCommand(PumpMove));
            }
        }

        private void OpenWindow()
        {
            SerialSetWindow serialSetWindow = new SerialSetWindow();
            serialSetWindow.ShowDialog();
        }

        private void SerialReconnect()
        {
            InitializeSerialPort();
        }

        private void EnterKeyDown()
        {
            try
            {
                _serialService.SendData(_serialService.PumpSerial, InputString);
                TextBoxAddText(">>" + InputString);

                Task.Run(() => SerialReadAsync());
            }
            catch (Exception ex)
            {
                TextBoxAddText(ex.Message);
            }
            finally
            {
                RemoveText();
            }
        }
        
        private void PumpMove()
        {
            try
            {
                _serialService.SendData(_serialService.PumpSerial, $"/1A{MovePara.ToString()}R");
                Task.Run(() => SerialReadAsync());
            }
            catch (Exception ex)
            {
                TextBoxAddText(ex.Message);
            }
        }

        public MainViewModel(IIniSetService iniSetService, ISerialService serialService, ITextBoxService textBoxService)
        {
            _iniSetService = iniSetService;
            _serialService = serialService;
            _textBoxService = textBoxService;
                       
            InputString = "";
            SerialLog = "";

            InitializeSerialPort();

        }

        private void InitializeSerialPort() 
        {
            _serialService.serialSet = new SerialSetModel()
            {
                PumpSerialPort = _iniSetService.GetIni("Pump", "Com", IniSetService.path1),
                PumpBoudRate = int.Parse(_iniSetService.GetIni("Pump", "BR", IniSetService.path1)),
            };

            try
            {
                _serialService.OpenConnection(_serialService.PumpSerial, _serialService.serialSet.PumpSerialPort, _serialService.serialSet.PumpBoudRate);
                TextBoxAddText("Pump Ready");
            }
            catch (Exception ex) 
            {
                TextBoxAddText(ex.Message);
            }
        }

        public void TextBoxAddText(string text)
        {
            _textBoxService.AddText(text);
            OnPropertyChanged(nameof(TextBoxContext));
        }
        public void RemoveText()
        {
            _textBoxService.RemoveText();
            OnPropertyChanged(nameof(InputString));
        }

        static TaskCompletionSource<bool>? dataReceivedTaskCompletion;
        public async Task<bool> SerialReadAsync()
        {
            dataReceivedTaskCompletion = new TaskCompletionSource<bool>();
            _serialService.PumpSerial.DataReceived += DataReceivedHandler;

            Task timeoutTask = Task.Delay(3500);
            await Task.WhenAny(dataReceivedTaskCompletion.Task, timeoutTask);
            await Task.Delay(100);

            string data = _serialService.PumpSerial.ReadExisting();
            _serialService.PumpSerial.DataReceived -= DataReceivedHandler;

            if (dataReceivedTaskCompletion.Task.IsCompleted)
            {
                TextBoxAddText("<<<" + data);
                return true;
            }
            else
            {
                TextBoxAddText("Data Read Fail");
                return false;
            }
        }
        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            dataReceivedTaskCompletion.TrySetResult(true);  // 데이터 수신 작업을 완료함
        }
    }
}
