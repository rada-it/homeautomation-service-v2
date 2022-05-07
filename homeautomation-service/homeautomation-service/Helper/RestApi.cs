using Newtonsoft.Json;

namespace homeautomation_service.Helper
{
    internal class RestApi : IRestInjector
    {
        private readonly string _url;
        private readonly int _interval;
        private IRestInjectorResponse _restInjector;
        Timer _stateTimer;

        public RestApi(string url, int intervalSec)
        {
            _url = url;
            _interval = intervalSec;
        }

        public string GetUrl()
        {
            return _url;
        }
        public dynamic CallRestApi()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_url);
                /*client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));*/
                var response = client.GetStringAsync(_url);
                return response;
            }
        }

        private void CallRestApiWithInjection(object state)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_url);
                /*client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));*/
                var response = client.GetStringAsync(_url).Result;
                if (response != null)
                {
                    _restInjector.ReceiveRestResponse(JsonConvert.DeserializeObject<object>(response));
                }
            }
        }

        public void CyclicRestApiCall(IRestInjectorResponse restInjector)
        {
            _restInjector = restInjector;
            _stateTimer = new(CallRestApiWithInjection, null, 0, 1000 * _interval);
        }
    }

    public interface IRestInjector
    {
        public void CyclicRestApiCall(IRestInjectorResponse restInjector);
        public string GetUrl();
    }

    public interface IRestInjectorResponse
    {
        public void ReceiveRestResponse(dynamic result);
    }
}