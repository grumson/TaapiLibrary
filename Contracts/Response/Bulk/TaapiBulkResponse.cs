using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Contracts.Response.Bulk;
public class TaapiBulkResponse
{


    public List<TaapiBulkDataResponseRow> data { get; set; } = null!;


}// class
