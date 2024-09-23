using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaapiLibrary.Enums;

namespace TaapiLibrary.Contracts.Requests.Interfaces;
public interface ITaapiIndicatorRequest {

    string Id { get; set; }
    TaapiIndicatorType Indicator { get; set; }
    TaapiChart Chart { get; set; }
    string? Backtrack { get; set; }
    bool? ChartGaps { get; set; }
    string? Results { get; set; }

}// interface
