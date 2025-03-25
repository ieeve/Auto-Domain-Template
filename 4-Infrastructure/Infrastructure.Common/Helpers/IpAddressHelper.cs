using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common.Helpers
{
    public static class IpAddressHelper
    {
        public static List<string> GetIpV4()
        {
            //取得所有IPV4地址
            List<string> IPV4 = new List<string>();
            string name = Dns.GetHostName();
            IPAddress[] ipadrlist = Dns.GetHostAddresses(name);
            foreach (IPAddress ipa in ipadrlist)
            {
                // IPv4，返回 InterNetwork；对于 IPv6，返回 InterNetworkV6
                if (ipa.AddressFamily == AddressFamily.InterNetwork)
                    IPV4.Add(ipa.ToString());
            }
            return IPV4;
        }
    }
}
