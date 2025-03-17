namespace KastViewer.Core
{
    public class RomItem(short romVersion)
    {
        public short RomVersion { get; internal set; } = romVersion;
        public string Name { get; internal set; } = string.Empty;
        public string? Tags { get; internal set; }
        public string? Category { get; internal set; }
        public Guid Guid { get; internal set; }

        public virtual void Load(BinaryReader br)
        {
            if (RomVersion < 6)
            {
                Name = br.ReadString();
                Guid = new Guid(br.ReadBytes(16));
            }
            else
            {
                Guid = new Guid(br.ReadBytes(16));
                Name = br.ReadString();
            }
            if (RomVersion >= 23)
            {
                Tags = br.ReadString();
                Category = br.ReadString();
            }
        }
    }
}
