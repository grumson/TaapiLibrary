using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Contracts.Response.Bulk.Interfaces.Indicators;
public interface IStochRsiIndicatorResults : ITaapiIndicatorResults {

    double? ValueFastK { get; set; }
    double? ValueFastD { get; set; }

}// interface
