using System.Text;
using System.IO.Compression;
using HNIdesu.DDS;
using BCnEncoder.Shared.ImageFiles;
using BCnEncoder.Decoder;
using System.Drawing;

namespace KastViewer.Core
{
    public class TextureInternal
    {
        public string? PlatformSignature { get; private set; }
        public string TextureType { get; private set; } = string.Empty;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public string FilePath { get; private set; } = string.Empty;
        public DateTime CreateDate { get; private set; }
        public bool IsCompressed => ActualSize != Size;
        public long ActualSize { get; private set; }
        public long Offset { get; private set; }
        public long Size { get; private set; }
        public int Format { get; private set; }
        public void Load(string filePath)
        {
            using var br = new BinaryReader(File.OpenRead(filePath));
            if (Encoding.ASCII.GetString(br.ReadBytes(4)) != "XETK")
                throw new InvalidDataException("Invalid file header");
            var version = br.ReadInt32();
            if (version >= 0x10000)
                PlatformSignature = Encoding.ASCII.GetString(br.ReadBytes(4).Reverse().ToArray());
            TextureType = Encoding.ASCII.GetString(br.ReadBytes(4).Reverse().ToArray());
            Width = br.ReadInt32();
            Height = br.ReadInt32();
            br.ReadInt32();
            var mipmapCount = br.ReadInt32();
            Format = br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
            for (int i = 0; i < 3; i++)
                br.ReadByte();
            CreateDate = new DateTime(
                br.ReadInt32(),
                br.ReadInt32(),
                br.ReadInt32(),
                br.ReadInt32(),
                br.ReadInt32(),
                br.ReadInt32()
            );
            ActualSize = br.ReadInt64();
            Size = br.ReadInt64();
            Offset = br.BaseStream.Position;
            FilePath = filePath;
        }

        public Stream OpenRead()
        {
            using var fs = File.OpenRead(FilePath);
            fs.Seek(Offset, SeekOrigin.Begin);
            var buffer = new byte[ActualSize];
            for (var bytesRead = 0; bytesRead < ActualSize; bytesRead += fs.Read(buffer, bytesRead, (int)(ActualSize - bytesRead))) ;
            if (IsCompressed)
                return new ZLibStream(new MemoryStream(buffer), CompressionMode.Decompress, false);
            else
                return new MemoryStream(buffer);
        }
    }
}
