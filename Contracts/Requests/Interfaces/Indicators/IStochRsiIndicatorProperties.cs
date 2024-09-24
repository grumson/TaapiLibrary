using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Contracts.Requests.Interfaces.Indicators;
public interface IStochRsiIndicatorProperties : ITaapiIndicatorProperties {

    int? KPeriod { get; set; }
    int? DPeriod { get; set; }
    int? RsiPeriod { get; set; }
    int? StochasticPeriod { get; set; }

}// interface
