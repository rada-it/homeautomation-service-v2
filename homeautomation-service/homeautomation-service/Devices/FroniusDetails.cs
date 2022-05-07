using homeautomation_service.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homeautomation_service.Devices
{
    internal class FroniusDetails : IRestInjectorResponse
    {
        private FroniusSummaryDataDetails _dataset ;

        //public override dynamic Data { get { return _dataset; } set { _dataset = value; } }

        public FroniusDetails(IRestInjector restApi)
        {
            restApi.CyclicRestApiCall(this);
        }

        public void ReceiveRestResponse(object result)
        {
            if (_dataset == null)
            {
                _dataset = new();
            }
            _dataset = JsonConvert.DeserializeObject<FroniusSummaryDataDetails>(JsonConvert.SerializeObject(result));
        }
        
        public double GetCurrentConsumption()
        {
            if (_dataset == null)
            {
                return 0;
            }
            if (_dataset.Body.Data != null)
            {
                    // negative ==> Netzeinspeisung
                    double currentConsumption = 0;
                foreach (KeyValuePair<int, FroniusSummaryDataBodyDetail> entry in _dataset.Body.Data)
                {
                    currentConsumption += entry.Value.PowerReal_P_Sum;
                }
                return currentConsumption;
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

   
    class FroniusSummaryDataDetails
    {
        [JsonProperty("Body")]
        public FroniusSummaryDataBodyDetails? Body { get; set; } = new();
    }
    
    class FroniusSummaryDataBodyDetails
    {
        [JsonProperty("Data")]
        public Dictionary<int, FroniusSummaryDataBodyDetail>? Data { get; set; } 
    }
    
    class FroniusSummaryDataBodyDetail
    {
        [JsonProperty("Current_AC_Phase_1")]
        public double Current_AC_Phase_1 { get; set; } 
        [JsonProperty("Current_AC_Phase_2")]
        public double Current_AC_Phase_2 { get; set; }
        [JsonProperty("Current_AC_Phase_3")]
        public double Current_AC_Phase_3 { get; set; }
        [JsonProperty("Details")]
        public FroniusSummaryDataBodyDetailDetails? Details { get; set; }
        [JsonProperty("Enable")]
        public int Enable { get; set; }
        [JsonProperty("EnergyReactive_VArAC_Sum_Consumed")]
        public double EnergyReactive_VArAC_Sum_Consumed { get; set; }
        [JsonProperty("EnergyReactive_VArAC_Sum_Produced")]
        public double EnergyReactive_VArAC_Sum_Produced { get; set; }
        [JsonProperty("EnergyReal_WAC_Minus_Absolute")]
        public double EnergyReal_WAC_Minus_Absolute { get; set; }
        [JsonProperty("EnergyReal_WAC_Plus_Absolute")]
        public double EnergyReal_WAC_Plus_Absolute { get; set; }
        [JsonProperty("EnergyReal_WAC_Sum_Consumed")]
        public double EnergyReal_WAC_Sum_Consumed { get; set; }
        [JsonProperty("EnergyReal_WAC_Sum_Produced")]
        public double EnergyReal_WAC_Sum_Produced { get; set; }
        [JsonProperty("Frequency_Phase_Average")]
        public double Frequency_Phase_Average { get; set; }
        [JsonProperty("Meter_Location_Current")]
        public double Meter_Location_Current { get; set; }
        [JsonProperty("PowerApparent_S_Phase_1")]
        public double PowerApparent_S_Phase_1 { get; set; }
        [JsonProperty("PowerApparent_S_Phase_2")]
        public double PowerApparent_S_Phase_2 { get; set; }
        [JsonProperty("PowerApparent_S_Phase_3")]
        public double PowerApparent_S_Phase_3 { get; set; }
        [JsonProperty("PowerApparent_S_Sum")]
        public double PowerApparent_S_Sum { get; set; }
        [JsonProperty("PowerFactor_Phase_1")]
        public double PowerFactor_Phase_1 { get; set; }
        [JsonProperty("PowerFactor_Phase_2")]
        public double PowerFactor_Phase_2 { get; set; }
        [JsonProperty("PowerFactor_Phase_3")]
        public double PowerFactor_Phase_3 { get; set; }
        [JsonProperty("PowerFactor_Sum")]
        public double PowerFactor_Sum { get; set; }
        [JsonProperty("PowerReactive_Q_Phase_1")]
        public double PowerReactive_Q_Phase_1 { get; set; }
        [JsonProperty("PowerReactive_Q_Phase_2")]
        public double PowerReactive_Q_Phase_2 { get; set; }
        [JsonProperty("PowerReactive_Q_Phase_3")]
        public double PowerReactive_Q_Phase_3 { get; set; }
        [JsonProperty("PowerReactive_Q_Sum")]
        public double PowerReactive_Q_Sum { get; set; }
        [JsonProperty("PowerReal_P_Phase_1")]
        public double PowerReal_P_Phase_1 { get; set; }
        [JsonProperty("PowerReal_P_Phase_2")]
        public double PowerReal_P_Phase_2 { get; set; }
        [JsonProperty("PowerReal_P_Phase_3")]
        public double PowerReal_P_Phase_3 { get; set; }
        [JsonProperty("PowerReal_P_Sum")]
        public double PowerReal_P_Sum { get; set; }
        [JsonProperty("TimeStamp")]
        public int TimeStamp { get; set; }
        [JsonProperty("Visible")]
        public int Visible { get; set; }
        [JsonProperty("Voltage_AC_PhaseToPhase_12")]
        public double Voltage_AC_PhaseToPhase_12 { get; set; }
        [JsonProperty("Voltage_AC_PhaseToPhase_23")]
        public double Voltage_AC_PhaseToPhase_23 { get; set; }
        [JsonProperty("Voltage_AC_PhaseToPhase_31")]
        public double Voltage_AC_PhaseToPhase_31 { get; set; }
        [JsonProperty("Voltage_AC_Phase_1")]
        public double Voltage_AC_Phase_1 { get; set; }
        [JsonProperty("Voltage_AC_Phase_2")]
        public double Voltage_AC_Phase_2 { get; set; }
        [JsonProperty("Voltage_AC_Phase_3")]
        public double Voltage_AC_Phase_3 { get; set; }
    }
    class FroniusSummaryDataBodyDetailDetails
    {
        [JsonProperty("Manufacturer")]
        public string? Manufacturer { get; set; }
        [JsonProperty("Model")]
        public string? Model { get; set; }
        [JsonProperty("Serial")]
        public string? Serial { get; set; }
    }


}
