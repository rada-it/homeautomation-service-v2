using homeautomation_service.Devices;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Newtonsoft.Json;
using System.Text;

namespace homeautomation_service
{
    internal class MQTT : IMQTTPublisher
    {
        //private static object _lockObj;
        private MqttFactory _mqttFactory = new MqttFactory();

        private IMqttClient _mqttClient;
        private IGetDevice _devices;
        private bool _lockInit = false;

        public MQTT(IGetDevice devices)
        {
            InitMQTT(devices);
        }

        private async Task InitMQTT(IGetDevice devices)
        {
            if (!_lockInit)
            {
                _lockInit = true;
                _devices = devices;
                _mqttClient = _mqttFactory.CreateMqttClient();

                var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("192.168.0.195", 1883).WithClientId("ServiceV2").WithCredentials("openhabian", "mqttredlham123").WithCleanSession(true).Build();
                var response = await _mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

                // Start Watcher
                _mqttClient.UseApplicationMessageReceivedHandler(e =>
                {
#if DEBUG
                    //Console.WriteLine("----------------------------------------------------------------");
                    //Console.WriteLine($"Topic={e.ApplicationMessage.Topic}");
                    //Console.WriteLine($"Payload={Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
#endif
                    // get device
                    if (devices.DeviceExisting(e.ApplicationMessage.Topic))
                    {
                        try
                        {
                            devices.SetData(e.ApplicationMessage.Topic, Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                        }
                        catch (Exception ex)
                        {
                            ;
                        }
                    }
                    //Console.WriteLine("----------------------------------------------------------------");
                });

                Console.WriteLine("The MQTT client is connected.");

                // Subscribe
                foreach (AbstractDevice device in devices.GetDevices().Where(x => x.Topic != ""))
                {
                    var mqttSubscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder()
                           .WithTopicFilter(f => { f.WithTopic(device.Topic); })
                           .Build();
                    await _mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

                    Console.WriteLine("MQTT client subscribed to topic " + device.Topic);
                }

                Console.WriteLine("All topics subscribed.");
                _lockInit = false;
            }
        }

        private async void Publish(string topic, dynamic content)
        {
            try
            {
                var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(JsonConvert.SerializeObject(content))
                .Build();
                await _mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Thread.Sleep(2000);
                /*lock (_lockObj)
                {
                    File.WriteAllText("log.txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": "+ ex.ToString());
                }*/
                if (ex.ToString().Contains("is not connected") || ex.ToString().Contains("Eine bestehende Verbindung"))
                {
                    InitMQTT(_devices);
                }
                else
                {
                    ;
                }
            }
        }

        public void PublishTopic(string name, string topic, dynamic content)
        {
            var topicName = name + '_' + topic;
            Publish(topicName, content);
        }

        /*public void PublishObject(string name, dynamic content)
        {
            if (content != null && name != null && name != "")
            {
                foreach (var prop in content.GetType().GetProperties())
                {
                    try
                    {
                        object[] attrs = prop.GetCustomAttributes(true);
                        foreach (object attr in attrs)
                        {
                            if (attr is MQTTAttribute ownAttr)
                            {
                                var topicName = name + '_' + ownAttr.Name;
                                Publish(topicName, prop.GetValue(content));
                                ;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        ;
                    }
                }
            }
        }*/
    }

    internal class MQTTAttribute : Attribute
    {
        public string Name { get; set; }
    }

    public interface IMQTTPublisher
    {
        public void PublishTopic(string name, string topic, dynamic content);

        //public void PublishObject(string name, object data);
    }
}