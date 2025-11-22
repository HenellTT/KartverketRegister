using KartverketRegister.Utils;
using Mysqlx.Crud;
using Xunit;

namespace KartverketRegister.Tests.Utils
{
    public class LinkGeneratorBrrTests
    {
        [Fact]
        public void HoydeDataCoords_GeneratesCorrectUrl()
        {
            double x = 123456.78;
            double y = 987654.32;
            string url = LinkGeneratorBrr.HoydeDataCoords(x, y);

            // Make sure the URL base is correct
            Assert.StartsWith("https://hoydedata.no/arcgis/rest/services", url);

            // Make sure the coordinates appear somewhere in the URL (invariant culture!)
            Assert.Contains(x.ToString(System.Globalization.CultureInfo.InvariantCulture), url);
            Assert.Contains(y.ToString(System.Globalization.CultureInfo.InvariantCulture), url);
        }

    }
}
