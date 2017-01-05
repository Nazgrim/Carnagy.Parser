using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontendApi.Models
{
    public class PBIReport
    {
        public string Id { get; set; }
        public string displayName { get; set; }
        public string webUrl { get; set; }
        public string embedUrl { get; set; }
    }
}
