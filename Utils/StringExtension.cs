using System.Net;

namespace KartverketRegister.Utils
{
    public static class StringExtension
    {
        public static string HtmlEncode(this string str)
        {
            return WebUtility.HtmlEncode(str);
        }
    }
}
