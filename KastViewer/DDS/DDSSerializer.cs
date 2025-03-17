using DDS;
using System.Runtime.InteropServices;
using System.Text;
namespace HNIdesu.DDS
{

    public class DDSSerializer
    {
        public static void Serialize(DDSFile dds,Stream stream)
        {
            var headerSize = Marshal.SizeOf<DDSHeader>();
            using var bw = new BinaryWriter(stream,Encoding.UTF8, leaveOpen: true);
            bw.Write(Encoding.ASCII.GetBytes(DDSFile.Signature));
            var ptr = Marshal.AllocHGlobal(headerSize);
            Marshal.StructureToPtr(dds.DDSHeader, ptr, true);
            var buffer = new byte[headerSize];
            Marshal.Copy(ptr, buffer, 0, buffer.Length);
            Marshal.FreeHGlobal(ptr);
            bw.Write(buffer);
            bw.Write(dds.TextureData);
        }
        public static DDSFile Deserialize(Stream stream)
        {
            var headerSize = Marshal.SizeOf<DDSHeader>();
            using var br = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
            br.BaseStream.Seek(4, SeekOrigin.Begin);//Skip the signature
            var ptr = Marshal.AllocHGlobal(headerSize);
            Marshal.Copy(br.ReadBytes(headerSize), 0, ptr, headerSize);
            var header = Marshal.PtrToStructure<DDSHeader>(ptr);
            Marshal.FreeHGlobal(ptr);
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            var data = ms.ToArray();
            return new DDSFile(header, data);
        }
    }
}
