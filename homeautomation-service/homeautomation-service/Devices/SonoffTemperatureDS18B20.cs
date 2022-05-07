using homeautomation_service.Evaluations;
using homeautomation_service.Helper;
using Newtonsoft.Json;

namespace homeautomation_service.Devices
{
    internal class SonoffTemperatureDS18B20 : AbstractDevice
    {
        private SonoffTemperatureSensorDS18B20 _dataset;

        public override dynamic Data
        {
            get { return _dataset; }
            set
            {
                try
                {
                    if (_dataset == null)
                    {
                        _dataset = new();
                    }
                    _dataset = JsonConvert.DeserializeObject<SonoffTemperatureSensorDS18B20>(value);
                }
                catch (Exception)
                {
                    ;
                }
            }
        }

        public SonoffTemperatureDS18B20(Device device, ISaveData saveData) : base(device, saveData)
        {
        }

        public override object CalcData()
        {
            return CalcTemperatureData();
        }
        public override object GetRawData()
        {
            if (_dataset == null)
            {
                return null;
            }
            return _dataset;
        }
        public TemperatureData CalcTemperatureData()
        {
            if (_dataset == null)
            {
                return null;
            }
            return new TemperatureData()
            {
                Temperature = _dataset.DS18B20.Temperature,
                Humidity = 0
            };
        }


        public override void SendMqttData()
        {
            if (_dataset != null && _mqttInterface != null && Topic != "" && Topic != null)
            {
                // send mqtt topics
                _mqttInterface.PublishTopic(Name, "Temperature", _dataset.DS18B20.Temperature);
            }
        }
    }

    internal class SonoffTemperatureSensorDS18B20
    {
        [JsonProperty("Time")]
        public DateTime Time { get; set; }

        [JsonProperty("DS18B20")]
        public SonoffTemperatureSensorDS18B20Data DS18B20 { get; set; } = new();
    }

    internal class SonoffTemperatureSensorDS18B20Data
    {
        [JsonProperty("Temperature")]
        public double Temperature { get; set; }
    }
}