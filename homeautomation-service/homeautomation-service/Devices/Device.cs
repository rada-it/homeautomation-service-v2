using homeautomation_service.Helper;
using Newtonsoft.Json;

namespace homeautomation_service.Devices
{
    internal class Device : AbstractDevice
    {
        private bool _dataset = new();
        public override dynamic Data
        { get { return _dataset; } set { _dataset = value; } }

        public Device(Device device, ISaveData saveData) : base(device, saveData)
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
            ;
        }
    }

    internal abstract class AbstractDevice
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("SaveRawData")]
        public bool SaveRawData { get; set; }
        [JsonProperty("Classification")]
        public int Classification { get; set; }
        [JsonProperty("Interval")]
        public int Interval { get; set; }
        [JsonProperty("Topic")]
        public string Topic { get; set; }

        public virtual dynamic Data { get; set; }

        private readonly ISaveData _saveData;
        protected IMQTTPublisher _mqttInterface;
        private readonly Timer _stateTimer;

        public AbstractDevice(Device device, ISaveData saveData)
        {
            if (device != null)
            {
                Name = device.Name;
                Classification = device.Classification;
                Interval = device.Interval;
                Topic = device.Topic;
                SaveRawData = device.SaveRawData;

                //_mqttInterface = mqttInterface;
                _saveData = saveData;
            }

            if (Interval != null)
            {
                if (Interval < 60)
                {
                    Interval = 60;
                }
                _stateTimer = new(SendData, null, 0, 1000 * Interval);
            }
        }
        public void HandoverMqttInterface(IMQTTPublisher mqttInterface)
        {
            _mqttInterface = mqttInterface;
        }

        private object _oldData;
        private DateTime _lastTimeDataSent;

        public void SendData(object state)
        {
            // calc
            var calcData = CalcData();
            var rawData = GetRawData();
            var newData = false;

            if (!(calcData is bool) && calcData != null && Topic != "")
            {
                if (_oldData == null)
                {
                    // new data
                    Console.WriteLine(Name + "(" + Topic + "): send calced data to DB");
                    Console.WriteLine(calcData);

                    _saveData.InsertData(Name, calcData);
                    if (SaveRawData)
                    {
                        _saveData.InsertRawData(Name, rawData);
                    }
                    newData = true;
                    _lastTimeDataSent = DateTime.Now;
                }
                else if ((JsonConvert.SerializeObject(calcData) != JsonConvert.SerializeObject(_oldData)) 
                    || _lastTimeDataSent < DateTime.Now.AddMinutes(60))
                {
                    // new data
                    Console.WriteLine(Name + "(" + Topic + "): send calced data to DB");
                    Console.WriteLine(calcData);

                    _saveData.InsertData(Name, calcData);
                    if (SaveRawData)
                    {
                        _saveData.InsertRawData(Name, rawData);
                    }
                    newData = true;
                    _lastTimeDataSent = DateTime.Now;
                }
                else
                {
                    Console.WriteLine(Name + "(" + Topic + "): data equal");
                }
                _oldData = calcData;
            }

            SendMqttData();
            /*if (newData && _mqttInterface != null && Topic != "" && Topic != null)
            {
                // send mqtt topics
                _mqttInterface.PublishObject(Name,calcData);
                _mqttInterface.PublishObject(Name,rawData);
            }*/
        }

        public abstract object CalcData();
        public abstract object GetRawData();
        public abstract void SendMqttData();
    }
}