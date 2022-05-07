using homeautomation_service.Evaluations;
using homeautomation_service.Helper;
using Newtonsoft.Json;

namespace homeautomation_service.Devices
{
    internal class SonoffTemperatureAM2301 : AbstractDevice, IGetTemperatureData
    {
        private SonoffTemperatureSensorAM2301 _dataset ;

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
                    _dataset = JsonConvert.DeserializeObject<SonoffTemperatureSensorAM2301>(value);
                }
                catch (Exception)
                {
                    ;
                }
            }
        }

        public SonoffTemperatureAM2301(Device device, ISaveData saveData) : base(device, saveData)
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
                Temperature = _dataset.AM2301.Temperature,
                Humidity = _dataset.AM2301.Humidity
            };
        }


        public override void SendMqttData()
        {
            if (_dataset != null && _mqttInterface != null && Topic != "" && Topic != null)
            {
                // send mqtt topics
                _mqttInterface.PublishTopic(Name, "Temperature", _dataset.AM2301.Temperature);
                _mqttInterface.PublishTopic(Name, "Humidity", _dataset.AM2301.Humidity);
            }
        }
    }

    internal class SonoffTemperatureSensorAM2301
    {
        [JsonProperty("Time")]
        public DateTime Time { get; set; }

        [JsonProperty("AM2301")]
        public SonoffTemperatureSensorAM2301Data AM2301 { get; set; } = new();
    }

    internal class SonoffTemperatureSensorAM2301Data
    {
        [JsonProperty("Temperature")]
        public double Temperature { get; set; }

        [JsonProperty("Humidity")]
        public double Humidity { get; set; }
    }
}