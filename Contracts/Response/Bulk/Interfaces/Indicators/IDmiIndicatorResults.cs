using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Contracts.Response.Bulk.Interfaces.Indicators;
public interface IDmiIndicatorResults : ITaapiIndicatorResults {

    double? Adx { get; set; }
    double? Pdi { get; set; }
    double? Mdi { get; set; }

}// interface
