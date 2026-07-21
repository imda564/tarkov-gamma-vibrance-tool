using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using tarkov_settings.Setting;
using tarkov_settings.GPU;

namespace tarkov_settings
{
    public partial class MainForm : Form
    {
        private ProcessMonitor pMonitor = ProcessMonitor.Instance;
        private IGPU gpu = GPUDevice.Instance;
        private AppSetting appSetting;

        private bool minimizeOnStart = false;

        #region Global Hotkeys
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int WM_HOTKEY = 0x0312;
        private const uint MOD_ALT = 0x1;
        private const uint MOD_CONTROL = 0x2;
        private const uint MOD_SHIFT = 0x4;

        private readonly Dictionary<string, int> profileHotkeyIds = new Dictionary<string, int>();
        private readonly Dictionary<int, string> hotkeyIdToProfile = new Dictionary<int, string>();
        private int nextHotkeyId = 100;
        private bool awaitingHotkey = false;
        #endregion

        public MainForm()
        {
            InitializeComponent();

            #region Load App Settings
            // Load Settings
            appSetting = AppSetting.Load();

            Brightness = appSetting.brightness;
            Contrast = appSetting.contrast;
            Gamma = appSetting.gamma;
            DVL = appSetting.saturation;
            minimizeOnStart = appSetting.minimizeOnStart;
            this.minimizeStartCheckBox.Checked = minimizeOnStart;
            #endregion
            
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            this.Text = String.Format("Tarkov Settings {0}", version);
            _ = new UpdateNotifier(version);

            // Saturation Initialize
            if (gpu.Vendor != GPUVendor.NVIDIA)
                DVLGroupBox.Enabled = false;

            #region Initialize Display
            // Initialize Display Dropdown
            foreach (string display in Display.displays)
            {
                DisplayCombo.Items.Add(display);
            }
            
            if(DisplayCombo.FindString(appSetting.display) != -1)
                DisplayCombo.SelectedIndex = DisplayCombo.FindString(appSetting.display);

            Display.Primary = (string)DisplayCombo.SelectedItem;
            #endregion

            // Initialize Process Monitor
            pMonitor.Parent = this;
            foreach (string pTarget in appSetting.pTargets)
            {
                pMonitor.Add(pTarget.ToLower());
            }
            pMonitor.Init();

            #region Initialize Profiles
            foreach (KeyValuePair<string, ColorProfile> entry in appSetting.profiles)
            {
                ProfileCombo.Items.Add(entry.Key);
                if (entry.Value.hotkey != 0)
                    RegisterProfileHotkey(entry.Key, entry.Value.hotkey);
            }
            #endregion
        }

        #region BCGS Getter/Setter
        public double Brightness
        {
            get => BrightnessBar.Value / 100.0;
            set => BrightnessBar.Value = (int)(value * 100);
        }

        public double Contrast
        {
            get => ContrastBar.Value / 100.0;
            set => ContrastBar.Value = (int)(value * 100);
        }

        public double Gamma
        {
            get => GammaBar.Value / 100.0;
            set => GammaBar.Value = (int)(value * 100);
        }

        public int DVL
        {
            get => DVLBar.Value;
            set => DVLBar.Value = value;
        }

        public (double, double, double, int) GetColorValue()
        {
            return (
                BrightnessBar.Value / 100.0,
                ContrastBar.Value / 100.0,
                GammaBar.Value / 100.0,
                DVLBar.Value
                );
        }
        #endregion

        public bool IsEnabled { get=> this.enableToolStripMenuItem.Checked;}

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (minimizeOnStart)
            {
                this.Visible = false;
                this.ShowInTaskbar = false;
                this.trayIcon.ShowBalloonTip(
                    2500,
                    "Tarkov Settings Initailized!",
                    "Check out tray to modify your color setting",
                    ToolTipIcon.Info
                    );
            }
        }

        #region Control Event Handlers
        private void ColorLabel_DClick(object sender, EventArgs e)
        {
            var label = sender as Label;
            
            if (label.Equals(BrightnessLabel))
            {
                BrightnessBar.Value = 50;
            }
            else if (label.Equals(ContrastLabel))
            {
                ContrastBar.Value = 50;
            }
            else if (label.Equals(GammaLabel))
            {
                GammaBar.Value = 100;
            }
            else if (label.Equals(DVLLabel))
            {
                DVLBar.Value = 0;
            }
        }
        private void TrackBar_ValueChanged(object sender, EventArgs e)
        {
            var trackBar = sender as TrackBar;

            if (trackBar.Equals(BrightnessBar))
            {
                BrightnessText.Text = (BrightnessBar.Value / 100.0).ToString("0.00");
            }
            else if (trackBar.Equals(ContrastBar))
            {
                ContrastText.Text = (ContrastBar.Value / 100.0).ToString("0.00");
            }
            else if (trackBar.Equals(GammaBar))
            {
                GammaText.Text = (GammaBar.Value / 100.0).ToString("0.00");
            }
            else if (trackBar.Equals(DVLBar))
            {
                DVLText.Text = DVLBar.Value.ToString();
            }
        }
        private void DisplayCombo_SelectedValueChanged(object sender, EventArgs e)
        {
            string selectedDisplay = (string)DisplayCombo.SelectedItem;
            Display.Primary = selectedDisplay;

            if(Display.Primary != selectedDisplay)
            {
                DisplayCombo.SelectedIndex = DisplayCombo.FindString(Display.Primary);
            }
        }

        private void SaveProfileButton_Click(object sender, EventArgs e)
        {
            string name = ProfileCombo.Text.Trim();
            if (name.Length == 0)
            {
                MessageBox.Show("Type a profile name in the box before saving.", "Save Profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Preserve an already-assigned hotkey when re-saving an existing profile
            int existingHotkey = appSetting.profiles.TryGetValue(name, out ColorProfile existing) ? existing.hotkey : 0;

            appSetting.profiles[name] = new ColorProfile
            {
                brightness = Brightness,
                contrast = Contrast,
                gamma = Gamma,
                saturation = DVL,
                hotkey = existingHotkey
            };
            appSetting.Save();

            if (ProfileCombo.FindStringExact(name) == -1)
                ProfileCombo.Items.Add(name);
        }

        private void LoadProfileButton_Click(object sender, EventArgs e)
        {
            string name = ProfileCombo.Text.Trim();
            if (!appSetting.profiles.TryGetValue(name, out ColorProfile profile))
            {
                MessageBox.Show(String.Format("No saved profile named \"{0}\".", name), "Load Profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Brightness = profile.brightness;
            Contrast = profile.contrast;
            Gamma = profile.gamma;
            DVL = profile.saturation;
            HotkeyText.Text = FormatHotkey((Keys)profile.hotkey);
        }

        private void DeleteProfileButton_Click(object sender, EventArgs e)
        {
            string name = ProfileCombo.Text.Trim();
            if (!appSetting.profiles.Remove(name))
                return;

            UnregisterProfileHotkey(name);
            appSetting.Save();
            int index = ProfileCombo.FindStringExact(name);
            if (index != -1)
                ProfileCombo.Items.RemoveAt(index);
            ProfileCombo.Text = "";
            HotkeyText.Text = "None";
        }

        private void ProfileCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string name = ProfileCombo.Text.Trim();
            HotkeyText.Text = appSetting.profiles.TryGetValue(name, out ColorProfile profile)
                ? FormatHotkey((Keys)profile.hotkey)
                : "None";
        }

        private void SetHotkeyButton_Click(object sender, EventArgs e)
        {
            string name = ProfileCombo.Text.Trim();
            if (!appSetting.profiles.ContainsKey(name))
            {
                MessageBox.Show("Save the profile first, then assign a hotkey to it.", "Set Hotkey", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            awaitingHotkey = true;
            HotkeyText.Text = "Press keys...";
        }

        private void ClearHotkeyButton_Click(object sender, EventArgs e)
        {
            string name = ProfileCombo.Text.Trim();
            if (!appSetting.profiles.TryGetValue(name, out ColorProfile profile))
                return;

            UnregisterProfileHotkey(name);
            profile.hotkey = 0;
            appSetting.Save();
            HotkeyText.Text = "None";
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (!awaitingHotkey)
                return;

            // Wait for an actual key, not just a modifier being held down
            if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey ||
                e.KeyCode == Keys.Menu || e.KeyCode == Keys.LWin || e.KeyCode == Keys.RWin)
                return;

            e.Handled = true;
            e.SuppressKeyPress = true;
            awaitingHotkey = false;

            string name = ProfileCombo.Text.Trim();
            if (!appSetting.profiles.TryGetValue(name, out ColorProfile profile))
                return;

            UnregisterProfileHotkey(name);

            int combo = (int)e.KeyData;
            if (!RegisterProfileHotkey(name, combo))
            {
                HotkeyText.Text = "None";
                MessageBox.Show(
                    String.Format("\"{0}\" is already in use by another application.", FormatHotkey((Keys)combo)),
                    "Set Hotkey", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            profile.hotkey = combo;
            appSetting.Save();
            HotkeyText.Text = FormatHotkey((Keys)combo);
        }

        private bool RegisterProfileHotkey(string name, int hotkeyValue)
        {
            if (hotkeyValue == 0)
                return true;

            Keys keys = (Keys)hotkeyValue;
            uint modifiers = 0;
            if ((keys & Keys.Control) == Keys.Control) modifiers |= MOD_CONTROL;
            if ((keys & Keys.Alt) == Keys.Alt) modifiers |= MOD_ALT;
            if ((keys & Keys.Shift) == Keys.Shift) modifiers |= MOD_SHIFT;
            uint vk = (uint)(keys & Keys.KeyCode);

            int id = nextHotkeyId++;
            if (!RegisterHotKey(this.Handle, id, modifiers, vk))
                return false;

            profileHotkeyIds[name] = id;
            hotkeyIdToProfile[id] = name;
            return true;
        }

        private void UnregisterProfileHotkey(string name)
        {
            if (profileHotkeyIds.TryGetValue(name, out int id))
            {
                UnregisterHotKey(this.Handle, id);
                profileHotkeyIds.Remove(name);
                hotkeyIdToProfile.Remove(id);
            }
        }

        private static string FormatHotkey(Keys keys)
        {
            if (keys == Keys.None)
                return "None";

            var parts = new List<string>();
            if ((keys & Keys.Control) == Keys.Control) parts.Add("Ctrl");
            if ((keys & Keys.Alt) == Keys.Alt) parts.Add("Alt");
            if ((keys & Keys.Shift) == Keys.Shift) parts.Add("Shift");
            parts.Add((keys & Keys.KeyCode).ToString());
            return String.Join("+", parts);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();
                if (hotkeyIdToProfile.TryGetValue(id, out string name) &&
                    appSetting.profiles.TryGetValue(name, out ColorProfile profile))
                {
                    Brightness = profile.brightness;
                    Contrast = profile.contrast;
                    Gamma = profile.gamma;
                    DVL = profile.saturation;
                }
            }
            base.WndProc(ref m);
        }
        #endregion

        private void ShowForm(object sender, EventArgs e)
        {
            this.Visible = true;
            this.ShowInTaskbar = true;
        }

        private void SaveAppSettings()
        {
            appSetting.brightness = Brightness;
            appSetting.contrast = Contrast;
            appSetting.gamma = Gamma;
            appSetting.saturation = DVL;
            appSetting.display = (string)DisplayCombo.SelectedItem;
            appSetting.minimizeOnStart = minimizeOnStart;
            appSetting.Save();
        }

        private void ExitFormClicked(object sender, EventArgs e)
        {
            SaveAppSettings();
            Application.Exit();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // Persist settings here too - most users close the window with the
                // X button and never learn about "Exit" in the tray menu, so relying
                // on that path alone made changes look like they never saved.
                SaveAppSettings();
                e.Cancel = true;
                this.Hide();
            }
            else
            {
                Console.WriteLine(e.CloseReason);
                this.trayIcon.Dispose();
                foreach (string name in new List<string>(profileHotkeyIds.Keys))
                    UnregisterProfileHotkey(name);
                Console.WriteLine("[mainForm] Closing pMonitor");
                pMonitor.Close();
            }
        }

        private void CheckOnMinimizeToTray(object sender, EventArgs e)
        {
            this.minimizeOnStart = this.minimizeStartCheckBox.Checked;
        }
    }
}
