using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;

namespace AP2_1
{
    class LibraryManager
    {
        public const string LIBRARY_PATH = "AnomaliesLibrary.dll";
        [DllImport(LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr detectAnomalis(IntPtr trainFile, IntPtr testFile);
        [DllImport(LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr getCorrelateFeatureByFeatureName(IntPtr r, IntPtr name);
        [DllImport(LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int isAnomaly(IntPtr r, IntPtr feature, int timeStep);
        [DllImport(LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern void deleteResults(IntPtr r);
        public const string LINEAR_LIBRARY_PATH = "LinearRegression.dll";
        [DllImport(LINEAR_LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern float getSlope(IntPtr r, IntPtr feature);
        [DllImport(LINEAR_LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern float getYIntercept(IntPtr r, IntPtr feature);
        [DllImport(LINEAR_LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr getAnomalyDescription(IntPtr r, int index);
        [DllImport(LINEAR_LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getAnomalyTimeStep(IntPtr r, int index);
        [DllImport(LINEAR_LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int anomaliesLen(IntPtr r);

        public static void SetLibrary(string path)
        {
            File.Copy(path, LIBRARY_PATH);
        }
    }
}
