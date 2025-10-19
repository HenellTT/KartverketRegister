﻿namespace KartverketRegister.Utils
{
	//inneholder konstater og innstillinger for databaseoppsett
    public static class Constants
    {
		public static string DataBaseIp { get; } = Environment.GetEnvironmentVariable("DATABASE_IP") ?? "127.0.0.1"; 
		public static int DataBasePort { get; } = int.TryParse(Environment.GetEnvironmentVariable("DATABASE_PORT"), out var port) ? port : 3306;
        public static string DataBaseRootPassword { get; } = Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? "mysecretpassword";
        public static string DataBaseName { get; } = "ObstacleRegister";
        public static bool ResetDbOnStartup { get; set; } = false;

    }
}
