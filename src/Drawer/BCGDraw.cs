using System;
using System.IO;

namespace BarcodeBakery.Common.Drawer
{
    internal abstract class BCGDraw
    {
        private const int MaxBufferLength = 32768;

        public abstract void Dispose();
        public abstract MemoryStream Draw(MemoryStream stream);

        internal static bool ByteArrayCompare(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
            {
                return false;
            }

            for (int i = 0; i < a1.Length; i++)
            {
                if (a1[i] != a2[i])
                {
                    return false;
                }
            }

            return true;
        }

        internal static void CopyToStream(Stream streamFrom, Stream streamTo)
        {
            var buffer = new byte[MaxBufferLength];

            int read;
            while ((read = streamFrom.Read(buffer, 0, buffer.Length)) > 0)
            {
                streamTo.Write(buffer, 0, read);
            }
        }

        internal static void CopyToStream(Stream streamFrom, Stream streamTo, int count)
        {
            var bufferLength = count == -1 ? MaxBufferLength : Math.Min(count, MaxBufferLength);
            var buffer = new byte[bufferLength];

            int read;
            var alreadyRead = 0;
            while (count - alreadyRead > 0 && (read = streamFrom.Read(buffer, 0, Math.Min(count - alreadyRead, MaxBufferLength))) > 0)
            {
                alreadyRead += read;

                streamTo.Write(buffer, 0, read);
            }
        }
    }
}
