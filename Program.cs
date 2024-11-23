using System;
using System.Device.Gpio;
using System.Threading;

namespace MN57_02
{
    class Program
    {
        static GpioController controller = new GpioController();
        public static RaspberryService _raspberry = new RaspberryService();
        public static UdpClientMN _client = new UdpClientMN();
        static void Main(string[] args)
        {
            try
            {
               
                Console.WriteLine("#Начало работы");
                _raspberry.OpenPins();
                _raspberry.REMOTE_IP_ADDRESS = "192.168.1.1";
                Thread thread1 = new Thread(new ThreadStart(_client.ReceiveMessage));
                thread1.Start();
                byte[] data = { 0x20, 0xF0, 0xA0, 0x00, 0x3C, 0x00 };
                _client.SendMessage(data, _raspberry.REMOTE_IP_ADDRESS);
                _raspberry.GPIO_Click();
                //int pin2 = 2;
                //controller.OpenPin(pin2);
                //while (true)
                //{

                //    var pin2state = controller.Read(pin2);

                //    Console.WriteLine(pin2state.ToString() + pin2state + pin2state + pin2state + pin2state + pin2state + pin2state + pin2state + pin2state + pin2state + pin2state);

                //    Thread.Sleep(300);
                //}
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);            
            }
        }
    }
}
