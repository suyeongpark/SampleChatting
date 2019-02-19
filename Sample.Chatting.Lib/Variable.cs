namespace Sample.Chatting.Lib
{
    public static class Keys
    {
        public const string KEY = "Key";
        public const string VALUE = "Value";

        public const string ID = "ID";
        public const string PASSWORD = "Password";
    }

    public static class Values
    {
        public const int DELAY_CHECK_QUEUE = 1000;

        public const string DB_DATASOURCE = "";
        public const string DB_PASSWORD = "";

        public static byte[] CRYPT_KEY = { 185, 229, 136, 247, 43, 50, 49, 236, 97, 141, 71, 229, 84, 43, 139, 99, };
        public static byte[] CRYPT_IV = { 233, 185, 128, 8, 221, 109, 53, 175, 50, 19, 228, 164, 244, 25, 239, 38, };
    }

    public static class Protocols
    {
        public const string EXIT_CLIENT = "ExitClient";

        public const string CREATE_USER = "CreateUser";
        public const string LOGIN = "Login";
    }

    public static class Servers
    {
        public const int PORT_LOGIN = 18100;
        public const int PORT_CHANNEL = 18200;

        public const string IP_LOGIN = "localhost";

        public static string[] IP_LOBBY = {
            "localhost",
        };
    }

    public static class Parameters
    {
        public const string ID = "@ID";
        public const string PASSWORD = "@Password";
    }
}
