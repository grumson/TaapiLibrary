using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaapiLibrary.Contracts.Response.Bulk;

namespace TaapiLibrary.Contracts.Response.Bulk.Interfaces;
public interface ITaapiIndicatorResults {

    string Id { get; set; }
    string Indicator { get; set; }
    List<string> Errors { get; set; }


}// interface
