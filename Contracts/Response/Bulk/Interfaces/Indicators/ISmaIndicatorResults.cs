﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Contracts.Response.Bulk.Interfaces.Indicators;
public interface ISmaIndicatorResults : ITaapiIndicatorResults {

    double? Value { get; set; }

}// interface