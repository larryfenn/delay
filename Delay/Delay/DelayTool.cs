using System;
using System.Windows;
using System.Windows.Controls;
using Un4seen.Bass;
using Un4seen.BassWasapi;

namespace Delay
{
    public class DelayTool
    {
        private ComboBox inlist;
        private ComboBox outlist;
        private bool initialized;
        public DelayTool(ComboBox inlist, ComboBox outlist)
        {
            BassNet.Registration("larry.fenn@gmail.com", "2X531420152222");
            this.inlist = inlist;
            this.outlist = outlist;
            initialized = false;
            init();
        }
        private void init()
        {
            bool result = false;
            for (int i = 0; i < BassWasapi.BASS_WASAPI_GetDeviceCount(); i++)
            {
                var device = BassWasapi.BASS_WASAPI_GetDeviceInfo(i);
                if (device.IsEnabled && device.IsLoopback)
                {
                    inlist.Items.Add(string.Format("{0} - {1}", i, device.name));
                    outlist.Items.Add(string.Format("{0} - {1}", i, device.name));
                }
            }
            inlist.SelectedIndex = 0;
            outlist.SelectedIndex = 0;
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, false);
            // 'No sound' device for double buffering: BASS_WASAPI_BUFFER flag requirement
            result = Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            if (!result) throw new Exception("Init Error");
        }
    }
}
