using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Contracts.Response.Bulk.Interfaces.Indicators;
public interface ICandleIndicatorResults : ITaapiIndicatorResults {

    double? Volume { get; set; }
    string? TimestampHuman { get; set; }
    int? Timestamp { get; set; }
    double? Open { get; set; }
    double? Close { get; set; }
    double? High { get; set; }
    double? Low { get; set; }

}// interface
