using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Contracts.Response.Bulk.Interfaces.Indicators;
public interface IMacdIndicatorResults : ITaapiIndicatorResults {

    double? ValueMACD { get; set; }
    double? ValueMACDSignal { get; set; }
    double? ValueMACDHist { get; set; }

}// interface
