namespace KastViewer.GUI
{
    internal static class Logger
    {
        public const string TAG = "KastViewer";
        private static readonly string LogFilePath = "log.txt";

        public static void Debug(string msg)
        {
            string logMessage = $"[{TAG}] [DEBUG] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {msg}";
            LogMessage(logMessage);
        }

        public static void Info(string msg)
        {
            string logMessage = $"[{TAG}] [INFO] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {msg}";
            LogMessage(logMessage);
        }

        public static void Error(string msg, Exception? ex = null)
        {
            string logMessage = $"[{TAG}] [ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {msg}";
            LogMessage(logMessage);

            if (ex != null)
            {
                string errorDetails = $"Exception: {ex.Message}\nStack Trace: {ex.StackTrace}";
                LogMessage(errorDetails);
            }
        }

        private static void LogMessage(string message)
        {
            try
            {
                using StreamWriter writer = new StreamWriter(LogFilePath, append: true);
                writer.WriteLine(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing log: {ex.Message}");
            }
        }
    }


}
