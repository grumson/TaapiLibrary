﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Contracts.Requests.Interfaces.Indicators;
public interface IMaIndicatorProperties : ITaapiIndicatorProperties {

    int? Period { get; set; }

}// interface
