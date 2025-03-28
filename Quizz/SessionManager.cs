namespace Quizz
{
    public static class SessionManager
    {
        public static string Username { get; set; }
        public static string Status { get; set; }

        public static void ClearSession()
        {
            Username = null;
            Status = null;
        }
    }
}
