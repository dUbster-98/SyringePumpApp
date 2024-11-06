using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyringePumpTest1.Service;
using SyringePumpTest1.Views;
using SyringePumpTest1.Models;
using SyringePumpTest1.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SyringePumpTest1.Services;
using CommunityToolkit.Mvvm.Messaging;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows;

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

        private int _downPara;
        public int DownPara
        {
            get => _downPara;
            set => SetProperty(ref _downPara, value);
        }

        private int _upPara;
        public int UpPara
        {
            get => _upPara;
            set => SetProperty(ref _upPara, value);
        }

        private int _lowerMargin;
        public int LowerMargin
        {
            get => _lowerMargin;
            set => SetProperty(ref _lowerMargin, value);
        }

        private int _upperMargin;
        public int UpperMargin
        {
            get => _upperMargin;
            set => SetProperty(ref _upperMargin, value);
        }

        private int _speedPara;
        public int SpeedPara
        {
            get => _speedPara;
            set => SetProperty(ref _speedPara, value);
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }

        private ICommand _openWindowCommand;
        public ICommand OpenWindowCommand
        {
            get => _openWindowCommand ?? (_openWindowCommand = new RelayCommand(OpenWindow));
        }

        private ICommand _reconnectCommand;
        public ICommand ReconnectCommand
        {
            get => _reconnectCommand ?? (_reconnectCommand = new RelayCommand(SerialReconnect));
        }

        private ICommand _keyDownCommand;
        public ICommand KeyDownCommand
        {
            get => _keyDownCommand ?? (_keyDownCommand = new RelayCommand(EnterKeyDown));
        }

        private ICommand _moveCommand;
        public ICommand MoveCommand
        {
            get => _moveCommand ?? (_moveCommand = new AsyncRelayCommand<object>(PumpMove));            
        }

        private ICommand _windowLoadedCommand;
        public ICommand WindowLoadedCommand
        {
            get => _windowLoadedCommand ?? (_windowLoadedCommand = new RelayCommand(Window_Loaded));
        }

        private ICommand _setSpeedCommand;
        public ICommand SetSpeedCommand
        {
            get => _setSpeedCommand ?? (_setSpeedCommand = new AsyncRelayCommand(SetSpeed));
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
        
        private async Task PumpMove(object dir)
        {
            try
            {
                int para = 0;
                switch (dir)
                {
                    case "A":
                        para = MovePara;
                        break;
                    case "P":
                        para = DownPara;
                        break;
                    case "D":
                        para = UpPara;
                        break;

                    default:
                        throw new Exception("Wrong direction");

                }
                _serialService.SendData(_serialService.PumpSerial, $"/1{dir}{para}R");
                TextBoxAddText($">>>/1{dir}{para}R");
                await SerialReadAsync();

                await ParsingPosition();

            }
            catch (Exception ex)
            {
                TextBoxAddText(ex.Message);
            }
        }

        private async Task ParsingPosition()
        {
            _serialService.SendData(_serialService.PumpSerial, $"/1?");
            string res = await SerialReadAsync();
            string resp = Regex.Replace(res, @"\p{C}+", "").Trim();
            MovePara = int.Parse(resp.Substring(4));

            UpperMargin = MovePara;
            LowerMargin = 6000 - MovePara;
        }

        private async Task SetSpeed()
        {
            try
            {
                _serialService.SendData(_serialService.PumpSerial, $"/1S{SpeedPara}R");
                TextBoxAddText($">>>/1S{SpeedPara}R");
                await SerialReadAsync();
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

            Task.Run(()=> ParsingPosition());
        }

        private void Window_Loaded()
        {
            if (Application.Current.MainWindow is Window wnd)
            {
                WindowInteropHelper helper = new(wnd);
                HwndSource source = HwndSource.FromHwnd(helper.Handle);
                source.AddHook(new HwndSourceHook(WndProc));
            }
        }

        private IntPtr WndProc(IntPtr hWnd, int nMsg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            uint WM_DEVICECHANGE = 0x0219;
            uint DBT_DEVICEARRIVAL = 0x8000;
            uint DBT_DEVICEREMOVECOMPLETE = 0x8004;

            if (nMsg == WM_DEVICECHANGE)
            {
                //장치 변동
                int wparam = wParam.ToInt32();
                if (wparam == DBT_DEVICEARRIVAL || wparam == DBT_DEVICEREMOVECOMPLETE)
                {
                    if (wParam.ToInt32() == DBT_DEVICEARRIVAL)
                    {
                        TextBoxAddText("New Device Detected");
                    }
                    else if (wParam.ToInt32() == DBT_DEVICEREMOVECOMPLETE)
                    {
                        TextBoxAddText("Device Unpluged");
                        IsConnected = false;
                    }
                }
            }
            return IntPtr.Zero;
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
                IsConnected = true;
            }
            catch (Exception ex) 
            {
                TextBoxAddText(ex.Message);
            }
        }

        public void TextBoxAddText(string text)
        {
            _textBoxService.AddText(text+"\r\n");
            OnPropertyChanged(nameof(TextBoxContext));
        }
        public void RemoveText()
        {
            _textBoxService.RemoveText();
            OnPropertyChanged(nameof(InputString));
        }

        static TaskCompletionSource<bool>? dataReceivedTaskCompletion;
        public async Task<string> SerialReadAsync()
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
            }
            else
            {
                TextBoxAddText("Data Read Fail");
            }
            return data;
        }
        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            dataReceivedTaskCompletion.TrySetResult(true);
        }
    }
}
