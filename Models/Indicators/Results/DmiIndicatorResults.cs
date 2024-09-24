using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaapiLibrary.Contracts.Response.Bulk.Interfaces.Indicators;

namespace TaapiLibrary.Models.Indicators.Results;
public class DmiIndicatorResults : IDmiIndicatorResults {


    #region *** PROPERTIES ***

    public string Id { get; set; } = string.Empty;
    public string Indicator { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new List<string>();
    public double? Adx { get; set; }
    public double? Pdi { get; set; }
    public double? Mdi { get; set; }

    #endregion


}// class
