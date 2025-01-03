using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Contracts.Requests.Interfaces.Indicators;
public interface IStandardDeviationIndicatorProperties : ITaapiIndicatorProperties {

    int? Period { get; set; }
    float? OptInNbDev { get; set; }

}// interface
