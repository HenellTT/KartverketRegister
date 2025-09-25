namespace ObstacleRegister.Utils
{
    public static class Constants
    {
        public static string DataBaseIp { get; } = "127.0.0.1"; // Change to "mariadb" if running docker compose
        public static string DataBaseRootPassword { get; } = "mysecretpassword";
        public static string DataBaseName { get; } = "ObstacleRegister";
        public static bool ResetDbOnStartup { get; } = false;

    }
}
