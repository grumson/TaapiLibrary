using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Core.Defaults;
public static class ApiRateLimits {

    public const int RateLimitsFree = 1;
    public const int RateLimitsBasic = 5;
    public const int RateLimitsPro = 30;
    public const int RateLimitsExpert = 75;

    public const int ConstructLimitsFree = 0;
    public const int ConstructLimitsBasic = 1;
    public const int ConstructLimitsPro = 3;
    public const int ConstructLimitsExpert = 10;

    public const int MaxCalculations = 20;

}// class
