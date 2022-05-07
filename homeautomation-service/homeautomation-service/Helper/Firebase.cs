using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homeautomation_service.Helper
{
    internal class FirebaseConnector : ISaveData
    {
        private readonly FirebaseClient _firebase ;
        public FirebaseConnector()
        {
            _firebase = new FirebaseClient("https://homeautomation-68eea-default-rtdb.europe-west1.firebasedatabase.app");
        }

        public async Task InsertData(string device, object data)
        {
            try
            {
                await _firebase.Child(device).Child("data").Child(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()).PutAsync(data);
            }
            catch (Exception)
            {
                
            }
        }

        public async Task InsertRawData(string device, object data)
        {
            try
            {
                await _firebase.Child(device).Child("raw").Child(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()).PutAsync(data);
            }
            catch (Exception)
            {

            }
        }
    }

    public interface ISaveData
    {
        public Task InsertData(string device, object data);
        public Task InsertRawData(string device, object data);
    }

}


