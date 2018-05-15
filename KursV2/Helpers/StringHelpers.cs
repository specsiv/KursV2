using System.IO;

namespace KursV2.Helpers
{
    static class StringHelpers
    {
        public static string singlelineFilter(string text)
        {
            string result = string.Empty;

            foreach (var character in text)
            {
                if (character != '\n' && character != '\r') result += character;
            }

            return result;
        }

        public static string cleaningFilter(string row)
        {
            return System.Text.RegularExpressions.Regex.Replace(row, @"\s+", " ").ToLower().Trim();
        }

        public static string[] simpleSplit(string row, char div, bool reverce = false)
        {
            string[] result = new string[2];
            int i;

            if (reverce) i = row.Length - 1;
            else i = 0;

            while (row[i] != div)
            {
                if (reverce)
                {
                    result[1] = row[i] + result[1];
                    --i;
                }
                else
                {
                    result[0] += row[i];
                    ++i;
                }
            }
            if (reverce) --i;
            else ++i;

            if (reverce)
                while (i >= 0)
                {
                    result[0] = row[i] + result[0];
                    --i;
                }
            else
                while (i < row.Length)
                {
                    result[1] += row[i];
                    ++i;
                }

            return result;
        }

        public static string load(string path)
        {
            string result;

            try
            {
                using (var fileStream = new StreamReader(path))
                {
                    result = fileStream.ReadToEnd();

                    fileStream.Close();
                }

                return result;
            }
            catch { return null; }
        }

        public static bool save(string path, string text)
        {
            try
            {
                using (var fileStream = new StreamWriter(path))
                {
                    fileStream.Write(text);

                    fileStream.Close();
                }

                return true;
            }
            catch { return false; }
        }
    }
}