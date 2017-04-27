namespace ThermalTalk
{
    using System;
    using System.Runtime.InteropServices;

    internal class NativeMethods
    {  // Structure and API declarions:
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pDataType;
        }

        /// <summary>
        /// Allow all fields to be zero if opening printer in read-only mode.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal class PRINTER_DEFAULTS
        {
            public IntPtr pDatatype;
            public IntPtr pDevMode;
            public int DesiredAccess;
        }

        /// <summary>
        /// http://www.pinvoke.net/default.aspx/winspool.OpenPrinter
        /// </summary>
        /// <param name="szPrinter"></param>
        /// <param name="hPrinter"></param>
        /// <param name="pd"></param>
        /// <returns></returns>
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPWStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        /// <summary>
        /// http://www.pinvoke.net/default.aspx/winspool.OpenPrinter
        /// </summary>
        /// <param name="szPrinter"></param>
        /// <param name="hPrinter"></param>
        /// <param name="pd"></param>
        /// <returns></returns>
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool OpenPrinter2([MarshalAs(UnmanagedType.LPWStr)] string szPrinter, out IntPtr hPrinter, PRINTER_DEFAULTS pd);

        /// <summary>
        /// http://www.pinvoke.net/default.aspx/winspool.ClosePrinter
        /// </summary>
        /// <param name="hPrinter"></param>
        /// <returns></returns>
        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        /// <summary>
        /// http://www.pinvoke.net/default.aspx/winspool.EndDocPrinter
        /// </summary>
        /// <param name="hPrinter"></param>
        /// <returns></returns>
        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool StartPagePrinter(IntPtr hPrinter);

        /// <summary>
        /// http://www.pinvoke.net/default.aspx/winspool.EndPagePrinter
        /// </summary>
        /// <param name="hPrinter"></param>
        /// <returns></returns>
        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool EndPagePrinter(IntPtr hPrinter);

        /// <summary>
        /// http://www.pinvoke.net/default.aspx/winspool.WritePrinter
        /// </summary>
        /// <param name="hPrinter"></param>
        /// <param name="pBytes"></param>
        /// <param name="dwCount"></param>
        /// <param name="dwWritten"></param>
        /// <returns></returns>
        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        /// <summary>
        /// http://www.pinvoke.net/default.aspx/winspool.ReadPrinter
        /// </summary>
        /// <param name="hPrinter"></param>
        /// <param name="pBytes"></param>
        /// <param name="dwCount"></param>
        /// <param name="dwNoBytesRead"></param>
        /// <returns></returns>
        [DllImport("winspool.drv", EntryPoint = "ReadPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool ReadPrinter(IntPtr hPrinter, out IntPtr pBytes, Int32 dwCount, out Int32 dwNoBytesRead);
    }
}
