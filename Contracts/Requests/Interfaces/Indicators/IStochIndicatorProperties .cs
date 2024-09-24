using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Contracts.Requests.Interfaces.Indicators;
public interface IStochIndicatorProperties : ITaapiIndicatorProperties {

    int? KPeriod { get; set; }

    int? DPeriod { get; set; }

    int? KSmooth { get; set; }

}// interface
