using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenshotTool
{
    public class Screenshot
    {
        readonly object imageLock = new object();

        private Bitmap image;
        private bool loading;
        private int cacheGeneration;

        /// <summary>
        /// The image if it is already in memory, otherwise null. This never touches
        /// the disk, so it is safe to use from the UI thread - reading the file can
        /// take seconds while the drive it lives on (a spun down NAS) wakes up.
        /// </summary>
        public Bitmap CachedImage
        {
            get { lock (imageLock) return image; }
        }

        /// <summary>True when the file could not be read the last time we tried.</summary>
        public bool LoadFailed { get; private set; }

        public string FileName { get; private set; }
        public bool Saved { get; set; }
        public string Path { get; private set; }
        private bool _saving = false;

        public Screenshot(Bitmap Image, string FileName)
        {
            this.image = Image;
            this.FileName = FileName;
            Saved = false;
        }
        public Screenshot(string path)
        {
            FileName = System.IO.Path.GetFileNameWithoutExtension(path);
            Saved = true;
            Path = path;
        }

        /// <summary>
        /// Reads the image off the disk. This blocks until the file has been read,
        /// so it must only be called from a background thread. A second thread
        /// asking for the same image while it is loading gets null instead of
        /// starting a second read - the first one still updates the cache.
        /// </summary>
        public Bitmap LoadOnCurrentThread()
        {
            string path;
            int generation;
            lock (imageLock)
            {
                if (image != null)
                    return image;
                if (loading)
                    return null;
                if (Path.IsNullOrWhiteSpace())
                    return null;

                loading = true;
                generation = cacheGeneration;
                path = Path;
            }

            Bitmap loaded = null;
            try
            {
                loaded = (Bitmap)Bitmap.FromFile(path);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Could not load {path}: {e.Message}");
            }

            lock (imageLock)
            {
                loading = false;
                if (loaded == null)
                {
                    LoadFailed = true;
                    return null;
                }
                if (generation != cacheGeneration)
                {
                    // The cache was dropped while we were reading, so this bitmap
                    // belongs to nobody - hand it to the GC instead of the cache.
                    loaded.Dispose();
                    return null;
                }
                image = loaded;
                LoadFailed = false;
                return loaded;
            }
        }

        public void Save()
        {
            if (Saved)
                return;     // already on disk, nothing to write

            _saving = true;
            try
            {
                Bitmap source = CachedImage ?? LoadOnCurrentThread();
                if (source == null)
                    return;

                Bitmap savingImage;
                lock (imageLock)
                    savingImage = (Bitmap)source.Clone();

                Path = config.Default.path + "\\" + FileName + ".png";
                savingImage.Save(Path);
                savingImage.Dispose();
                Saved = true;
            }
            finally
            {
                _saving = false;
            }
        }
        public void PutInClipboard()
        {
            Bitmap cached = CachedImage;
            if (cached == null)
                return;     // still on its way from disk, the caller has to wait for it

            lock (imageLock)
            {
                Clipboard.SetImage(cached);
            }
        }
        public void DisposeImageCache()
        {
            lock (imageLock)
            {
                if (Saved && image != null)
                {
                    image.Dispose();
                    image = null;
                }
                // Invalidate reads that are still in flight so they cannot
                // repopulate the cache we just dropped.
                cacheGeneration++;
                LoadFailed = false;
            }
        }
        public void Delete()
        {
            while (_saving)
                Task.Delay(200).Wait();
            lock (this)
            {
                try
                {
                    if (File.Exists(Path))
                        FileSystem.DeleteFile(Path,
                                UIOption.OnlyErrorDialogs,
                                RecycleOption.SendToRecycleBin,
                                UICancelOption.ThrowException);
                }
                catch (Exception e) { Debug.WriteLine(e); }
            }
        }
    }
}
