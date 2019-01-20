using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Breadboard
{

    // API object Lab
    public class Lab
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    // API object Run
    public class RunAPI
    {
        public string runtime { get; set; }
        public JObject parameters { get; set; }
        public int lab { get; set; }
    }


}
