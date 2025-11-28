using KartverketRegister.Models;
using KartverketRegister.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace KartverketRegister.Controllers
{
    // Database-migrering - beskyttet med hemmelig nøkkel
    public class MigrateController : Controller
    {
        // SHA256-hash av hemmelig migrasjonsnøkkel
        private const string MigrationKeyHash = "84654139869a7dc2efc4fa2110d1e9a85a30d43beed59ee315f4cf102f01a026";

        [HttpGet]
        public IActionResult Index() => View("Migrate");

        // POST brukes for sikkerhet - GET-parametere logges i server-logs
        [HttpPost]
        public IActionResult Migrate([FromForm] string migrationKey)
        {
            string hashedKey = ComputeSha256Hash(migrationKey);

            if (hashedKey != MigrationKeyHash)
                return Json(new GeneralResponse(false, "Permission Denied"));

            try
            {
                var migrator = new SequelMigrator();
                migrator.Migrate();
                return Json(new GeneralResponse(true, "Database migrated successfully"));
            }
            catch (Exception e)
            {
                return Json(new GeneralResponse(false, $"Migration failed: {e.Message}"));
            }
        }

        private static string ComputeSha256Hash(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

            var builder = new StringBuilder();
            foreach (byte b in bytes)
                builder.Append(b.ToString("x2"));

            return builder.ToString();
        }
    }
}
