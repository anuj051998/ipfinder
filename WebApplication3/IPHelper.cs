using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace WebApplication3
{


    public static class Extension
    {
        private static string GetMACAddress()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                    return AddressBytesToString(nic.GetPhysicalAddress().GetAddressBytes());
            }

            return string.Empty;
        }

        private static string AddressBytesToString(byte[] addressBytes)
        {
            return string.Join(":", (from b in addressBytes
                                     select b.ToString("X2")).ToArray());
        }
        public static void Get_MAC_Address(this MyDictionary vs)
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces().Where(x => x.OperationalStatus == OperationalStatus.Up).Select(x => x.GetIPProperties());
            vs.Add("MAC_ADDRESS", GetMACAddress());
            foreach (var item in NetworkInterface.GetAllNetworkInterfaces())
            {
                vs.Add(item.Name, item.Description);
            }
            foreach (var iface in interfaces)
            {
                foreach (var x in iface.DnsAddresses)
                {
                    vs.Add($"DNS", x.ToString());
                }
                foreach (var x in iface.WinsServersAddresses)
                {
                    vs.Add($"WIN_SERVER_ADDRESS", x.ToString());
                }
                foreach (var x in iface.AnycastAddresses)
                {
                    vs.Add($"ANYCAST", x.ToString());
                }
                foreach (var x in iface.DhcpServerAddresses)
                {
                    vs.Add($"DHCP", x.ToString());
                }
                foreach (var x in iface.GatewayAddresses)
                {
                    vs.Add($"GATEWAY", x.Address.ToString());
                }
                foreach (var x in iface.MulticastAddresses)
                {
                    vs.Add($"MULTI_CAST", x.Address.ToString());
                }
                foreach (var x in iface.UnicastAddresses)
                {
                    vs.Add($"UNI_CAST", x.Address.ToString());
                }
            }
        }
        public static void LoadRawHttp(this MyDictionary vs, HttpContext httpContext)
        {
            string allHttp = httpContext.GetServerVariable("ALL_HTTP")?.ToString();
            if (!string.IsNullOrEmpty(allHttp))
            {
                var arr = allHttp.Split("\n").Where(x => x.Contains(':'));
                foreach (var x in arr)
                {
                    string[] res = x.Split(':');
                    vs.Add(res[0], res[1]);
                }
            }
            allHttp = httpContext.GetServerVariable("ALL_RAW")?.ToString();
            if (!string.IsNullOrEmpty(allHttp))
            {
                var arr = allHttp.Split("\n").Where(x => x.Contains(':'));
                foreach (var x in arr)
                {
                    string[] res = x.Split(':');
                    vs.Add(res[0], res[1]);
                }
            }
        }
        public static void LoadServerVariables(this MyDictionary vs, HttpContext httpContext)
        {
            string[] headerKeys = new string[] {
                "REMOTE_ADDR",
                "HTTP_X_FORWARDED_FOR",
                "REMOTE_USER",
                "REMOTE_PORT",
                "REMOTE_HOST",
                "LOGON_USER",
                "LOCAL_ADDR",
                "SERVER_ADDR"
            };
            foreach (var key in headerKeys)
            {
                vs.Add(key, httpContext.GetServerVariable(key)?.ToString() ?? "");
            }
        }
        public static void DnsData(this MyDictionary vs, HttpContext httpContext)
        {
            foreach (var key in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                vs.Add(new Random().Next().ToString(), key.ToString());
            }
            vs.Add("GetHostName", Dns.GetHostName());
            vs.Add("HostEntryRemote", Dns.GetHostEntry(Dns.GetHostName()).HostName);
            try
            {
                vs.Add("HostEntryRemoteIP", Dns.GetHostEntry(httpContext.GetServerVariable("REMOTE_ADDR")).HostName);
            }
            catch(Exception ex)
            {
                vs.Add("HostEntryRemoteIP_EXC", ex.Message);
            }
            vs.Add("HostEntryRemoteOne", Dns.GetHostEntry(httpContext.Connection.LocalIpAddress).HostName);
            var remote_ip = httpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
            vs.Add("remoteIP", remote_ip);
            vs.Add("LOCAL_IP_ADDRESS", httpContext.Connection.LocalIpAddress.ToString());
            vs.Add("REMOTE_IP_ADDRESS", httpContext.Connection.RemoteIpAddress.ToString());
            int index = 0;
            foreach (var item in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                vs.Add($"AddressList-{index++}", Convert.ToBase64String(item.GetAddressBytes()));
            }
            try
            {
                vs.Add("DNS_HOST_ENTRY", Dns.GetHostEntry(httpContext.GetServerVariable("REMOTE_ADDR").ToString()).HostName);
            }
            catch { }
            try
            {
                vs.Add("RESOLVE", string.Join(',', Dns.Resolve(Dns.GetHostName()).AddressList.Select(x => Convert.ToBase64String(x.GetAddressBytes()))));
            }
            catch { }
            try
            {
                vs.Add("DNS_HOST_ENTRY_1", Dns.GetHostByAddress(httpContext.GetServerVariable("REMOTE_ADDR").ToString()).HostName);
            }
            catch { }
        }

        public static void LoadRequestHeader(this MyDictionary vs, HttpRequest httpRequest)
        {
            int index = 0;
            foreach (var item in httpRequest.Headers)
            {
                vs.Add($"{item.Key} - {index++}", item.Value.ToString());
            }
        }
        public static void LoadResponseHeader(this MyDictionary vs, HttpResponse httpResponse)
        {
            int index = 0;
            foreach (var item in httpResponse.Headers)
            {
                vs.Add($"{item.Key} - {index++}", item.Value.ToString());
            }
        }
    }
}
