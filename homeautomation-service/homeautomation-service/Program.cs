// See https://aka.ms/new-console-template for more information
using Firebase.Database;
using Firebase.Database.Query;
using Google.Cloud.Firestore;
using homeautomation_service;
using homeautomation_service.Devices;
using homeautomation_service.Helper;
using System.Globalization;

Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");

Console.WriteLine("Starting HomeAutomation Service");

FirebaseConnector firebase = new();
Configuration configuration = new(firebase);
MQTT client = new(configuration);
configuration.HandOverMqttInterface(client);

//File.WriteAllText("log.txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": Service started");
Console.WriteLine("Init ready");

await Task.Delay(-1);
