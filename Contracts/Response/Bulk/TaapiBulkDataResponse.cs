using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Contracts.Response.Bulk;
public class TaapiBulkDataResponse
{

    public string id { get; set; } = string.Empty;
    public string indicator { get; set; } = string.Empty;
    public TaapiIndicatorValuesResponse result { get; set; } = null!;
    public List<string> errors { get; set; } = new List<string>();

}// class
