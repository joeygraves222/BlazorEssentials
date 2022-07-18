using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorEssentials.Models
{
    public static class Util
    {

        public static string EncodeToBase64(string toEncode)
        {
            return Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(toEncode));
        }

        public static string DecodeFromBase64(string encodedData)
        {
            return ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(encodedData));
        }
    }
}
