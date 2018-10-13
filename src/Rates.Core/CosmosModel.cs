using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rates.Core
{
    public abstract class CosmosModel
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}
