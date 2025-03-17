
namespace KastViewer.Core
{
    public class ResourceItem(short romVersion) : RomItem(romVersion)
    {
        public string Path { get; internal set; } = string.Empty;
        public override void Load(BinaryReader br)
        {
            base.Load(br);
            Path = br.ReadString();
        }
    }
}
