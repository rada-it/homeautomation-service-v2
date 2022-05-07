// See https://aka.ms/new-console-template for more information
using Firebase.Database;
using Firebase.Database.Query;
using Google.Cloud.Firestore;
using homeautomation_service;
using homeautomation_service.Devices;
using homeautomation_service.Helper;


Console.WriteLine("Starting HomeAutomation Service");

FirebaseConnector firebase = new();
Configuration configuration = new(firebase);
MQTT client = new(configuration);
configuration.HandOverMqttInterface(client);

Console.WriteLine("Init ready");
Console.ReadLine();
