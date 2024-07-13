using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaapiLibrary.Enums;

namespace TaapiLibrary.Contracts.Requests;
public class TaapiBulkRequest
{


    #region *** PROPERTIES ***

    [JsonProperty("secret")]
    public string Secret { get; private set; }


    // List of constructs to be requested for this bulk request
    [JsonProperty("construct")]
    public List<TaapiBulkConstruct> Construct { get; private set; }

    #endregion



    #region *** CONSTRUCTORS ***
    public TaapiBulkRequest(string secret, List<TaapiBulkConstruct> bulkConstructList)
    {

        // check if the secret is null or empty
        if (string.IsNullOrEmpty(secret))
        {
            throw new ArgumentException("The secret cannot be null or empty.");
        }
        Secret = secret;

        // check if the list is null or empty
        if (bulkConstructList == null || bulkConstructList.Count == 0)
        {
            throw new ArgumentException("The list of constructs cannot be null or empty.");
        }

        Construct = bulkConstructList;

    }
    #endregion


}// class
