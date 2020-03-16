using System;


namespace GeneratorExtensions {
    public static class Extensions {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public static string IndexSubstring(this string str, int startIndex, int endIndex) {
            return str.Substring(startIndex, (endIndex - startIndex) + 1);
        }

    }
}

