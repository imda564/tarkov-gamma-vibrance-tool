using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace tarkov_settings.Setting
{
    internal class Settings<T> where T : new()
    {
        private const string DEFAULT_FILENAME = "settings.json";

        // Anchor to the exe's own folder instead of the process's current
        // working directory, which can differ from the exe's folder depending
        // on how it was launched (shortcut "Start in", autostart entry, etc.)
        // and was the cause of "my settings don't save" reports.
        private static string ResolvePath(string fileName)
        {
            return Path.IsPathRooted(fileName)
                ? fileName
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
        }

        public void Save(string fileName = DEFAULT_FILENAME) {
            string path = ResolvePath(fileName);
            try
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                MessageBox.Show(
                    $"Failed to save settings to:\n{path}\n\n{ex.Message}\n\n" +
                    "Make sure the app isn't running from inside a zip file or a read-only/protected folder.",
                    "Settings Save Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                    );
            }
        }

        public static T Load(string fileName = DEFAULT_FILENAME)
        {
            string path = ResolvePath(fileName);
            T t = new T();
            try
            {
                if (File.Exists(path))
                {
                    T loaded = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
                    if (loaded != null)
                        t = loaded;
                }
            }
            catch (JsonException ex)
            {
                MessageBox.Show(
                    $"settings.json is invalid and will be reset to defaults:\n\n{ex.Message}",
                    "Settings Load Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                    );
                t = new T();
            }
            return t;
        }
    }
}
