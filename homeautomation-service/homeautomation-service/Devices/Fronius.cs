using homeautomation_service.Evaluations;
using homeautomation_service.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homeautomation_service.Devices
{
    internal class Fronius : AbstractDevice, IGetConsumptionProductionData
    {
        private readonly FroniusTotal _froniusSmartmeter;
        private readonly FroniusDetails _froniusSmartmeter1;

        public Fronius(Device device, ISaveData saveData) : base(device, saveData)
        {
            if(Interval != 0)
            {
                _froniusSmartmeter = new(new RestApi("http://192.168.0.111/solar_api/v1/GetInverterRealtimeData.cgi?Scope=System", Interval));
                _froniusSmartmeter1 = new(new RestApi("http://192.168.0.111/solar_api/v1/GetMeterRealtimeData.cgi?Scope=System", Interval));
            }
        }

        public override object CalcData()
        {
            return CalcConsumptionProductionData();
        }
        public override object GetRawData()
        {
            if (_froniusSmartmeter.GetRawData() == null)
            {
                return null;
            }
            return new{ FroniusTotal = _froniusSmartmeter.GetRawData(), FroniusDetails = _froniusSmartmeter1.GetRawData() };
        }

        private double _consumption = 0;
        private double _production = 0;
        public PowerConsumptionProductionSplice CalcConsumptionProductionData()
        {
            if(_froniusSmartmeter1 != null)
            {
                _consumption = _froniusSmartmeter1.GetCurrentConsumption();
                _production = _froniusSmartmeter.GetCurrentProduction();

                if (_consumption != 0)
                {
                    if (_consumption > 0)
                    {
                        // Netzbezug
                        return new PowerConsumptionProductionSplice()
                        {
                            Consumption = _consumption + _production,
                            Production = _production,
                            ProductionInternalUsage = _production,
                            FeedIn = 0,
                            NetSupply = _consumption
                        };
                    }
                    else
                    {
                        // Netzeinspeisung
                        return new PowerConsumptionProductionSplice()
                        {
                            Consumption = _production + _consumption,
                            Production = _production,
                            ProductionInternalUsage = _production + _consumption,
                            FeedIn = -_consumption,
                            NetSupply = 0
                        };
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }


        public override void SendMqttData()
        {
            if (_froniusSmartmeter != null && _mqttInterface != null && Topic != "" && Topic != null)
            {
                // send mqtt topics
                _mqttInterface.PublishTopic(Name, "Production", _production);
            }
        }
    }
}
