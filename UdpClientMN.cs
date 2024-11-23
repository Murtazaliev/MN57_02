using System.Collections.Generic;
using System.Linq;
using System;
using System.Net;
using System.Net.Sockets;

namespace MN57_02
{
    class UdpClientMN
    {
        static int remotePort = 7755; // порт для отправки данных
        static int localPort = 7755; // локальный порт для прослушивания входящих подключений
        static UdpClient sender = new UdpClient(); // создаем UdpClient для отправки сообщений
        static UdpClient receiver = new UdpClient(localPort); // UdpClient для получения данных
        static IPEndPoint remoteIp = null; // адрес входящего подключения

        public void SendMessage(byte[] data, string remoteIp)
        {
            try
            {
                //WriteByteArray(data, data[4] + "####");
                if (data[4] == 60)
                {
                    List<byte> resultData = new List<byte>();
                    foreach (var item in data)
                    {
                        resultData.Add(item);
                    }
                    var resultPinValues = Program._raspberry.GetPinValues();
                    foreach (var item in resultPinValues)
                    {
                        resultData.Add(item);
                    }

                    byte[] result = new byte[resultData.Count];



                    result = resultData.ToArray();
                    sender.Send(result, result.Length, remoteIp, remotePort); // отправка



                    //WriteByteArray(resultData.ToArray(), data[4] + "__________________________________________3");
                    resultData = null;

                }
                else
                {
                    sender.Send(data, data.Length, remoteIp, remotePort); // отправка
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        //Todo Исправить номер команды в строке 81
        public void ReceiveMessage()
        {
            try
            {
                while (true)
                {
                    byte[] data = receiver.Receive(ref remoteIp); // получаем данные

                    if (data != null)
                    {
                        //Console.WriteLine(data[4]); //вывод полученного пина на изменение

                        (data[1], data[2]) = (data[2], data[1]);

                        try
                        {
                            if (data[4] == 60)
                            {
                                SendMessage(data, remoteIp.Address.ToString());
                            }
                            else if (data[4] == 0)
                            {
                                Program._raspberry.SetAllPinsState();
                                SendMessage(data, remoteIp.Address.ToString());
                            }
                            else
                            {
                                int pin = Program._raspberry.raspberryAccords.FirstOrDefault(x => x.Channel == (int)data[4]).GPIO;
                                if (pin == 24 && data[5] == 1)
                                {
                                    Program._raspberry.SetPinState(pin, 1);
                                    Program._raspberry.SetPinState(10, 0);
                                }
                                else if (pin == 10 && data[5] == 1)
                                {
                                    Program._raspberry.SetPinState(pin, 1);
                                    Program._raspberry.SetPinState(24, 1);
                                }
                                else if (pin == 10 && data[5] == 0)
                                {
                                    Program._raspberry.SetPinState(pin, 0);
                                    Program._raspberry.SetPinState(24, 0);
                                }
                                else if (pin == 11 || pin == 17)
                                {
                                    Program._raspberry.Gong(data[5]);
                                }
                                else
                                {
                                    Program._raspberry.SetPinState(pin, data[5]);
                                }
                                SendMessage(data, remoteIp.Address.ToString());
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "####");
            }
            finally
            {
                receiver.Close();
            }
        }
        public static void WriteByteArray(byte[] bytes, string name)
        {
            const string underLine = "--------------------------------";

            Console.WriteLine(name);
            Console.WriteLine(underLine.Substring(0,
                Math.Min(name.Length, underLine.Length)));
            Console.WriteLine(BitConverter.ToString(bytes));
            Console.WriteLine();
        }
    }
}
