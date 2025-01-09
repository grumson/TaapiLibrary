using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaapiLibrary.Core.Models;

namespace TaapiLibrary.Services;
public interface ITaapiClient {


    Task<IndicatorResponse> GetIndicatorAsync(IndicatorRequest request);
    Task<BulkResponse> GetBulkAsync(BulkRequest requestBulk);
    Task<bool> TestConnectionAsync();


}// interface
