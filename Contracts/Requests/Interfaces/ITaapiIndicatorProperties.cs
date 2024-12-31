using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaapiLibrary.Enums;

namespace TaapiLibrary.Contracts.Requests.Interfaces;
public interface ITaapiIndicatorProperties {

    string Id { get; set; }
    TaapiIndicatorType Indicator { get; }
    TaapiChart Chart { get; set; }
    int? Backtrack { get; set; }
    bool? ChartGaps { get; set; }
    string? Results { get; set; }
    bool? AddResultTimestamp { get; set; }

    string? FromTimestamp { get; set; }
    string? ToTimestamp { get; set; }


}// interface
