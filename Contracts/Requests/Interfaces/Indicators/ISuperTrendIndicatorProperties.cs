using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Contracts.Requests.Interfaces.Indicators;
public interface ISuperTrendIndicatorProperties : ITaapiIndicatorProperties {

    int? Period { get; set; }
    int? Multiplier { get; set; }

}// interface
