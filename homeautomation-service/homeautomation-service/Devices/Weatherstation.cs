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
    internal class WeatherStation : AbstractDevice, IHTMLParseResponse
    {
        private WeatherStationData _dataset = new(); // muss deklariert sein, weil es an den Parser gesendet wird
        private readonly Parser _parser;

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
       

        public WeatherStation(Device device, ISaveData saveData) : base(device, saveData)
        {
            if (Interval != 0)
            {
                _parser = new(_dataset, Interval, "http://192.168.0.199/livedata.htm");
                _parser.CyclicParseFromUrl(this);
            }
        }

        public override object CalcData()
        {
            if (_dataset == null)
            {
                return null;
            }
            else if (_dataset.OutTemp == 0)
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
            else if (_dataset.OutTemp == 0)
            {
                return null;
            }
            return _dataset;
        }

        public void ReceiveParserResponse(dynamic result)
        {
            if (_dataset == null)
            {
                _dataset = new();
            }
            _dataset = JsonConvert.DeserializeObject<WeatherStationData>(JsonConvert.SerializeObject(result));
        }


        public override void SendMqttData()
        {
            if (_dataset != null && _mqttInterface != null && Topic != "" && Topic != null)
            {
                // send mqtt topics
                _mqttInterface.PublishTopic(Name, "InTemp", _dataset.InTemp);
                _mqttInterface.PublishTopic(Name, "InHumi", _dataset.InHumi);
                _mqttInterface.PublishTopic(Name, "AbsPress", _dataset.AbsPress);
                _mqttInterface.PublishTopic(Name, "RelPress", _dataset.RelPress);
                _mqttInterface.PublishTopic(Name, "OutTemp", _dataset.OutTemp);
                _mqttInterface.PublishTopic(Name, "OutHumi", _dataset.OutHumi);
                _mqttInterface.PublishTopic(Name, "Windir", _dataset.Windir);
                _mqttInterface.PublishTopic(Name, "Avgwind", _dataset.Avgwind);
                _mqttInterface.PublishTopic(Name, "Gustspeed", _dataset.Gustspeed);
                _mqttInterface.PublishTopic(Name, "Solarrad", _dataset.Solarrad);
                _mqttInterface.PublishTopic(Name, "Uv", _dataset.Uv);
                _mqttInterface.PublishTopic(Name, "Uvi", _dataset.Uvi);
                _mqttInterface.PublishTopic(Name, "Rainofhourly", _dataset.Rainofhourly);
                _mqttInterface.PublishTopic(Name, "Rainofweekly", _dataset.Rainofweekly);
                _mqttInterface.PublishTopic(Name, "Rainofyearly", _dataset.Rainofyearly);
            }
        }
    }

    public class WeatherStationData
    {
        [HTMLParser(Name = "CurrTime")]
        [JsonProperty("CurrTime")]
        public string CurrTime { get; set; }

        [HTMLParser(Name = "inTemp")]
        [JsonProperty("InTemp")]
        public double InTemp { get; set; }

        [HTMLParser(Name = "inHumi")]
        [JsonProperty("InHumi")]
        public double InHumi { get; set; }

        [HTMLParser(Name = "AbsPress")]
        [JsonProperty("AbsPress")]
        public double AbsPress { get; set; }

        [HTMLParser(Name = "RelPress")]
        [JsonProperty("RelPress")]
        public double RelPress { get; set; }

        [HTMLParser(Name = "outTemp")]
        [JsonProperty("OutTemp")]
        public double OutTemp { get; set; }

        [HTMLParser(Name = "outHumi")]
        [JsonProperty("OutHumi")]
        public double OutHumi { get; set; }
        
        [HTMLParser(Name = "windir")]
        [JsonProperty("Windir")]
        public double Windir { get; set; }

        [HTMLParser(Name = "avgwind")]
        [JsonProperty("Avgwind")]
        public double Avgwind { get; set; }

        [HTMLParser(Name = "gustspeed")]
        [JsonProperty("Gustspeed")]
        public double Gustspeed { get; set; }

        [HTMLParser(Name = "solarrad")]
        [JsonProperty("Solarrad")]
        public double Solarrad { get; set; }

        [HTMLParser(Name = "uv")]
        [JsonProperty("Uv")]
        public double Uv { get; set; }

        [HTMLParser(Name = "uvi")]
        [JsonProperty("Uvi")]
        public double Uvi { get; set; }

        [HTMLParser(Name = "rainofhourly")]
        [JsonProperty("Rainofhourly")]
        public double Rainofhourly { get; set; }

        [HTMLParser(Name = "rainofdaily")]
        [JsonProperty("Rainofdaily")]
        public double Rainofdaily { get; set; }

        [HTMLParser(Name = "rainofweekly")]
        [JsonProperty("Rainofweekly")]
        public double Rainofweekly { get; set; }

        [HTMLParser(Name = "rainofmonthly")]
        [JsonProperty("Rainofmonthly")]
        public double Rainofmonthly { get; set; }

        [HTMLParser(Name = "rainofyearly")]
        [JsonProperty("Rainofyearly")]
        public double Rainofyearly { get; set; }
    }
}
