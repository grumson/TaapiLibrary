using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaapiLibrary.Models.Indicators.Results;

namespace TaapiLibrary.Contracts.Response.Bulk.Interfaces.Indicators;
public interface ISuperTrendIndicatorResults : ITaapiIndicatorResults {

    double? Value { get; set; }
    string? ValueAdvice { get; set; }

}// interface
