//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Device.Gpio;
//using System.Threading;
//using System.Configuration;

//namespace MN57_02
//{
//    public class RaspberryService
//    {
//        public static string REMOTE_IP_ADDRESS;
//        static PinValue GPIO2_STATE = PinValue.High;
//        public static List<RaspberryAccord> raspberryAccords = new List<RaspberryAccord>()
//        {
//            new RaspberryAccord(){Channel=8, GPIO=27, State = 0},
//            new RaspberryAccord(){Channel=9, GPIO=22, State = 0},
//            new RaspberryAccord(){Channel=10, GPIO=23, State = 0},
//            new RaspberryAccord(){Channel=11, GPIO=24, State = 0},
//            new RaspberryAccord(){Channel=12, GPIO=10, State = 0},
//            new RaspberryAccord(){Channel=13, GPIO=9, State = 0},
//            new RaspberryAccord(){Channel=14, GPIO=25, State = 0},
//            new RaspberryAccord(){Channel=15, GPIO=11, State = 0},
//            new RaspberryAccord(){Channel=16, GPIO=8, State = 0},
//            new RaspberryAccord(){Channel=19, GPIO=17, State = 0}
//        };
//        static List<RaspberryAccord> raspberryAccordsOld = new List<RaspberryAccord>();
//        public static void SetPinState()
//        {
//            while (true)
//            { 
//            bool IsWork = true;
//            foreach (var item in raspberryAccords)
//            {
//                raspberryAccordsOld.Add(item);
//            }
//            using var controller = new GpioController();
//                while (IsWork)
//                {
//                    lock (raspberryAccords)
//                    {
//                        foreach (var item in raspberryAccords)
//                        {
//                            if (item.GPIO == 17 && item.State == 1)
//                            {
//                                GPIO17_Power();
//                                item.State = 0;
//                            }
//                            else
//                            {
//                                controller.OpenPin(item.GPIO, PinMode.Output);
//                                controller.Write(item.GPIO, (item.State == 0 ? PinValue.High : PinValue.Low));
//                            }
//                        }
//                        Thread.Sleep(30);
//                    }
//                    while (!IsWork)
//                    {
//                        foreach (var item in raspberryAccords)
//                        {
//                            var accord = raspberryAccordsOld.FirstOrDefault(x => x.GPIO == item.GPIO);
//                            if (accord.State != item.State)
//                            {
//                                accord.State = item.State;
//                                IsWork = true;
//                            }
//                        }
//                        Thread.Sleep(30);
//                    }
//                }
//            }
//        }

//        static public List<byte> GetPinValues()
//        {
//            using var controller = new GpioController();
//            List<byte> result = new List<byte>();
//            foreach (var item in raspberryAccords)
//            {
//                int pin = item.GPIO;
//                controller.OpenPin(pin, PinMode.Output);
//                result.Add(controller.Read(pin) == PinValue.High ? (byte)0 : (byte)1);
//            }
//            return result;
//        }

//        static public void GPIO2_Click()
//        {
//            //string IP_ADDRESS = "192.168.1.12";// ConfigurationManager.AppSettings.Get("IpAddress");
//            Console.WriteLine(ConfigurationManager.AppSettings.Get("IpAddress") + "***");
//            int pin = 2;
//                using var controller = new GpioController();
//                 controller.OpenPin(pin, PinMode.Input);
//                controller.OpenPin(11, PinMode.Input);
//            while (true)
//            {

                

//                var pin2state = controller.Read(pin);

//                if (pin2state != GPIO2_STATE)
//                {
                    
//                    if (pin2state == PinValue.Low)
//                    {
//                        GPIO11_Power();                    
//                    }
//                    var pin11state = controller.Read(11);
//                    byte PIN_STATE = Convert.ToByte(pin11state == PinValue.High ? (byte)0 : (byte)1);
//                    byte[] data = { 0x20, 0xF0, 0xA0, 0x00, 0x0F, PIN_STATE };
//                    UdpClientMN.SendMessage(data, REMOTE_IP_ADDRESS);
//                    GPIO2_STATE = pin2state;
//                    Thread.Sleep(300);
//                }
//            }
//        }
//        //static Thread thread = new Thread(new ThreadStart(GPIO17_Power));
//        static void GPIO11_Power()
//        {
//            lock (raspberryAccords)
//            {
//                var pin = raspberryAccords.FirstOrDefault(x => x.GPIO == 11);

//                pin.State = pin.State == 0 ? 1 : 0;
//                if (pin.State == 1)
//                {
//                    GPIO17_Power();
//                }
//            }
//        }
//        static void GPIO17_Power()
//        {

//            var pin = 17;
//            using var controller = new GpioController();
//            controller.OpenPin(pin, PinMode.Output);
//            controller.Write(pin, PinValue.Low);
//            Thread.Sleep(800);
//            controller.Write(pin, PinValue.High);
//        }
//    }











//    public class RaspberryAccord
//    {
//        public int Channel { get; set; }
//        public int GPIO { get; set; }
//        public int State { get; set; }
//    }
//}
