using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tungstenlabs.integration.zuvaai
{
    public class ZuvaRequestInfo
    {
        public string ZuvaFileID { get; set; }
        public string ZuvaToken { get; set; }
        public List<ZuvaFieldConfig> ZuvaFieldConfigCollection { get; set; }
    }

    public class ZuvaFieldConfig
    {
        public string ZuvaFieldIDs { get; set; }
        public short TransformationType { get; set; } //0-None, 1-First, 2-Concat all
        public short NormalizationType { get; set; } //1-Date, 2-Currency, 3-Durations
    }
}
