using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorEssentials.Models
{
    public class ClientUserAgent
    {
        public string AppInfo { get; set; }
        public string PlatformInfo { get; set; }
        public string ProductInfo { get; set; }
        public string ApplicationNameInfo { get; set; }

        public OSType GetOSType()
        {
            if (!String.IsNullOrEmpty(PlatformInfo))
            {
                if (PlatformInfo.ToLower().Contains("Macintosh".ToLower()) || (PlatformInfo.ToLower().Contains("Mac OS".ToLower()) && (!PlatformInfo.ToLower().Contains("iPhone".ToLower()) || !PlatformInfo.ToLower().Contains("iPad".ToLower()) || !PlatformInfo.ToLower().Contains("iOS".ToLower()))))
                {
                    return OSType.MacOS;
                }
                else if (PlatformInfo.ToLower().Contains("Windows".ToLower()))
                {
                    return OSType.Windows;
                }
                else if (PlatformInfo.ToLower().Contains("Linux".ToLower()) && (!PlatformInfo.ToLower().Contains("Android".ToLower())))
                {
                    return OSType.Linux;
                }
                else if (PlatformInfo.ToLower().Contains("Linux".ToLower()) && (PlatformInfo.ToLower().Contains("Android".ToLower())))
                {
                    return OSType.Android;
                }
                else if (PlatformInfo.ToLower().Contains("iPhone".ToLower()) || PlatformInfo.ToLower().Contains("iPad".ToLower()) || PlatformInfo.ToLower().Contains("iOS".ToLower()))
                {
                    return OSType.iOS;
                }
                else
                {
                    return OSType.Other;
                }
            }
            else
            {
                return OSType.Other;
            }
        }

        public ArchitectureType GetArchitectureType()
        {
            if (!String.IsNullOrEmpty(PlatformInfo))
            {
                if (PlatformInfo.ToLower().Contains("Macintosh".ToLower()) || PlatformInfo.ToLower().Contains("Mac OS".ToLower()))
                {
                    return ArchitectureType.X64;
                }
                else if (PlatformInfo.ToLower().Contains("Windows".ToLower()))
                {
                    if (PlatformInfo.ToLower().Contains("x64".ToLower()))
                    {
                        return ArchitectureType.X64;
                    }
                    else if (PlatformInfo.ToLower().Contains("x86".ToLower()))
                    {
                        return ArchitectureType.X86;
                    }
                    else
                    {
                        return ArchitectureType.Unknown;
                    }
                }
                else if (PlatformInfo.ToLower().Contains("Linux".ToLower()) && (!PlatformInfo.ToLower().Contains("Android".ToLower())))
                {
                    if (PlatformInfo.ToLower().Contains("Linux x86_64".ToLower()))
                    {
                        return ArchitectureType.X64;
                    }
                    else if (PlatformInfo.ToLower().Contains("Linux i686".ToLower()))
                    {
                        return ArchitectureType.X86;
                    }
                    else
                    {
                        return ArchitectureType.Unknown;
                    }
                }
                else
                {
                    return ArchitectureType.Unknown;
                }
            }
            else
            {
                return ArchitectureType.Unknown;
            }
        }

        public ClientUserAgent()
        {

        }


    }

    public static class ClientUserAgentExtensions
    {
        public static ClientUserAgent TryParse(this ClientUserAgent ua, string userAgent)
        {
            ua = new ClientUserAgent();
            var data = userAgent.Replace('(', '|');
            data = data.Replace(')', '|');

            var dataSplit = data.Split('|');

            if (dataSplit.Length >= 5)
            {
                ua.AppInfo = dataSplit[0];
                ua.PlatformInfo = dataSplit[1];
                ua.ProductInfo = dataSplit[2];
                ua.ApplicationNameInfo = dataSplit[3] + " " + dataSplit[4];
            }

            return ua;
        }
    }

    public enum ArchitectureType
    {
        X86,
        X64,
        Unknown
    }

    public enum OSType
    {
        Windows,
        MacOS,
        Linux,
        Android,
        iOS,
        Other
    }
}
