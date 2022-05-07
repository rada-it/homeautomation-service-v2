using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homeautomation_service.Evaluations
{
    public interface IGetConsumptionProductionData
    {
        public PowerConsumptionProductionSplice CalcConsumptionProductionData();
    }
    public class PowerConsumptionProductionSplice
    {
        public double Consumption { get; set; } // Verbrauch
        public double Production { get; set; } // Produktion PV
        public double ProductionInternalUsage { get; set; } // Eigenverbrauch
        public double FeedIn { get; set; } // Netzeinspeisung
        public double NetSupply { get; set; } // Netzbezug
    }

    public interface IGetConsumptionData
    {
        public PowerConsumptionSplice CalcConsumptionData();
    }
    public class PowerConsumptionSplice
    {
        public double Consumption { get; set; } // Verbrauch
        public double Total { get; set; } // Verbrauch kWh Total
        public DateTime TotalStartTime { get; set; } // Verbrauch kWh Total
        public bool Running { get; set; } // Verbrauch kWh
    }
}
