using KartverketRegister.Auth;
using KartverketRegister.Models;
using Microsoft.AspNetCore.Identity;

namespace KartverketRegister.Utils
{
    // Genererer testdata for utvikling og testing
    public class DummyCreator
    {
        private readonly UserManager<AppUser> _userManager;

        public DummyCreator(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        // Genererer dummy-brukere og markører
        public async Task<GeneralResponse> FillIn()
        {
            var rnd = new Random();

            string[] firstNames = {
                "Liam", "Olivia", "Noah", "Emma", "Elijah",
                "Ava", "Sophia", "James", "Isabella", "Benjamin",
                "Mia", "Lucas", "Charlotte", "Henry", "Amelia",
                "Alexander", "Harper", "Michael", "Evelyn", "Daniel",
                "Abigail", "Jacob", "Emily", "Jackson", "Ella",
                "Logan", "Elizabeth", "Sebastian", "Avery", "Jack"
            };

            string[] lastNames = {
                "Smith", "Johnson", "Williams", "Brown", "Jones",
                "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
                "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson",
                "Thomas", "Taylor", "Moore", "Jackson", "Martin",
                "Lee", "Perez", "Thompson", "White", "Harris",
                "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson"
            };

            string[] organizations = {
                "TechNova Solutions", "GreenLeaf Industries", "BluePeak Software",
                "SilverGate Logistics", "SunCore Energy", "Apex Robotics",
                "NorthStar Finance", "UrbanHive Marketing", "QuantumSphere Labs",
                "BrightPath Consulting"
            };

            string[] obstacleCategories = {
                "Tower", "Crane", "Wind Turbine", "Mast",
                "Building", "Power Line", "Other"
            };

            var seq = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
            var createdUsers = new List<AppUser>();

            // Opprett dummy-brukere
            for (int i = 0; i < lastNames.Length; i++)
            {
                string randomOrg = organizations[rnd.Next(organizations.Length)];
                string email = $"{lastNames[i]}{rnd.Next(1000)}@gmail.com";

                var user = new AppUser
                {
                    Name = email,
                    FirstName = firstNames[i],
                    LastName = lastNames[i],
                    Organization = randomOrg,
                    UserName = email,
                    UserType = "User",
                    Password = "1234",
                    Email = email
                };

                Console.WriteLine($"[DummyCreator] Creating user: {email}");

                var result = await _userManager.CreateAsync(user, "1234");

                if (result.Succeeded)
                {
                    createdUsers.Add(user);
                }
                else
                {
                    Console.WriteLine("[DummyCreator] Failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            // Opprett dummy-markører for hver bruker
            foreach (var user in createdUsers)
            {
                double lat = RandomInRange(rnd, 57.0, 60.0);
                double lng = RandomInRange(rnd, 6.0, 10.0);
                string obstacle = obstacleCategories[rnd.Next(obstacleCategories.Length)];

                seq.SaveMarker(
                    type: "Dummy",
                    description: "Dummy marker",
                    lat: lat,
                    lng: lng,
                    userId: rnd.Next(20) + 1,
                    organization: "Dummy",
                    heightM: rnd.Next(5, 150),
                    heightMOverSea: rnd.Next(5, 1500),
                    accuracyM: rnd.Next(1, 10),
                    obstacleCategory: obstacle,
                    isTemporary: false,
                    lighting: "None",
                    source: "DummyCreator"
                );

                Console.WriteLine($"[DummyCreator] Created marker for {user.Email} at {lat}, {lng}");
            }

            return new GeneralResponse(true, "Dummy users and markers generated");
        }

        private static double RandomInRange(Random r, double min, double max)
        {
            return r.NextDouble() * (max - min) + min;
        }

        // Genererer standard testbrukere (User, Admin, Employee)
        public async Task<GeneralResponse> GenerateDefaultUsers()
        {
            var user = new AppUser
            {
                Name = "1234@user.test",
                FirstName = "Johnny",
                LastName = "Test",
                Organization = "UiA",
                UserName = "1234@user.test",
                UserType = "User",
                Password = "!Testink00!",
                Email = "1234@user.test"
            };

            var admin = new AppUser
            {
                Name = "1234@admin.test",
                FirstName = "Adminman",
                LastName = "Testman",
                Organization = "Kartverket",
                UserType = "Admin",
                Password = "!Testink00!",
                Email = "1234@admin.test"
            };

            var employee = new AppUser
            {
                Name = "1234@employee.test",
                FirstName = "Employman",
                LastName = "Test",
                Organization = "Kartverket",
                UserName = "1234@employee.test",
                UserType = "Employee",
                Password = "!Testink00!",
                Email = "1234@employee.test"
            };

            await _userManager.CreateAsync(user, "!Testink00!");
            await _userManager.CreateAsync(admin, "!Testink00!");
            await _userManager.CreateAsync(employee, "!Testink00!");

            return new GeneralResponse(true, "Default test users created");
        }
    }
}
