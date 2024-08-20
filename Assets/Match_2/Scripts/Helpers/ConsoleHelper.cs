using UnityEngine;

namespace Helpers
{
    public static class ConsoleHelper
    {
        private static bool debug = false;

        public static void PrintLog(string _message)
        {
            if (!debug)
                return;

            Debug.Log(_message);
        }

        public static void PrintLogWithColor(string _message, string _color)
        {
            if (!debug)
                return;

            Debug.Log($"<color={_color}>{_message}</color>");
        }

        public static void PrintSuccess(string _message)
        {
            if (!debug)
                return;

            Debug.Log($"<color=green>{_message}</color>");
        }

        public static void PrintWarning(string _message)
        {
            if (!debug)
                return;

            Debug.LogWarning(_message);
        }

        public static void PrintError(string _message)
        {
            if (!debug)
                return;

            Debug.LogError(_message);
        }
    }
}

