using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Contracts.Requests.Interfaces.Indicators;
public interface IMacdIndicatorProperties : ITaapiIndicatorProperties {

    int? OptInFastPeriod { get; set; }
    int? OptInSlowPeriod { get; set; }
    int? OptInSignalPeriod { get; set; }

}// interface
