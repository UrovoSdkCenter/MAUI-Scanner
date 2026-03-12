using Android.Content;
using Android.Device;
using Android.Device.Scanner.Configuration;
using Android.OS;
using Android.Util;
using MauiScanner_net8.Platforms.Android;
using MauiScanner_net8.Services;
using Application = Android.App.Application;

[assembly: Dependency(typeof(MAUIScannerManager))]
namespace MauiScanner_net8.Platforms.Android
{
    public class MAUIScannerManager : BroadcastReceiver, IMAUIScannerManager
    {
        public event EventHandler<string>? BroadcastReceived;

        private string? _action;
        private ScanManager scanManager = new();

        public void RegisterBroadcastReceiver(string action)
        {
            _action = action;
            IntentFilter filter = new(action);
            Application.Context.RegisterReceiver(this, filter);
            Log.Debug("test", "RegisterBroadcastReceiver========");

        }
        public void UnregisterBroadcastReceiver()
        {
            Log.Debug("test", "UnregisterBroadcastReceiver========");

            Application.Context.UnregisterReceiver(this);
        }
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == _action)
            {
                byte[] bytes = intent.GetByteArrayExtra("barcode");
                string str = System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                Log.Debug("test", "OnReceive========str:" + str);
                BroadcastReceived?.Invoke(this, str);
            }
        }

        public bool CloseScanner()
        {
            return scanManager.CloseScanner();
        }

        public void EnableAllSymbologies(bool enable)
        {
            scanManager.EnableAllSymbologies(enable);
        }

        public void EnableSymbology(Symbology barcodeType, bool enable)
        {
            scanManager.EnableSymbology(barcodeType, enable);
        }

        public int GetOutputMode()
        {
            return scanManager.OutputMode;
        }

        public int[] GetParameterInts(int[] idBuffer)
        {
            return scanManager.GetParameterInts(idBuffer);
        }

        public string[] GetParameterString(int[] idBuffer)
        {
            return scanManager.GetParameterString(idBuffer);
        }

        public bool GetScannerState()
        {
            return scanManager.ScannerState;
        }

        public bool GetTriggerLockState()
        {
            return scanManager.TriggerLockState;
        }

        public Triggering GetTriggerMode()
        {
            return scanManager.TriggerMode;
        }

        public bool IsSymbologyEnabled(Symbology barcodeType)
        {
            return scanManager.IsSymbologyEnabled(barcodeType);
        }

        public bool IsSymbologySupported(Symbology barcodeType)
        {
            return scanManager.IsSymbologySupported(barcodeType);
        }

        public bool LockTrigger()
        {
            return scanManager.LockTrigger();
        }

        public bool OpenScanner()
        {
            return scanManager.OpenScanner();
        }

        public bool ResetScannerParameters()
        {
            return scanManager.ResetScannerParameters();
        }

        public int SetParameterInts(int[] idBuffer, int[] valueBuffer)
        {
            return scanManager.SetParameterInts(idBuffer, valueBuffer);
        }

        public bool SetParameterString(int[] idBuffer, string[] valueBuffer)
        {
            return scanManager.SetParameterString(idBuffer, valueBuffer);
        }

        public void SetTriggerMode(Triggering mode)
        {
            scanManager.TriggerMode = mode;
        }

        public bool StartDecode()
        {
            return scanManager.StartDecode();
        }

        public bool StopDecode()
        {
            return scanManager.StopDecode();
        }

        public bool SwitchOutputMode(int mode)
        {
            return scanManager.SwitchOutputMode(mode);
        }

        public bool UnlockTrigger()
        {
            return scanManager.UnlockTrigger();
        }

    }
}