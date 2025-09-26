namespace KartverketRegister.Utils
{
    public static class Constants
    {
        public static string DataBaseIp { get; } = "mariadb"; // Change to "mariadb" if running docker compose
        public static int DataBasePort { get; } = 3306; // Change to "mariadb" if running docker compose
        public static string DataBaseRootPassword { get; } = "mysecretpassword";
        public static string DataBaseName { get; } = "ObstacleRegister";
        public static bool ResetDbOnStartup { get; } = false;

    }
}
