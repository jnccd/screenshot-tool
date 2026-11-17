using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ScreenshotTool
{
    /// <summary>
    /// Caches the screenshot folder listing. Enumerating a network folder that holds tens of
    /// thousands of screenshots takes seconds, so the last listing is stored beside the user
    /// settings and shown at startup; a background scan then corrects it.
    /// </summary>
    internal static class ScreenshotFolderCache
    {
        static string CacheDirectory =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ScreenshotTool");

        static string CachePath(string folder)
        {
            using (var sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(folder.ToLowerInvariant()));
                return Path.Combine(CacheDirectory, "listing-" + BitConverter.ToString(hash, 0, 8).Replace("-", "") + ".txt");
            }
        }

        /// <summary>Newest first, matching the order the strip expects.</summary>
        internal static string[] Scan(string folder) =>
            System.IO.Directory.EnumerateFiles(folder)
                .Where(s => (s.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                             s.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)) &&
                            Path.GetFileNameWithoutExtension(s).Contains("Screenshot_"))
                .OrderByDescending(x => x, StringComparer.Ordinal)
                .ToArray();

        internal static string[] Load(string folder, out bool fromCache)
        {
            fromCache = false;
            try
            {
                string cache = CachePath(folder);
                if (File.Exists(cache))
                {
                    string[] cached = File.ReadAllLines(cache).Where(line => line.Length > 0).ToArray();
                    // A cached listing whose newest entry is gone means the folder changed
                    // behind our back (or the share is offline): scan again instead of
                    // handing out names that cannot be opened.
                    if (cached.Length > 0 && File.Exists(cached[0]))
                    {
                        fromCache = true;
                        return cached;
                    }
                }
            }
            catch
            {
                // fall back to a real scan below
            }
            return Scan(folder);
        }

        internal static void Save(string folder, string[] files)
        {
            try
            {
                System.IO.Directory.CreateDirectory(CacheDirectory);
                File.WriteAllLines(CachePath(folder), files);
            }
            catch
            {
                // caching is an optimisation, never a failure
            }
        }
    }
}
