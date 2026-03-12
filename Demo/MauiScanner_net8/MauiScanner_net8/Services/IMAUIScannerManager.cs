namespace MauiScanner_net8.Services
{
    using Android.Device.Scanner.Configuration;

    public interface IMAUIScannerManager
    {
        void RegisterBroadcastReceiver(string action);
        void UnregisterBroadcastReceiver();
        event EventHandler<string> BroadcastReceived;

        bool CloseScanner();
        void EnableAllSymbologies(bool enable);
        void EnableSymbology(Symbology barcodeType, bool enable);
        int GetOutputMode();
        int[] GetParameterInts(int[] idBuffer);
        string[] GetParameterString(int[] idBuffer);
        bool GetScannerState();
        bool GetTriggerLockState();
        Triggering GetTriggerMode();
        bool IsSymbologyEnabled(Symbology barcodeType);
        bool IsSymbologySupported(Symbology barcodeType);
        bool LockTrigger();
        bool OpenScanner();
        bool ResetScannerParameters();
        int SetParameterInts(int[] idBuffer, int[] valueBuffer);
        bool SetParameterString(int[] idBuffer, string[] valueBuffer);
        void SetTriggerMode(Triggering mode);
        bool StartDecode();
        bool StopDecode();
        bool SwitchOutputMode(int mode);
        bool UnlockTrigger();
    }
}
