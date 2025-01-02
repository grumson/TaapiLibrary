using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Contracts.Requests.Interfaces.Indicators;
public interface IFibonacciRetracementIndicatorProperties : ITaapiIndicatorProperties {

    int? Period { get; set; }
    public float? Retracement { get; set; }
    public string? Trend { get; set; }

}// interface
