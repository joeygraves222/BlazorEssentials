using BlazorEssentials.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorEssentials.Extensions
{
    public static class MarkupExtensions
    {
        public static string ToCSSString(this MarkupColor color)
        {
            return color.ToString().ToLower();
        }

        public static string ToCSSString(this LoaderSize size)
        {
            return size.ToString().ToLower();
        }

        public static string ToCSSString(this LoaderStyle style)
        {
            return style.ToString().ToLower();
        }
    }
}
