using KartverketRegister.Models;
using KartverketRegister.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

//kontroller for database-migrering
namespace KartverketRegister.Controllers
{
    public class MigrateController : Controller
    {
        private readonly string MigrationHashword = "84654139869a7dc2efc4fa2110d1e9a85a30d43beed59ee315f4cf102f01a026"; // secrethash
        [HttpGet]
        public IActionResult Index()
        {
            return View("Migrate");
        }
        [HttpGet]
        public IActionResult Migrate(string hashish)
        {
            string hashedHashish = ComputeSha256Hash(hashish);

            if (hashedHashish != MigrationHashword)
            {
                return Json(new GeneralResponse(false, "Permission Denied"));
            }


            SequelMigrator seq = new SequelMigrator();
            try
            {
                seq.Migrate();
                return Json(new GeneralResponse(true, "Database migrated successfully"));
            } catch
            {
                return Json(new GeneralResponse(false, "Database migration failed"));

            }
        }

        //beregner SHA256-hash av input
        private string ComputeSha256Hash(string rawData)
        {
            if (rawData == null) { return "a"; }
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); // lowercase hex
                }
                return builder.ToString();
            }
        }
    }
}
