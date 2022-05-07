using homeautomation_service.Devices;
using homeautomation_service.Helper;
using Newtonsoft.Json;

namespace homeautomation_service
{
    internal interface IGetDevice
    {
        public List<AbstractDevice> GetDevices();
        public bool DeviceExisting(string topic);
        public void SetData(string topic, string data);
    }

    internal class Configuration : IGetDevice
    {
        public List<AbstractDevice> Devices { get; set; } = new();
        private readonly ISaveData _saveData;
        private IMQTTPublisher _mqttInterface;

        public Configuration(ISaveData saveData/*, IMQTTPublisher mqttInterface*/)
        {
            _saveData = saveData;
            ReadConfig();
        }

        public void HandOverMqttInterface(IMQTTPublisher mqttInterface)
        {
               _mqttInterface = mqttInterface;
            foreach (AbstractDevice device in Devices)
            {
                device.HandoverMqttInterface(_mqttInterface);
            }
        }

        private void ReadConfig()
        {
            string fileContent = File.ReadAllText("config.json");
            AbstractDevice[] deviceList = JsonConvert.DeserializeObject<Device[]>(fileContent);

            foreach (Device device in deviceList)
            {
                switch (device.Classification)
                {
                    case 100:
                        Devices.Add(new SonoffPower(device, _saveData));
                        break;

                    case 101:
                        Devices.Add(new SonoffTemperatureAM2301(device, _saveData));
                        break;

                    case 102:
                        Devices.Add(new SonoffTemperatureDS18B20(device, _saveData));
                        break;

                    case 120:
                        Devices.Add(new BeckhoffPower(device, _saveData));
                        break;

                    case 130:
                        Devices.Add(new Fronius(device, _saveData));
                        break;

                    case 140:
                        Devices.Add(new WeatherStation(device, _saveData));
                        break;
                }
            }
        }

        public List<AbstractDevice> GetDevices()
        {
            return Devices;
        }
        public bool DeviceExisting(string topic)
        {
            return Devices.Any(x => x.Topic == topic);
        }
        public void SetData(string topic, string data)
        {
            if(Devices.Any(x => x.Topic == topic))
            {
                Devices.First(x => x.Topic == topic).Data = data;
                Console.WriteLine("Date received");
                Console.WriteLine(topic);
                Console.WriteLine(data);
            }
        }
        
    }
}