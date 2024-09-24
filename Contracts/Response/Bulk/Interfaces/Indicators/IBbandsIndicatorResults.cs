using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Contracts.Response.Bulk.Interfaces.Indicators;
public interface IBbandsIndicatorResults : ITaapiIndicatorResults {

    double? ValueUpperBand { get; set; }
    double? ValueMiddleBand { get; set; }
    double? ValueLowerBand { get; set; }

}// interface
