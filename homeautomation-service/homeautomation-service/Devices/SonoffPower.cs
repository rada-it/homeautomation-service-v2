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
    internal class SonoffPower : AbstractDevice, IGetConsumptionData
    {
        private SonoffPowerSensor _dataset ;
        public override dynamic Data { get { return _dataset; } set {
                try
                {
                    if (_dataset == null)
                    {
                        _dataset = new();
                    }
                    _dataset = JsonConvert.DeserializeObject<SonoffPowerSensor>(value);
                }
                catch (Exception)
                {
                    ;
                }
                
            } }

        public SonoffPower(Device device, ISaveData saveData) : base(device, saveData)
        {

        }

        public override object CalcData()
        {
            return CalcConsumptionData();
        }
        public override object GetRawData()
        {
            if (_dataset == null)
            {
                return null;
            }
            return _dataset;
        }
        public PowerConsumptionSplice CalcConsumptionData()
        {
            if (_dataset == null)
            {
                return null;
            }

            double measurementDays = (DateTime.Now - _dataset.ENERGY.TotalStartTime).TotalDays;
            if (measurementDays > 0)
            {
                double yearlyConsumption = _dataset.ENERGY.Total / measurementDays * 365;
                // send mqtt topics
                _mqttInterface.PublishTopic(Name, "YearlyConsumption", yearlyConsumption);
            }

            return new PowerConsumptionSplice()
            {
                Consumption = _dataset.ENERGY.Power,
                Total = _dataset.ENERGY.Total,
                TotalStartTime = _dataset.ENERGY.TotalStartTime,
                Running = false
            };
        }


        public override void SendMqttData()
        {
            if (_dataset != null && _mqttInterface != null && Topic != "" && Topic != null)
            {
                // send mqtt topics
                _mqttInterface.PublishTopic(Name, "Total", _dataset.ENERGY.Total);
                _mqttInterface.PublishTopic(Name, "Power", _dataset.ENERGY.Power);
                _mqttInterface.PublishTopic(Name, "Voltage", _dataset.ENERGY.Voltage);
                _mqttInterface.PublishTopic(Name, "Current", _dataset.ENERGY.Current);
            }
        }
    }

    class SonoffPowerSensor
    {
        [JsonProperty("Time")]
        public DateTime Time { get; set; }
        [JsonProperty("ENERGY")]
        public SonoffEnergy ENERGY { get; set; } = new();
    }
    class SonoffState
    {
        public DateTime Time { get; set; }
        public string Uptime { get; set; }
        public double Vcc { get; set; }
        public string SleepMode { get; set; }
        public double Sleep { get; set; }
        public double LoadAvg { get; set; }
        public string POWER { get; set; }
    }
    class SonoffEnergy
    {
        [JsonProperty("TotalStartTime")]
        public DateTime TotalStartTime { get; set; }
        [JsonProperty("Total")]
        public double Total { get; set; }
        [JsonProperty("Yesterday")]
        public double Yesterday { get; set; }
        [JsonProperty("Today")]
        public double Today { get; set; }
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
