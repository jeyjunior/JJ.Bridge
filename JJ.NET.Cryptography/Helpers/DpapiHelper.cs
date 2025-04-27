using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace JJ.NET.Cryptography.Helpers
{
    internal static class DpapiHelper
    {
        [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool CryptProtectData(
            ref DATA_BLOB pDataIn,
            string szDataDescr,
            IntPtr pOptionalEntropy,
            IntPtr pvReserved,
            IntPtr pPromptStruct,
            int dwFlags,
            ref DATA_BLOB pDataOut);

        [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool CryptUnprotectData(
            ref DATA_BLOB pDataIn,
            string szDataDescr,
            IntPtr pOptionalEntropy,
            IntPtr pvReserved,
            IntPtr pPromptStruct,
            int dwFlags,
            ref DATA_BLOB pDataOut);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DATA_BLOB
        {
            public int cbData;
            public IntPtr pbData;
        }

        public static byte[] Protect(byte[] data)
        {
            var inputBlob = new DATA_BLOB();
            var outputBlob = new DATA_BLOB();

            try
            {
                inputBlob.pbData = Marshal.AllocHGlobal(data.Length);
                Marshal.Copy(data, 0, inputBlob.pbData, data.Length);
                inputBlob.cbData = data.Length;

                if (!CryptProtectData(ref inputBlob, null, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0, ref outputBlob))
                    throw new SecurityException($"CryptProtectData failed: {Marshal.GetLastWin32Error()}");

                byte[] result = new byte[outputBlob.cbData];
                Marshal.Copy(outputBlob.pbData, result, 0, outputBlob.cbData);

                return result;
            }
            finally
            {
                if (inputBlob.pbData != IntPtr.Zero)
                    Marshal.FreeHGlobal(inputBlob.pbData);
                if (outputBlob.pbData != IntPtr.Zero)
                    Marshal.FreeHGlobal(outputBlob.pbData);
            }
        }

        public static byte[] Unprotect(byte[] data)
        {
            var inputBlob = new DATA_BLOB();
            var outputBlob = new DATA_BLOB();

            try
            {
                inputBlob.pbData = Marshal.AllocHGlobal(data.Length);
                Marshal.Copy(data, 0, inputBlob.pbData, data.Length);
                inputBlob.cbData = data.Length;

                if (!CryptUnprotectData(ref inputBlob, null, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0, ref outputBlob))
                    throw new SecurityException($"CryptUnprotectData failed: {Marshal.GetLastWin32Error()}");

                byte[] result = new byte[outputBlob.cbData];
                Marshal.Copy(outputBlob.pbData, result, 0, outputBlob.cbData);

                return result;
            }
            finally
            {
                if (inputBlob.pbData != IntPtr.Zero)
                    Marshal.FreeHGlobal(inputBlob.pbData);
                if (outputBlob.pbData != IntPtr.Zero)
                    Marshal.FreeHGlobal(outputBlob.pbData);
            }
        }
    }
}
