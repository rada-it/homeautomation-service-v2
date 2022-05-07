using homeautomation_service.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homeautomation_service.Devices
{

    internal class FroniusTotal : IRestInjectorResponse
    {
        private FroniusSummaryData _dataset ;
        //private readonly IRestInjectorResponse _restResponse;

        //public override dynamic Data { get { return _dataset; } set { _dataset = value; } }

        public FroniusTotal(IRestInjector restApi)
        {
            restApi.CyclicRestApiCall(this);
        }
        public void ReceiveRestResponse(object result)
        {
            if (_dataset == null)
            {
                _dataset = new();
            }
            _dataset = JsonConvert.DeserializeObject<FroniusSummaryData>(JsonConvert.SerializeObject(result)); 
        }
        public double GetCurrentProduction()
        {
            if (_dataset == null)
            {
                return 0;
            }
            if (_dataset.Body.Data.PAC.Values != null)
            {
                double currentProduction = 0;
                foreach (KeyValuePair<string, int> entry in _dataset.Body.Data.PAC.Values)
                {
                    currentProduction += entry.Value;
                }
                return currentProduction;
            }
            else
            {
                return 0;
            }


        }
        public object GetRawData()
        {
            if (_dataset == null)
            {
                return null;
            }
            return _dataset;
        }
    }

    class FroniusSummaryData
    {
        [JsonProperty("Body")]
        public FroniusSummaryDataBody? Body { get; set; } = new();
    }
    class FroniusSummaryDataBody
    {
        [JsonProperty("Data")]
        public FroniusSummaryDataBodyData? Data { get; set; } = new();
    }
    class FroniusSummaryDataBodyData
    {
        [JsonProperty("DAY_ENERGY")]
        public FroniusSummaryDataBodyDataUnitValues? DAY_ENERGY { get; set; } = new();
        [JsonProperty("PAC")]
        public FroniusSummaryDataBodyDataUnitValues? PAC { get; set; } = new();
        [JsonProperty("TOTAL_ENERGY")]
        public FroniusSummaryDataBodyDataUnitValues? TOTAL_ENERGY { get; set; } = new();
        [JsonProperty("YEAR_ENERGY")]
        public FroniusSummaryDataBodyDataUnitValues? YEAR_ENERGY { get; set; } = new();
    }
    class FroniusSummaryDataBodyDataUnitValues
    {
        [JsonProperty("Unit")]
        public string? Unit { get; set; }
        [JsonProperty("Values")]
        public Dictionary<string, int> Values { get; set; }
    }


}
