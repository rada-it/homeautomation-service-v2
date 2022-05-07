using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homeautomation_service.Evaluations
{
    public interface IGetTemperatureData
    {
        public TemperatureData CalcTemperatureData();
    }
    public class TemperatureData
    {
        public double Temperature { get; set; }
        public double Humidity { get; set; }
    }
}
