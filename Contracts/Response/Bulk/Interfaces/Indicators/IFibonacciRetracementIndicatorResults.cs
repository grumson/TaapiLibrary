using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Contracts.Response.Bulk.Interfaces.Indicators;
public interface IFibonacciRetracementIndicatorResults : ITaapiIndicatorResults {

    double? Value { get; set; }
    public string? Trend { get; set; }
    public double? StartPrice { get; set; }
    public double? EndPrice { get; set; }
    public string? StartTimestamp { get; set; }
    public string? EndTimestamp { get; set; }

}// interface
