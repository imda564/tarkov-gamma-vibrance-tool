using System;
using System.Net.Http;
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
            foreach (string name in appSetting.profiles.Keys)
            {
                ProfileCombo.Items.Add(name);
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

            appSetting.profiles[name] = new ColorProfile
            {
                brightness = Brightness,
                contrast = Contrast,
                gamma = Gamma,
                saturation = DVL
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
        }

        private void DeleteProfileButton_Click(object sender, EventArgs e)
        {
            string name = ProfileCombo.Text.Trim();
            if (!appSetting.profiles.Remove(name))
                return;

            appSetting.Save();
            int index = ProfileCombo.FindStringExact(name);
            if (index != -1)
                ProfileCombo.Items.RemoveAt(index);
            ProfileCombo.Text = "";
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
