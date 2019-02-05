namespace SampleChatting.Lib
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
    }

    public static class Protocols
    {
        public const string EXIT_CLIENT = "ExitClient";
        public const string GET_USER_ACCESS = "GetUserAccess";
    }

    public static class Servers
    {
        public const int PORT_LOGIN = 18100;
        public const int PORT_CHANNEL = 18200;

        public static string[] IP_LOBBY = {
            "localhost",
        };
    }
}
