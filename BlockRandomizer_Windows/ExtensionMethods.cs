using System;
using System.Collections.Generic;

namespace GeneratorExtensions {
    public static class Extensions {

        /// <summary>
        /// Retrieves a substring starting at one index and ending at another index.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startIndex">The index where the substring will begin (inclusive)</param>
        /// <param name="endIndex">The index where the substring will end (inclusive)</param>
        /// <returns>A substring in the given string</returns>
        public static string IndexSubstring(this string str, int startIndex, int endIndex) {
            if(endIndex < startIndex) {
                return "";
            }
            return str.Substring(startIndex, (endIndex - startIndex) + 1);
        }

        /// <summary>
        /// Removes characters from a string starting at one index and ending at another index
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startIndex">The index at which characters will start being removed (inclusive)</param>
        /// <param name="endIndex">The index at which characters will stop being removed (inclusive)</param>
        /// <returns>A new string with the contents of the current string minus the deleted characters</returns>
        public static string IndexRemove(this string str, int startIndex, int endIndex) {
            if(endIndex < startIndex) {
                return str;
            }
            return str.Remove(startIndex, (endIndex - startIndex) + 1);
        }

        /// <summary>
        /// Returns the zero based index of the next occurence of a given character starting at a specified index.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="c">the character to search for</param>
        /// <param name="index">the index to start searching from (inclusive)</param>
        /// <returns>the index of the next occurence of the specified character</returns>
        public static int NextIndexOf(this string str, char c, int index) {
            return str.Substring(index).IndexOf(c) + index;
        }

        /// <summary>
        /// Returns the indices of all occurences of a given character
        /// </summary>
        /// <param name="str"></param>
        /// <param name="c">the character to search for</param>
        /// <returns>a list containing all indices of all occurences of the specified  character</returns>
        public static List<int> AllIndexesOf(this string str, char c) {
            List<int> indices = new List<int>();
            for (int i = 0; i < str.Length; i++) {
                if (str[i].Equals(c)) {
                    indices.Add(i);
                }
            }
            return indices;
        }

    }
}

