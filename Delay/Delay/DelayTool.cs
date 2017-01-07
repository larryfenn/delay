using System;
using System.Windows;
using System.Windows.Controls;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.BassWasapi;

namespace Delay
{
    public class DelayTool
    {
        private ComboBox inlist;
        private ComboBox outlist;
        private int sourceIndex;
        private int outIndex;
        private int pushstr;
        private int outstr;
        private float latency;
        private WASAPIPROC inProc, outProc;

        public DelayTool(ComboBox inlist, ComboBox outlist)
        {
            BassNet.Registration("larry.fenn@gmail.com", "2X531420152222");
            this.inlist = inlist;
            this.outlist = outlist;
            init();
        }
        private void init()
        {
            for (int i = 0; i < BassWasapi.BASS_WASAPI_GetDeviceCount(); i++)
            {
                var device = BassWasapi.BASS_WASAPI_GetDeviceInfo(i);
                if (!device.IsEnabled) {
                    continue;
                }
                var descriptor = string.Format("{0} - {1}", i, device.name);
                Console.WriteLine(descriptor);
                Console.WriteLine("  device.IsInput: " + (device.IsInput ? "true" : "false"));
                Console.WriteLine("  device.IsLoopback: " + (device.IsLoopback ? "true" : "false"));
                Console.WriteLine("  device.IsDefault: " + (device.IsDefault ? "true" : "false"));
                Console.WriteLine("  device.IsEnabled: " + (device.IsEnabled ? "true" : "false"));
                Console.WriteLine("  device.IsDisabled: " + (device.IsDisabled ? "true" : "false"));
                Console.WriteLine("  device.IsInitialized: " + (device.IsInitialized ? "true" : "false"));
                Console.WriteLine("  device.IsUnplugged: " + (device.IsUnplugged ? "true" : "false"));
                if (device.IsLoopback) {
                    inlist.Items.Add(descriptor);
                } else if (!device.IsInput) {
                    outlist.Items.Add(descriptor);
                }
            }
            inlist.SelectedIndex = 0;
            outlist.SelectedIndex = 0;
        }
        public void start(double delay)
        {
            latency = (float)delay/10;
            var str = (inlist.Items[inlist.SelectedIndex] as string);
            var array = str.Split(' ');
            sourceIndex = Convert.ToInt32(array[0]);
            str = (outlist.Items[outlist.SelectedIndex] as string);
            array = str.Split(' ');
            outIndex = Convert.ToInt32(array[0]);
            Console.WriteLine(sourceIndex + " " + outIndex);
            Console.WriteLine(delay);

            stop();
            Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero); // "No Sound" device init

            // Garbage collector can't track the reference BassWasapi holds,
            // so we're adding a reference here to prevent garbage collection
            inProc = sourceWasapiProc;
            outProc = outWasapiProc;
            BassWasapi.BASS_WASAPI_Init(sourceIndex, 44100, 0, 0, 1, 0, inProc, IntPtr.Zero);
            BassWasapi.BASS_WASAPI_Init(outIndex, 44100, 0, 0, 4 * latency, 1 * latency, outProc, IntPtr.Zero);

            BassWasapi.BASS_WASAPI_SetDevice(outIndex);
            pushstr = Bass.BASS_StreamCreatePush(44100, 2, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT, IntPtr.Zero);
            outstr = BassMix.BASS_Mixer_StreamCreate(44100, 2, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
            BassMix.BASS_Mixer_StreamAddChannel(outstr, pushstr, 0);

            BassWasapi.BASS_WASAPI_SetDevice(sourceIndex);
            BassWasapi.BASS_WASAPI_Start();
            BassWasapi.BASS_WASAPI_SetDevice(outIndex);
            BassWasapi.BASS_WASAPI_Start();
        }

        public void stop()
        {
            BassWasapi.BASS_WASAPI_SetDevice(outIndex);
            BassWasapi.BASS_WASAPI_Stop(true);
            BassWasapi.BASS_WASAPI_Free();
            Bass.BASS_Free();
            BassWasapi.BASS_WASAPI_SetDevice(sourceIndex);
            BassWasapi.BASS_WASAPI_Stop(true);
            BassWasapi.BASS_WASAPI_Free();
            Bass.BASS_Free();

        }

        private int sourceWasapiProc(IntPtr buffer, int length, IntPtr user)
        {
            Bass.BASS_StreamPutData(pushstr, buffer, length);
            return 1;
        }

        private int outWasapiProc(IntPtr buffer, int length, IntPtr user)
        {
            return Bass.BASS_ChannelGetData(outstr, buffer, length);
        }
    }
}
