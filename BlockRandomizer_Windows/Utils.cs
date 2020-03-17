using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities {
    public static class Utils {

        /// <summary>
        /// Swaps the values between two indices in an array
        /// </summary>
        /// <param name="array">the array</param>
        /// <param name="index1">the first index</param>
        /// <param name="index2">the second index</param>
        public static void SwapItems<T>(T[] array, int index1, int index2) {
            T temp = array[index1];
            array[index1] = array[index2];
            array[index2] = temp;
        }
    }
}
