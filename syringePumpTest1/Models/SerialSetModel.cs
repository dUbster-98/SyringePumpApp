using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SyringePumpTest1.Models
{
    public class SerialSetModel : ObservableObject
    {
        private string _pumpSerialPort;
        public string PumpSerialPort
        {
            get => _pumpSerialPort;
            set => SetProperty(ref _pumpSerialPort, value);
        }

        private int _pumpBoudRate;
        public int PumpBoudRate
        {
            get => _pumpBoudRate;
            set => SetProperty(ref _pumpBoudRate, value);
        }
    }
}
