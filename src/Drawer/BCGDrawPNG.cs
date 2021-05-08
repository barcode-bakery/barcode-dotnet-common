using System;
using System.IO;

namespace BarcodeBakery.Common.Drawer
{
    internal sealed class BCGDrawPNG : BCGDraw, IDisposable
    {
        private MemoryStream? stream = null;

        internal BCGDrawPNG()
        {
        }

        public override MemoryStream Draw(MemoryStream stream)
        {
            var c = "0000004D74455874436F707972696768740047656E657261746564207769746820426172636F64652042616B65727920666F72202E4E455420687474703A2F2F7777772E626172636F646562616B6572792E636F6DFB518FC3";

            // We can only have one stream at a time.
            this.Dispose();

            byte[] buffer = new byte[8];
            this.stream = new MemoryStream((int)stream.Length + c.Length / 2);

            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(buffer, 0, 8);
            if (BCGDraw.ByteArrayCompare(buffer, new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }))
            {
                stream.Seek(0, SeekOrigin.Begin);
                BCGDraw.CopyToStream(stream, this.stream, 33);
                var i = 0;
                while (i < c.Length)
                {
                    buffer[0] = (byte)Convert.ToInt32(c.Substring(i, 2), 16);
                    this.stream.Write(buffer, 0, 1);
                    i += 2;
                }

                BCGDraw.CopyToStream(stream, this.stream);
            }
            else
            {
                stream.Seek(0, SeekOrigin.Begin);
                BCGDraw.CopyToStream(stream, this.stream);
            }

            return this.stream;
        }

        public override void Dispose()
        {
            if (this.stream != null)
            {
                this.stream.Dispose();
                this.stream = null;
            }
        }
    }
}
