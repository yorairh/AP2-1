using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace AP2_1
{
    class StringWrapper
    {
        public const string STRING_LIBRARY_PATH = "StringWrapper.dll";
        [DllImport(STRING_LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CreatestringWrapper();
        [DllImport(STRING_LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int len(IntPtr s);
        [DllImport(STRING_LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern char getCharByIndex(IntPtr s, int x);
        [DllImport(STRING_LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern void addChar(IntPtr s, char c);
        [DllImport(STRING_LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern void removeStr(IntPtr s);
        

        public static string GetStr(IntPtr s)
        {
            int l = len(s);
            string str = "";
            for (int i = 0; i < l; ++i)
            {
                str += getCharByIndex(s, i).ToString();
            }
            return str;
        }

        public static IntPtr CreateStringWrapperFromString(string str)
        {
            IntPtr s = CreatestringWrapper();
            foreach (char c in str)
            {
                addChar(s, c);
            }
            return s;
        }
    }
}
