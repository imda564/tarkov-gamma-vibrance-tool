using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.Display.Structures;

namespace tarkov_settings.GPU
{
    class NVIDIA : IGPU
    {
        private GPUVendor _vendor;
        private DisplayHandle displayHandle;

        private int _maxSaturation;
        private int _minSaturation;
        private int _initSaturation;
        private int currentSaturation;

        public GPUVendor Vendor
        {
            get => this._vendor;
        }

        public int MaxSaturation
        {
            get => _maxSaturation;
        }

        public int MinSaturation
        {
            get => _minSaturation;
        }

        public int InitSaturation
        {
            get => _initSaturation;
        }

        public int Saturation
        {
            get => currentSaturation;
            set
            {
                if (value > this.MaxSaturation)
                    value = this.MaxSaturation;
                if (value < this.MinSaturation)
                    value = this.MinSaturation;

                try
                {
                    DisplayApi.SetDVCLevel(displayHandle, value);
                    this.currentSaturation = value;
                }
                catch (NvAPIWrapper.Native.Exceptions.NVIDIAApiException)
                {
                    // Display handle went stale (RDP, driver reset, monitor hotplug).
                    // Swallow instead of crashing the caller (often a WinEventHook callback).
                }
            }
        }

        public NVIDIA(GPUVendor vendor)
        {
            try
            { 
                NvAPIWrapper.NVIDIA.Initialize();
            }
            catch (NvAPIWrapper.Native.Exceptions.NVIDIAApiException)
            {
                MessageBox.Show("NvAPI Intialize Failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this._vendor = vendor;
        }

        public void ResetSaturation()
        {
            this.Saturation = this.InitSaturation;
        }

        public void Load(string display) {
            try
            {
                displayHandle = DisplayApi.GetAssociatedNvidiaDisplayHandle(display);
                PrivateDisplayDVCInfo dvcInfo = DisplayApi.GetDVCInfo(displayHandle);
                this._maxSaturation = dvcInfo.MaximumLevel;
                this._minSaturation = dvcInfo.MinimumLevel;
                this._initSaturation = this.currentSaturation = dvcInfo.CurrentLevel;
            }
            catch (NvAPIWrapper.Native.Exceptions.NVIDIAApiException)
            {
                // No valid NVIDIA display handle for this monitor right now
                // (e.g. mid RDP session). Leave previous values in place.
            }
        }

        public void Close() {
            try
            {
                NvAPIWrapper.NVIDIA.Unload();
            }
            catch (NvAPIWrapper.Native.Exceptions.NVIDIAApiException)
            {
                MessageBox.Show("NvAPI Unload Failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
