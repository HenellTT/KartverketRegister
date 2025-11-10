using KartverketRegister.Auth;
using KartverketRegister.Models;
using Microsoft.AspNetCore.Identity;

namespace KartverketRegister.Utils
{
    public class DummyCreator
    {
        private readonly UserManager<AppUser> _userManager;

        public DummyCreator(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<GeneralResponse> FillIn()
        {
            Random rnd = new Random();

            string[] firstNames = new string[]
            {
                "Liam", "Olivia", "Noah", "Emma", "Elijah",
                "Ava", "Sophia", "James", "Isabella", "Benjamin",
                "Mia", "Lucas", "Charlotte", "Henry", "Amelia",
                "Alexander", "Harper", "Michael", "Evelyn", "Daniel",
                "Abigail", "Jacob", "Emily", "Jackson", "Ella",
                "Logan", "Elizabeth", "Sebastian", "Avery", "Jack"
            };

            string[] lastNames = new string[]
            {
                "Smith", "Johnson", "Williams", "Brown", "Jones",
                "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
                "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson",
                "Thomas", "Taylor", "Moore", "Jackson", "Martin",
                "Lee", "Perez", "Thompson", "White", "Harris",
                "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson"
            };

            string[] organizations = new string[]
            {
                "TechNova Solutions",
                "GreenLeaf Industries",
                "BluePeak Software",
                "SilverGate Logistics",
                "SunCore Energy",
                "Apex Robotics",
                "NorthStar Finance",
                "UrbanHive Marketing",
                "QuantumSphere Labs",
                "BrightPath Consulting"
            };

            string[] obstacleCategories = new string[]
            {
                "Tower", "Crane", "Wind Turbine", "Mast",
                "Building", "Power Line", "Other"
            };

            // DB accessor
            SequelMarker seq = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);

            // 🔹 Keep track of created users
            List<AppUser> createdUsers = new List<AppUser>();

            // ✅ Create dummy users
            for (int i = 0; i < lastNames.Length; i++)
            {
                string randomOrg = organizations[rnd.Next(organizations.Length)];
                string email = $"{lastNames[i]}{rnd.Next(1000)}@gmail.com";

                AppUser user = new AppUser
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

                Console.WriteLine($"[Dummy Data Creator] Creating user: {email}");

                var result = await _userManager.CreateAsync(user, "1234"); // ✅ Password must be passed here

                if (result.Succeeded)
                {
                    createdUsers.Add(user);
                }
                else
                {
                    Console.WriteLine("[Dummy Data Creator] Failed to create user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            // ✅ Create dummy markers for each dummy user
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
                    userId: rnd.Next(25),
                    organization: "Dummy",
                    heightM: rnd.Next(5, 150),
                    heightMOverSea: rnd.Next(5, 1500),
                    accuracyM: rnd.Next(1, 10),
                    obstacleCategory: obstacle,
                    isTemporary: false,
                    lighting: "None",
                    source: "DummyCreator"
                );

                Console.WriteLine($"[Dummy Data Creator] Created marker for user {user.Email} at {lat}, {lng}");
            }

            return new GeneralResponse(true, "Dummy users and markers generated");
        }

        public double RandomInRange(Random r, double min, double max)
        {
            return r.NextDouble() * (max - min) + min;
        }
        public async Task<GeneralResponse> GenerateDefaultUsers()
        {
            string email = "1234@user.test";
            AppUser user = new AppUser
            {
                Name = email,
                FirstName = "Johnny",
                LastName = "Test",
                Organization = "UiA",
                UserName = email,
                UserType = "User",
                Password = "!Testink69!",
                Email = email
            };

            email = "1234@admin.test";
            AppUser Admin = new AppUser
            {
                Name = email,
                FirstName = "Adminman",
                LastName = "Testman",
                Organization = "Kartverket",
                UserType = "Admin",
                Password = "!Testink69!",
                Email = email
            };
            email = "1234@employee.test";
            AppUser Employee = new AppUser
            {
                Name = email,
                FirstName = "Employman",
                LastName = "Test",
                Organization = "Kartverket",
                UserName = email,
                UserType = "Employee",
                Password = "!Testink69!",
                Email = email
            };

            await _userManager.CreateAsync(user, "!Testink69!");
            await _userManager.CreateAsync(Admin, "!Testink69!");
            await _userManager.CreateAsync(Employee, "!Testink69!");
            return new GeneralResponse(true,"Shit worked");
        }
    }
}
