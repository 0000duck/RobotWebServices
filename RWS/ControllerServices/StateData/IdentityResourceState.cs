﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RWS.Data
{
    public class IdentityResourceState
    {
        [JsonProperty(PropertyName = "_type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "_title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "ctrl-name")]
        public string ControllerName { get; set; }

        [JsonProperty(PropertyName = "ctrl-id")]
        public string ControllerID { get; set; }
    }

}
