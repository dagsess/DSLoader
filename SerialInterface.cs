using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLoader
{
    class SerialInterface
    {
        string _portName;
        int _baudRate;
        int _readTimeout;
        int _writeTimeout;
        public SerialInterface(string portName, int baudRate, int readTimeout = 2000, int writeTimeout = 2000)
        {
            _portName = portName;
            _baudRate = baudRate;
            _readTimeout = readTimeout;
            _writeTimeout = writeTimeout;
        }

        public Task<bool> OpenInterface()
        {
            return Task<bool>.Factory.StartNew(() =>
            {
                SerialPort serialPort = new SerialPort();
                serialPort.PortName = _portName;
                serialPort.BaudRate = _baudRate;

                serialPort.Open();
                serialPort.ReadTimeout = _readTimeout;
                serialPort.WriteTimeout = _writeTimeout;

                try
                {
                    string message = "";
                    do
                    {
                        serialPort.Write("HELLO");
                        message = serialPort.ReadLine();
                    }
                    while (message != "OK");
                    serialPort.Close();
                    return true;
                }
                catch (TimeoutException)
                {

                }
                serialPort.Close();
                return false;
            });
        }

    }
}
