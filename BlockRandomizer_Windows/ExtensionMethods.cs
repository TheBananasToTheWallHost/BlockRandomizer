using System;


namespace GeneratorExtensions {
    public static class Extensions {

        /// <summary>
        /// Retrieves a substring starting at one index and ending at another index.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startIndex">The inclusive index where the substring will begin</param>
        /// <param name="endIndex">The inclusive index where the substring will end</param>
        /// <returns>A substring in the given string</returns>
        public static string IndexSubstring(this string str, int startIndex, int endIndex) {
            if(endIndex > startIndex) {
                return "";
            }
            return str.Substring(startIndex, (endIndex - startIndex) + 1);
        }

    }
}

