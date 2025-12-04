using System.Net;
using System.Text.RegularExpressions;

namespace KartverketRegister.Utils
{
    public static class StringExtension
    {
        // This is the extension method.
        // The first parameter takes the "this" modifier
        // and specifies the type for which the method is defined.
        
        public static string HtmlEncode(this string str)
        {
            return WebUtility.HtmlEncode(str);
        }
    }

}
