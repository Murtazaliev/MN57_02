using System;
using System.Collections.Generic;
using System.Linq;
using System.Device.Gpio;
using System.Threading;
using System.Configuration;

namespace MN57_02
{
    public class RaspberryService
    {


        public string REMOTE_IP_ADDRESS;
        PinValue GPIO2_STATE = PinValue.High;
        PinValue GPIO3_STATE = PinValue.High;
        public List<RaspberryAccord> raspberryAccords = new List<RaspberryAccord>()
        {
            new RaspberryAccord(){Channel=8, GPIO=27, State = 0}, //освещение стола,
            new RaspberryAccord(){Channel=9, GPIO=22, State = 0}, //переднего тамбура
            new RaspberryAccord(){Channel=10, GPIO=23, State = 0}, //заднего тамбура
            new RaspberryAccord(){Channel=11, GPIO=24, State = 0}, //50%
            new RaspberryAccord(){Channel=12, GPIO=10, State = 0}, //100%
            new RaspberryAccord(){Channel=13, GPIO=9, State = 0}, //щелевого освещения,
            new RaspberryAccord(){Channel=14, GPIO=25, State = 0}, //декоративного освещения,
            new RaspberryAccord(){Channel=15, GPIO=11, State = 0},  //вызова БП из туалета
            new RaspberryAccord(){Channel=16, GPIO=8, State = 0}, //мониторов,
            new RaspberryAccord(){Channel=19, GPIO=17, State = 0}, //гонг
            new RaspberryAccord(){Channel=21, GPIO=14, State = 0} //свет гардероб
        };
        GpioController controller = new GpioController();
        public void OpenPins()
        {

            foreach (var item in raspberryAccords)
            {
                lock (controller)
                {
                    controller.OpenPin(item.GPIO, PinMode.Output);
                }

            }
        }

        public void SetAllPinsState()
        {

            foreach (var item in raspberryAccords.Where(x => x.Channel != 15 && x.Channel != 16 && x.Channel != 19))
            {
                lock (controller)
                {
                    SetPinState(item.GPIO, 0);
                }
            }
        }
        public void SetPinState(int pin, int state)
        {

            lock (controller)
            {
                controller.Write(pin, state);
            }
        }

        public List<byte> GetPinValues()
        {
            List<byte> result = new List<byte>();
            foreach (var item in raspberryAccords.Where(x => x.Channel != 19))
            {
                int pin = item.GPIO;
                var state = controller.Read(pin) == PinValue.Low ? (byte)0 : (byte)1;
                result.Add(state);
            }
            return result;
        }

        public void GPIO_Click()
        {
            int callFLPin = 2; //пин кнопки вызова бортпроводника из туалета
            int garderobLightPin = 3; //пин открытия двери гардероба
            int pin11 = 11;
            controller.OpenPin(callFLPin, PinMode.Input);
            controller.OpenPin(garderobLightPin, PinMode.Input);
            while (true)
            {
                lock (controller)
                {

                    var callFLPinState = controller.Read(callFLPin);
                    var garderobLightPinState = controller.Read(garderobLightPin);

                    if (callFLPinState != GPIO2_STATE)
                    {
                        Thread.Sleep(100);
                        var callFLPinStateNew = controller.Read(callFLPin);
                        if (callFLPinState == callFLPinStateNew)
                        {
                            if (callFLPinState == PinValue.Low)
                            {
                                GPIO11_Power();
                            }
                            var pin11state = controller.Read(pin11);
                            byte PIN_STATE = Convert.ToByte(pin11state == PinValue.High ? (byte)1 : (byte)0);
                            byte[] data = { 0x20, 0xF0, 0xA0, 0x00, 0x0F, PIN_STATE };
                            Program._client.SendMessage(data, REMOTE_IP_ADDRESS);
                            GPIO2_STATE = callFLPinState;
                        }
                    }
                    if (garderobLightPinState != GPIO3_STATE)
                    {
                        Thread.Sleep(100);
                        var garderobLightPinStateNew = controller.Read(garderobLightPin);
                        if (garderobLightPinState == garderobLightPinStateNew)
                        {
                            GPIO14_Power(garderobLightPinState);
                            GPIO3_STATE = callFLPinState;
                        }
                    }


                    Thread.Sleep(300);
                }
            }

        }
        void GPIO11_Power()
        {
            lock (controller)
            {
                var pin11 = 11;
                PinValue pin11state = controller.Read(pin11);
                // Console.WriteLine(String.Format("pin:{0}/State{1}", pin11, pin11state));
                SetPinState(pin11, pin11state == PinValue.Low ? 1 : 0);



                if (pin11state == PinValue.Low)
                {
                    GPIO17_Power();
                }
            }
        }
        void GPIO14_Power(PinValue state)
        {
            lock (controller)
            {
                var pin14 = 14;
                SetPinState(pin14, state == PinValue.Low ? 1 : 0);
            }
        }
        public void Gong(int state)
        {
            lock (controller)
            {
                var pin11 = 11;
                PinValue pin11state = controller.Read(pin11);
                //Console.WriteLine(String.Format("pin:{0}/State{1}", pin11, pin11state));
                //SetPinState(pin11, pin11state == PinValue.Low ? 1 : 0);


                //pin11state = controller.Read(pin11);
                //byte PIN_STATE = Convert.ToByte(pin11state == PinValue.High ? (byte)0 : (byte)1);
                //byte[] data = { 0x20, 0xF0, 0xA0, 0x00, 0x0F, PIN_STATE };
                //Program._client.SendMessage(data, REMOTE_IP_ADDRESS);



                if (state == 1)
                {
                    GPIO17_Power();
                }
                else
                {
                    SetPinState(pin11, 0);
                }
            }
        }
        void GPIO17_Power()
        {

            var pin17 = 17;
            SetPinState(pin17, 1);
            Thread.Sleep(800);
            SetPinState(pin17, 0);
        }
    }











    public class RaspberryAccord
    {
        public int Channel { get; set; }
        public int GPIO { get; set; }
        public int State { get; set; }
    }

}
