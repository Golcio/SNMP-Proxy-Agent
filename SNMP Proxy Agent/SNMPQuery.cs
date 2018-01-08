using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMP_Proxy_Agent
{
    [Serializable]
    public class SNMPQuery
    {
        public string message;
        public SNMPQuery(string msg)
        {
            message = msg;
        }
    }
}
