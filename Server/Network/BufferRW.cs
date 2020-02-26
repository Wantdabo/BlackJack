using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Network
{
    public class BufferRW
    {
        private MemoryStream memoryStream;
        private byte[] buffer;

        private BinaryReader binaryReader;
        private BinaryWriter binaryWriter;

        public BufferRW()
        {
            memoryStream = new MemoryStream();
        }
        public void BeginRead(byte[] _buffer)
        {
            memoryStream = new MemoryStream(_buffer);
            binaryReader = new BinaryReader(memoryStream);
        }

        public void EndRead()
        {
            memoryStream.Close();
            memoryStream = null;
            binaryReader.Close();
            binaryReader = null;
        }

        public void BeginWrite()
        {
            memoryStream = new MemoryStream();
            binaryWriter = new BinaryWriter(memoryStream);
        }

        public byte[] EndWrite()
        {
            buffer = memoryStream.ToArray();
            memoryStream.Close();
            memoryStream = null;
            binaryWriter.Close();
            binaryWriter = null;

            return buffer;
        }

        public bool ReadBoolean()
        {
            return binaryReader.ReadBoolean();
        }

        public short ReadInt16()
        {
            return binaryReader.ReadInt16();
        }

        public int ReadInt32()
        {
            return binaryReader.ReadInt32();
        }

        public long ReadInt64()
        {
            return binaryReader.ReadInt64();
        }

        public string ReadString()
        {
            return binaryReader.ReadString();
        }

        public void WriteBoolean(bool _b)
        {
            binaryWriter.Write(_b);
        }

        public void WriteInt16(short _s)
        {
            binaryWriter.Write(_s);
        }

        public void WriteInt32(int _i)
        {
            binaryWriter.Write(_i);
        }

        public void WriteInt64(long _l)
        {
            binaryWriter.Write(_l);
        }

        public void WriteString(string _s)
        {
            binaryWriter.Write(_s);
        }
    }
}
