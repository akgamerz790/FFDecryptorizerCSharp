using System;
using System.Runtime.InteropServices;

namespace CoreOSLabsUtils
{
    public class NSSWrapper
    {
        [DllImport("nss3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NSS_Init(string configDir);

        [DllImport("nss3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NSS_Shutdown();

        [DllImport("nss3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PK11SDR_Decrypt(ref SECItem input, ref SECItem output, IntPtr cx);

        [StructLayout(LayoutKind.Sequential)]
        public struct SECItem
        {
            public uint Type;
            public IntPtr Data;
            public uint Len;
        }

        public bool Initialize(string profilePath)
        {
            int rc = NSS_Init(profilePath);
            return rc == 0;
        }

        public void Shutdown()
        {
            NSS_Shutdown();
        }

        public string Decrypt(string base64Data)
        {
            byte[] data = Convert.FromBase64String(base64Data);
            GCHandle pinnedArray = GCHandle.Alloc(data, GCHandleType.Pinned);

            SECItem input = new SECItem
            {
                Type = 0,
                Data = pinnedArray.AddrOfPinnedObject(),
                Len = (uint)data.Length
            };

            SECItem output = new SECItem();
            int result = PK11SDR_Decrypt(ref input, ref output, IntPtr.Zero);

            pinnedArray.Free();

            if (result != 0)
            {
                throw new Exception("Decryption failed");
            }

            byte[] decrypted = new byte[output.Len];
            Marshal.Copy(output.Data, decrypted, 0, (int)output.Len);

            return System.Text.Encoding.UTF8.GetString(decrypted);
        }
    }
}
