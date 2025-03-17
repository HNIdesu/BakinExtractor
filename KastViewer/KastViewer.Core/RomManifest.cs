using System.Text;

namespace KastViewer.Core
{
    public class RomManifest
    {
        private enum RomSignature
        {
            Texture = 4387
        }
        public List<RomItem>? RomList { get; private set; }
        public List<RomItem>? MissingRomList { get;private set; }
        public short RomVersion { get; private set; }
        public int LastUsedSwitch { get; private set; }
        public void Load(BinaryReader br)
        {
            if (Encoding.ASCII.GetString(br.ReadBytes(5)) != "YUKAR")
                throw new InvalidDataException("Invalid file header");
            long chunkSize, currentPosition;
            chunkSize = br.ReadInt32();
            br.ReadInt16();
            currentPosition = br.BaseStream.Position;
            var romVersion = br.ReadInt16();
            RomVersion = romVersion;
            var lastUsedSwitch = br.ReadInt32();
            LastUsedSwitch = lastUsedSwitch;
            var lostRomCount = br.ReadInt32();
            var missingRomList = new List<RomItem>();
            for(var i = 0; i < lostRomCount; i++)
            {
                var guid =new Guid(br.ReadBytes(16));
                var name = br.ReadString();
                var path = br.ReadString();
                missingRomList.Add(new ResourceItem(romVersion)
                {
                    Name = name,
                    Guid = guid,
                    Path = path
                });
            }
            MissingRomList = missingRomList;
            br.BaseStream.Seek(currentPosition + chunkSize, SeekOrigin.Begin);
            if (romVersion == 22 || romVersion > 74)
                throw new NotSupportedException($"Not supported rom verison: {romVersion}");
            var romList = new List<RomItem>();
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                chunkSize = br.ReadInt32();
                var romSignature = (RomSignature)br.ReadInt16();
                currentPosition = br.BaseStream.Position;
                switch (romSignature)
                {
                    case RomSignature.Texture:
                        {
                            var rom = new Texture(romVersion);
                            rom.Load(br);
                            romList.Add(rom);
                            break;
                        }
                    default:
                        break;
                }
                br.BaseStream.Seek(currentPosition + chunkSize, SeekOrigin.Begin);
            }
            RomList = romList;
        }
    }
}
