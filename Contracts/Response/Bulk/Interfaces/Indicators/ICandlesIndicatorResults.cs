using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaapiLibrary.Models.Indicators.Results;

namespace TaapiLibrary.Contracts.Response.Bulk.Interfaces.Indicators;
public interface ICandlesIndicatorResults : ITaapiIndicatorResults {

    List<CandleIndicatorResults> Candles { get; set; }

}// interface
