using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using SyringePumpTest1.Models;

namespace SyringePumpTest1.Service
{
    public interface ISerialService
    {
        SerialSetModel serialSet { get; set; }
        SerialPort PumpSerial { get; set; }

        void OpenConnection(SerialPort serialPort, string portName, int baudRate);
        void SendData(SerialPort serialPort, string data);

        public class SerialService : ISerialService
        {
            public SerialSetModel serialSet { get; set; }
            public SerialPort PumpSerial { get; set; }
            public SerialService()
            {                
                serialSet = new SerialSetModel();
                PumpSerial = new SerialPort();
            }

            public void OpenConnection (SerialPort serialPort, string portName, int baudRate)
            {
                if (serialPort.IsOpen == false)
                {
                    serialPort.PortName = portName;
                    serialPort.BaudRate = baudRate;
                    serialPort.Parity = Parity.None;
                    serialPort.DataBits = 8;
                    serialPort.StopBits = StopBits.One;

                    try
                    {
                        serialPort.Open();
                    }
                    catch
                    {
                        serialPort.Close();
                        throw new Exception("SerialPort Open Fail");
                    }
                }
            }

            public void SendData (SerialPort serialPort, string data)
            {
                try
                {
                    if (!serialPort.IsOpen)
                    {
                        serialPort.Open();
                    }
                    serialPort.Write(data);
                }
                catch (Exception)
                {
                    serialPort.Close();
                    throw new Exception(serialPort.PortName + "SerialPort Write Fail");
                }
            }
        }
    }
}
