using homeautomation_service.Evaluations;
using homeautomation_service.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homeautomation_service.Devices
{
    internal class BeckhoffPower : AbstractDevice
    {
        private BeckhoffPowerMeasurement _dataset ;
        public override dynamic Data { get { return _dataset; } set {
                try
                {
                    if(_dataset == null)
                    {
                        _dataset = new();
                    }
                    _dataset = JsonConvert.DeserializeObject<BeckhoffPowerMeasurement>(value);
                }
                catch (Exception)
                {
                    ;
                }
                
            } }

        public BeckhoffPower(Device device, ISaveData saveData) : base(device, saveData)
        {
            
        }

        public override object CalcData()
        {
            if (_dataset == null)
            {
                return null;
            }
            return _dataset;
        }
        public override object GetRawData()
        {
            if (_dataset == null)
            {
                return null;
            }
            return _dataset;
        }

        public override void SendMqttData()
        {
            if (_dataset != null && _mqttInterface != null && Topic != "" && Topic != null)
            {
                // send mqtt topics
                _mqttInterface.PublishTopic(Name, "Total_Service", _dataset.ENERGY.Total);
                _mqttInterface.PublishTopic(Name, "Power_Service", _dataset.ENERGY.Power);
                _mqttInterface.PublishTopic(Name, "Voltage_Service", _dataset.ENERGY.Voltage);
                _mqttInterface.PublishTopic(Name, "Current_Service", _dataset.ENERGY.Current);
            }
        }
    }

    class BeckhoffPowerMeasurement
    {
        [JsonProperty("Time")]
        public DateTime Time { get; set; }
        [JsonProperty("ENERGY")]
        public BeckhoffPowerMeasurementData ENERGY { get; set; } = new();
    }
    class BeckhoffPowerMeasurementData
    {
        [JsonProperty("Total")]
        public double Total { get; set; }
        [JsonProperty("Yesterday")]
        public double Yesterday { get; set; }
        [JsonProperty("Period")]
        public double Period { get; set; }
        [JsonProperty("Power")]
        public double Power { get; set; }
        [JsonProperty("ApparentPower")]
        public double ApparentPower { get; set; }
        [JsonProperty("ReactivePower")]
        public double ReactivePower { get; set; }
        [JsonProperty("Factor")]
        public double Factor { get; set; }
        [JsonProperty("Voltage")]
        public double Voltage { get; set; }
        [JsonProperty("Current")]
        public double Current { get; set; }
    }
}
