using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS3AutoClip
{
    internal static class Extensions
    {
        public static IEnumerable<T> AsTyped<T>(this System.Collections.IEnumerable collection)
        {
            foreach (var item in collection)
            {
                yield return (T)item;
            }
        }

        public static bool StartsWith(this byte[] bytes, byte b0)
        {
            return bytes.Length >= 1 && bytes[0] == b0;
        }
        public static bool StartsWith(this byte[] bytes, byte b0, byte b1)
        {
            return bytes.Length >= 2 && bytes[0] == b0 && bytes[1] == b1;
        }
        public static bool StartsWith(this byte[] bytes, byte b0, byte b1, byte b2)
        {
            return bytes.Length >= 3 && bytes[0] == b0 && bytes[1] == b1 && bytes[2] == b2;
        }
        public static bool StartsWith(this byte[] bytes, byte b0, byte b1, byte b2, byte b3)
        {
            return bytes.Length >= 4 && bytes[0] == b0 && bytes[1] == b1 && bytes[2] == b2 && bytes[3] == b3;
        }

        public static byte[] Slice(this byte[] bytes, int start, int length = -1)
        {
            if (length < 0)
            {
                length = bytes.Length - start + 1 + length;
            }

            var result = new byte[length];
            Array.Copy(bytes, start, result, 0, result.Length);
            return result;
        }
    }
}
