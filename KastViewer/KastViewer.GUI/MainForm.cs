using KastViewer.Core;
using System.ComponentModel;

namespace KastViewer.GUI
{
    public partial class MainForm : Form
    {

        private class ExportBackgroundTask(
            List<KeyValuePair<Texture, TextureInternal>> textureList,
            string saveDirectory,
            BackgroundWorker backgroundWorker
        )
        {
            public int SavedFileCount { get; private set; }
            public int ProcessedFileCount { get; private set; }
            public int TotalFileCount { get; private set; } = textureList.Count;
            public void Run()
            {
                ProcessedFileCount = 0;
                SavedFileCount = 0;
                foreach (var entry in textureList)
                {
                    if (backgroundWorker.CancellationPending)
                        break;
                    ProcessedFileCount++;
                    var savePath = Path.Join(saveDirectory, entry.Key.Path[2..]);
                    var parent = Path.GetDirectoryName(savePath);
                    if (parent != null && !Directory.Exists(parent))
                        Directory.CreateDirectory(parent);
                    try
                    {
                        TextureInternalToImageConverter.Convert(entry.Value).Save(savePath);
                        SavedFileCount++;
                    }
                    catch (Exception) { }
                    backgroundWorker.ReportProgress(Convert.ToInt32(100 * (ProcessedFileCount / (double)TotalFileCount)),this);
                }
            }
        }
        public MainForm()
        {
            InitializeComponent();
        }
        private readonly List<KeyValuePair<Texture, TextureInternal>> mTextureList = [];

        private void openDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                var root = openFolderBrowserDialog.SelectedPath;
                var romMainfestPath = Path.Combine(root, "unpack.zip", "ResourceItem.rbr");
                if (!Path.Exists(romMainfestPath))
                {
                    MessageBox.Show($"File {romMainfestPath} not exists");
                    return;
                }
                mTextureList.Clear();
                var romManifest = new RomManifest();
                using (var br = new BinaryReader(File.OpenRead(romMainfestPath)))
                {
                    try
                    {
                        romManifest.Load(br);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to parse ResourceItem.rbr:{ex.Message}");
                        return;
                    }
                }
                var romList = romManifest.RomList;
                if (romList == null) return;
                foreach (var romItem in romList)
                {
                    var texture = romItem as Texture;
                    if (texture == null) continue;
                    var guid = romItem.Guid.ToString("N");
                    var kastFilePath = Path.Combine(
                        root, "library", "assets", "pc", guid[0..2],
                        guid + ".kast");
                    var rawFilePath = Path.Combine(root, texture.Path);
                    if (!(Path.Exists(kastFilePath) && File.Exists(rawFilePath)))
                        continue;
                    var textureInternal = new TextureInternal();
                    textureInternal.Load(kastFilePath);
                    mTextureList.Add(new KeyValuePair<Texture, TextureInternal>(texture, textureInternal));
                }
                listView1.Items.Clear();
                foreach (var pair in mTextureList)
                {
                    listView1.Items.Add(new ListViewItem([
                        pair.Key.Name,
                        pair.Value.Size.ToString(),
                        pair.Key.Path
                    ]));
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count == 0) return;
            var selectedIndex = listView1.SelectedIndices[0];
            var texture = mTextureList[selectedIndex];
            Logger.Info($"Texture {{ name:{texture.Key.Name},path:{texture.Key.Path},file:{texture.Value.FilePath},width:{texture.Value.Width},height:{texture.Value.Height},format:{texture.Value.Format} }}");
            try
            {
                pictureBox1.Image = TextureInternalToImageConverter.Convert(texture.Value);
                toolStripStatusLabel1.Text = "";
            }
            catch (Exception ex)
            {
                pictureBox1.Image = null;
                toolStripStatusLabel1.Text = "Failed to open texture";
                Logger.Error("Failed to open texture", ex);
            }
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                foreach (ListViewItem item in listView1.Items)
                    item.Selected = true;
            }
            else if (e.Control && e.KeyCode == Keys.I)
            {
                listView1.BeginUpdate();
                for (int i = 0; i < listView1.Items.Count; i++)
                    listView1.Items[i].Selected = !listView1.Items[i].Selected;
                listView1.EndUpdate();
            }
        }

        private void exportAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                var saveDirectory = saveFolderBrowserDialog.SelectedPath;
                var task = new ExportBackgroundTask(mTextureList, saveDirectory,exportBackgroundWorker);
                exportBackgroundWorker.RunWorkerAsync(task);
            }
        }

        private void exportSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                var saveDirectory = saveFolderBrowserDialog.SelectedPath;
                var selectedTextures = new List<KeyValuePair<Texture, TextureInternal>>();
                for (int i = 0; i < listView1.SelectedIndices.Count; i++)
                    selectedTextures.Add(mTextureList[listView1.SelectedIndices[i]]);
                var task = new ExportBackgroundTask(selectedTextures, saveDirectory, exportBackgroundWorker);
                exportBackgroundWorker.RunWorkerAsync(task);
            }
        }

        private void exportBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var task = e.Argument as ExportBackgroundTask;
            task?.Run();
        }

        private void exportBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is ExportBackgroundTask task)
            {
                if (task.ProcessedFileCount < task.TotalFileCount)
                    toolStripStatusLabel1.Text = $"Exporting {task.ProcessedFileCount}/{task.TotalFileCount}";
                else
                    toolStripStatusLabel1.Text = $"{task.SavedFileCount} of {task.TotalFileCount} files exported";
            }
        }

        private void exportBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e){}
    }
}
