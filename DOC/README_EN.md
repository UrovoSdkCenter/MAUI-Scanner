# UROVO Scanner SDK for .NET MAUI

.NET MAUI wrapper for UROVO barcode scanner SDK, providing barcode scanning functionality support.

## Platform Support

- Android

## Features

- **Scan Control**: Start/stop scanning, open/close scanner
- **Broadcast Receiving**: Receive scan results via Android broadcast
- **Output Mode**: Broadcast mode, text box output mode
- **Trigger Control**: Trigger mode settings, trigger key lock
- **Symbol Management**: Enable/disable specific barcode types
- **Parameter Configuration**: Integer and string parameter configuration
- **Event-driven**: Receive scan results via EventHandler

## Requirements

- **.NET**: 8.0+
- **MAUI**: .NET MAUI workload
- **Android SDK**: API 19+
- **Dependencies**: android.device.ScanManager

## Project Structure

```
Services/
  └── IMAUIScannerManager.cs    # Scanner service interface definition

Platforms/
  └── Android/
      └── MAUIScannerManager.cs  # Android platform scanner implementation
```

## Quick Start

### 1. Define Service Interface

```csharp
using Android.Device.Scanner.Configuration;
using System;

namespace YourApp.Services
{
    public interface IMAUIScannerManager
    {
        // Broadcast management
        void RegisterBroadcastReceiver(string action);
        void UnregisterBroadcastReceiver();
        event EventHandler<string> BroadcastReceived;

        // Scanner control
        bool OpenScanner();
        bool CloseScanner();
        bool GetScannerState();
        bool StartDecode();
        bool StopDecode();

        // Output mode
        bool SwitchOutputMode(int mode);
        int GetOutputMode();

        // Trigger control
        void SetTriggerMode(Triggering mode);
        Triggering GetTriggerMode();
        bool LockTrigger();
        bool UnlockTrigger();
        bool GetTriggerLockState();

        // Symbol management
        void EnableAllSymbologies(bool enable);
        void EnableSymbology(Symbology barcodeType, bool enable);
        bool IsSymbologyEnabled(Symbology barcodeType);
        bool IsSymbologySupported(Symbology barcodeType);

        // Parameter configuration
        int SetParameterInts(int[] idBuffer, int[] valueBuffer);
        int[] GetParameterInts(int[] idBuffer);
        bool SetParameterString(int[] idBuffer, string[] valueBuffer);
        string[] GetParameterString(int[] idBuffer);
        bool ResetScannerParameters();
    }
}
```

### 2. Implement Android Platform Service

```csharp
using Android.Content;
using Android.Device;
using Android.Device.Scanner.Configuration;
using Android.Util;
using YourApp.Services;
using Application = Android.App.Application;

[assembly: Dependency(typeof(YourApp.Platforms.Android.MAUIScannerManager))]
namespace YourApp.Platforms.Android
{
    public class MAUIScannerManager : BroadcastReceiver, IMAUIScannerManager
    {
        public event EventHandler<string>? BroadcastReceived;

        private string? _action;
        private ScanManager scanManager = new();

        // Register broadcast receiver
        public void RegisterBroadcastReceiver(string action)
        {
            _action = action;
            IntentFilter filter = new(action);
            Application.Context.RegisterReceiver(this, filter);
            Log.Debug("Scanner", "RegisterBroadcastReceiver");
        }

        // Unregister broadcast receiver
        public void UnregisterBroadcastReceiver()
        {
            Log.Debug("Scanner", "UnregisterBroadcastReceiver");
            Application.Context.UnregisterReceiver(this);
        }

        // Receive broadcast
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == _action)
            {
                byte[] bytes = intent.GetByteArrayExtra("barcode");
                string str = System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                Log.Debug("Scanner", "OnReceive str:" + str);
                BroadcastReceived?.Invoke(this, str);
            }
        }

        // Open scanner
        public bool OpenScanner()
        {
            return scanManager.OpenScanner();
        }

        // Close scanner
        public bool CloseScanner()
        {
            return scanManager.CloseScanner();
        }

        // Get scanner state
        public bool GetScannerState()
        {
            return scanManager.ScannerState;
        }

        // Start scanning
        public bool StartDecode()
        {
            return scanManager.StartDecode();
        }

        // Stop scanning
        public bool StopDecode()
        {
            return scanManager.StopDecode();
        }

        // Switch output mode
        public bool SwitchOutputMode(int mode)
        {
            return scanManager.SwitchOutputMode(mode);
        }

        // Get output mode
        public int GetOutputMode()
        {
            return scanManager.OutputMode;
        }

        // Set trigger mode
        public void SetTriggerMode(Triggering mode)
        {
            scanManager.TriggerMode = mode;
        }

        // Get trigger mode
        public Triggering GetTriggerMode()
        {
            return scanManager.TriggerMode;
        }

        // Lock trigger key
        public bool LockTrigger()
        {
            return scanManager.LockTrigger();
        }

        // Unlock trigger key
        public bool UnlockTrigger()
        {
            return scanManager.UnlockTrigger();
        }

        // Get trigger lock state
        public bool GetTriggerLockState()
        {
            return scanManager.TriggerLockState;
        }

        // Enable/disable all symbologies
        public void EnableAllSymbologies(bool enable)
        {
            scanManager.EnableAllSymbologies(enable);
        }

        // Enable/disable specific symbology
        public void EnableSymbology(Symbology barcodeType, bool enable)
        {
            scanManager.EnableSymbology(barcodeType, enable);
        }

        // Check if symbology is enabled
        public bool IsSymbologyEnabled(Symbology barcodeType)
        {
            return scanManager.IsSymbologyEnabled(barcodeType);
        }

        // Check if symbology is supported
        public bool IsSymbologySupported(Symbology barcodeType)
        {
            return scanManager.IsSymbologySupported(barcodeType);
        }

        // Set integer parameters
        public int SetParameterInts(int[] idBuffer, int[] valueBuffer)
        {
            return scanManager.SetParameterInts(idBuffer, valueBuffer);
        }

        // Get integer parameters
        public int[] GetParameterInts(int[] idBuffer)
        {
            return scanManager.GetParameterInts(idBuffer);
        }

        // Set string parameters
        public bool SetParameterString(int[] idBuffer, string[] valueBuffer)
        {
            return scanManager.SetParameterString(idBuffer, valueBuffer);
        }

        // Get string parameters
        public string[] GetParameterString(int[] idBuffer)
        {
            return scanManager.GetParameterString(idBuffer);
        }

        // Reset scanner parameters
        public bool ResetScannerParameters()
        {
            return scanManager.ResetScannerParameters();
        }
    }
}
```

### 3. Use in Page

```csharp
using YourApp.Services;

namespace YourApp
{
    public partial class MainPage : ContentPage
    {
        IMAUIScannerManager scannerManager;
        bool isScanning = false;

        public MainPage()
        {
            InitializeComponent();
            
            // Get scanner service
            scannerManager = DependencyService.Get<IMAUIScannerManager>();
            
            // Register event
            scannerManager.BroadcastReceived += OnBroadcastReceived;
            
            // Set output mode to broadcast mode
            scannerManager.SwitchOutputMode(0);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            // Register broadcast receiver
            scannerManager?.RegisterBroadcastReceiver("android.intent.ACTION_DECODE_DATA");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            
            // Unregister broadcast receiver
            scannerManager?.UnregisterBroadcastReceiver();
        }

        // Scan result callback
        private void OnBroadcastReceived(object sender, string data)
        {
            isScanning = false;
            InfoLabel.Text = $"Scan result: {data}";
        }

        // Start/stop scan button
        private void OnScanClicked(object sender, EventArgs e)
        {
            if (isScanning)
            {
                scannerManager.StopDecode();
            }
            else
            {
                scannerManager.OpenScanner();
                scannerManager.StartDecode();
            }
            
            isScanning = !isScanning;
            ScanButton.Text = isScanning ? "Stop Scan" : "Start Scan";
        }
    }
}
```

## API Documentation

### IMAUIScannerManager Interface

#### Events

##### BroadcastReceived
Scan result event

```csharp
event EventHandler<string> BroadcastReceived;
```

**Event Parameters:**
- `string`: Scanned barcode content

#### Broadcast Management

##### RegisterBroadcastReceiver(string action)
Register broadcast receiver

```csharp
scannerManager.RegisterBroadcastReceiver("android.intent.ACTION_DECODE_DATA");
```

**Parameters:**
- `action`: Broadcast Action, usually `"android.intent.ACTION_DECODE_DATA"`

**Note:** Must be called when page appears (`OnAppearing`)

##### UnregisterBroadcastReceiver()
Unregister broadcast receiver

```csharp
scannerManager.UnregisterBroadcastReceiver();
```

**Note:** Must be called when page disappears (`OnDisappearing`) to avoid memory leaks

#### Scanner Control

##### OpenScanner()
Power on scanner

```csharp
bool isOpen = scannerManager.OpenScanner();
```

**Returns:** `true`=success, `false`=failure

##### CloseScanner()
Power off scanner

```csharp
bool isClosed = scannerManager.CloseScanner();
```

**Returns:** `true`=success, `false`=failure

##### GetScannerState()
Get scanner power state

```csharp
bool isOn = scannerManager.GetScannerState();
```

**Returns:** `true`=powered on, `false`=powered off

##### StartDecode()
Start scanning

```csharp
bool started = scannerManager.StartDecode();
```

**Returns:** `true`=success, `false`=failure

**Note:** Scan result is returned via `BroadcastReceived` event

##### StopDecode()
Stop scanning

```csharp
bool stopped = scannerManager.StopDecode();
```

**Returns:** `true`=success, `false`=failure

#### Output Mode

##### SwitchOutputMode(int mode)
Switch output mode

```csharp
bool switched = scannerManager.SwitchOutputMode(0);
```

**Parameters:**
- `mode`: Output mode
  - `0`: Broadcast mode (recommended)
  - `1`: Text box output mode

**Returns:** `true`=success, `false`=failure

##### GetOutputMode()
Get current output mode

```csharp
int mode = scannerManager.GetOutputMode();
```

**Returns:** `0`=broadcast mode, `1`=text box mode

#### Trigger Control

##### SetTriggerMode(Triggering mode)
Set trigger mode

```csharp
using Android.Device.Scanner.Configuration;

scannerManager.SetTriggerMode(Triggering.Host);
```

**Parameters:**
- `Triggering.Host`: Host trigger (via code)
- `Triggering.Level`: Level trigger
- `Triggering.Pulse`: Pulse trigger

##### GetTriggerMode()
Get current trigger mode

```csharp
Triggering mode = scannerManager.GetTriggerMode();
```

**Returns:** Current trigger mode

##### LockTrigger()
Lock scan trigger key

```csharp
bool locked = scannerManager.LockTrigger();
```

**Returns:** `true`=success, `false`=failure

**Note:** Physical scan button will be disabled after locking

##### UnlockTrigger()
Unlock scan trigger key

```csharp
bool unlocked = scannerManager.UnlockTrigger();
```

**Returns:** `true`=success, `false`=failure

##### GetTriggerLockState()
Get trigger key lock state

```csharp
bool isLocked = scannerManager.GetTriggerLockState();
```

**Returns:** `true`=locked, `false`=unlocked

#### Symbol Management

##### EnableAllSymbologies(bool enable)
Enable/disable all barcode types

```csharp
scannerManager.EnableAllSymbologies(true);   // Enable all
scannerManager.EnableAllSymbologies(false);  // Disable all
```

**Parameters:**
- `enable`: `true`=enable, `false`=disable

##### EnableSymbology(Symbology barcodeType, bool enable)
Enable/disable specific barcode type

```csharp
using Android.Device.Scanner.Configuration;

scannerManager.EnableSymbology(Symbology.Code128, true);
scannerManager.EnableSymbology(Symbology.QrCode, true);
```

**Parameters:**
- `barcodeType`: Barcode type (`Symbology` enum)
- `enable`: `true`=enable, `false`=disable

**Common Barcode Types:**
- `Symbology.Code128`
- `Symbology.Code39`
- `Symbology.QrCode`
- `Symbology.Ean13`
- `Symbology.Upca`
- See `Symbology` enum for more

##### IsSymbologyEnabled(Symbology barcodeType)
Check if specific barcode type is enabled

```csharp
bool isEnabled = scannerManager.IsSymbologyEnabled(Symbology.Code128);
```

**Returns:** `true`=enabled, `false`=disabled

##### IsSymbologySupported(Symbology barcodeType)
Check if device supports specific barcode type

```csharp
bool isSupported = scannerManager.IsSymbologySupported(Symbology.DataMatrix);
```

**Returns:** `true`=supported, `false`=not supported

#### Parameter Configuration

##### SetParameterInts(int[] idBuffer, int[] valueBuffer)
Set integer type parameters

```csharp
int[] ids = { paramId1, paramId2 };
int[] values = { value1, value2 };
int result = scannerManager.SetParameterInts(ids, values);
```

**Returns:** `0`=success, other values=failure

##### GetParameterInts(int[] idBuffer)
Get integer type parameters

```csharp
int[] ids = { paramId1, paramId2 };
int[] values = scannerManager.GetParameterInts(ids);
```

**Returns:** Parameter value array

##### SetParameterString(int[] idBuffer, string[] valueBuffer)
Set string type parameters

```csharp
int[] ids = { paramId1 };
string[] values = { "value1" };
bool result = scannerManager.SetParameterString(ids, values);
```

**Returns:** `true`=success, `false`=failure

##### GetParameterString(int[] idBuffer)
Get string type parameters

```csharp
int[] ids = { paramId1 };
string[] values = scannerManager.GetParameterString(ids);
```

**Returns:** Parameter value array

##### ResetScannerParameters()
Reset all parameters to factory defaults

```csharp
bool reset = scannerManager.ResetScannerParameters();
```

**Returns:** `true`=success, `false`=failure

## Complete Example

```csharp
using YourApp.Services;
using Android.Device.Scanner.Configuration;

namespace YourApp
{
    public partial class MainPage : ContentPage
    {
        IMAUIScannerManager scannerManager;
        bool isScanning = false;

        public MainPage()
        {
            InitializeComponent();
            
            // Get service
            scannerManager = DependencyService.Get<IMAUIScannerManager>();
            
            // Register event
            scannerManager.BroadcastReceived += OnBroadcastReceived;
            
            // Initialize scanner
            InitScanner();
        }

        private void InitScanner()
        {
            // Set broadcast output mode
            scannerManager.SwitchOutputMode(0);
            
            // Open scanner
            bool isOpen = scannerManager.OpenScanner();
            if (!isOpen)
            {
                InfoLabel.Text = "Failed to open scanner";
                return;
            }
            
            // Configure barcode types
            scannerManager.EnableSymbology(Symbology.Code128, true);
            scannerManager.EnableSymbology(Symbology.QrCode, true);
            scannerManager.EnableSymbology(Symbology.Ean13, true);
            
            InfoLabel.Text = "Scanner ready";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            // Register broadcast
            scannerManager?.RegisterBroadcastReceiver("android.intent.ACTION_DECODE_DATA");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            
            // Stop scanning
            if (isScanning)
            {
                scannerManager?.StopDecode();
            }
            
            // Unregister broadcast
            scannerManager?.UnregisterBroadcastReceiver();
        }

        // Scan result callback
        private void OnBroadcastReceived(object sender, string data)
        {
            isScanning = false;
            ScanButton.Text = "Start Scan";
            
            // Display result
            string time = DateTime.Now.ToString("HH:mm:ss");
            InfoLabel.Text = $"[{time}] Scan result:\n{data}";
        }

        // Start/stop scanning
        private void OnScanClicked(object sender, EventArgs e)
        {
            if (isScanning)
            {
                bool stopped = scannerManager.StopDecode();
                if (stopped)
                {
                    isScanning = false;
                    ScanButton.Text = "Start Scan";
                    InfoLabel.Text = "Scanning stopped";
                }
            }
            else
            {
                bool started = scannerManager.StartDecode();
                if (started)
                {
                    isScanning = true;
                    ScanButton.Text = "Stop Scan";
                    InfoLabel.Text = "Scanning...";
                }
            }
        }

        // Lock/unlock trigger key
        private void OnLockTriggerClicked(object sender, EventArgs e)
        {
            bool isLocked = scannerManager.GetTriggerLockState();
            
            if (isLocked)
            {
                bool unlocked = scannerManager.UnlockTrigger();
                if (unlocked)
                {
                    LockButton.Text = "Lock Key";
                    InfoLabel.Text = "Scan key unlocked";
                }
            }
            else
            {
                bool locked = scannerManager.LockTrigger();
                if (locked)
                {
                    LockButton.Text = "Unlock Key";
                    InfoLabel.Text = "Scan key locked";
                }
            }
        }

        // Enable/disable all barcode types
        private void OnToggleSymbologiesClicked(object sender, EventArgs e)
        {
            // Check current state
            bool code128Enabled = scannerManager.IsSymbologyEnabled(Symbology.Code128);
            
            if (code128Enabled)
            {
                scannerManager.EnableAllSymbologies(false);
                InfoLabel.Text = "All barcode types disabled";
            }
            else
            {
                scannerManager.EnableAllSymbologies(true);
                InfoLabel.Text = "All barcode types enabled";
            }
        }

        // Get scanner status
        private void OnGetStatusClicked(object sender, EventArgs e)
        {
            bool scannerState = scannerManager.GetScannerState();
            int outputMode = scannerManager.GetOutputMode();
            Triggering triggerMode = scannerManager.GetTriggerMode();
            bool triggerLocked = scannerManager.GetTriggerLockState();
            
            string status = $"Scanner Status:\n" +
                          $"- Power: {(scannerState ? "On" : "Off")}\n" +
                          $"- Output Mode: {(outputMode == 0 ? "Broadcast" : "Text Box")}\n" +
                          $"- Trigger Mode: {triggerMode}\n" +
                          $"- Key Locked: {(triggerLocked ? "Yes" : "No")}";
            
            InfoLabel.Text = status;
        }
    }
}
```

## Working Principle

### Broadcast Receiving Mechanism

```
1. Register broadcast receiver
   RegisterBroadcastReceiver("android.intent.ACTION_DECODE_DATA")
           ↓
2. Scanner scans barcode
   StartDecode()
           ↓
3. System sends broadcast
   Intent: android.intent.ACTION_DECODE_DATA
   Extra: "barcode" (byte[])
           ↓
4. OnReceive() receives broadcast
   byte[] → UTF8 String
           ↓
5. Trigger event
   BroadcastReceived?.Invoke(this, str)
           ↓
6. Application processes result
   OnBroadcastReceived(sender, data)
```

### BroadcastReceiver Inheritance

`MAUIScannerManager` inherits from `BroadcastReceiver` and implements `OnReceive` method:

```csharp
public override void OnReceive(Context context, Intent intent)
{
    if (intent.Action == _action)
    {
        byte[] bytes = intent.GetByteArrayExtra("barcode");
        string str = System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        BroadcastReceived?.Invoke(this, str);
    }
}
```

## Important Notes

1. **Broadcast Registration Lifecycle**:
   - Must register broadcast in `OnAppearing()`
   - Must unregister broadcast in `OnDisappearing()`
   - Avoid memory leaks

2. **Output Mode**:
   - Mode 0 (broadcast mode) is the recommended method
   - Call `SwitchOutputMode(0)` in constructor

3. **DependencyService Registration**:
   - Use `[assembly: Dependency]` to register in Android project
   - Use `DependencyService.Get<IMAUIScannerManager>()` to get instance

4. **Broadcast Action**:
   - Fixed as `"android.intent.ACTION_DECODE_DATA"`
   - Corresponds to `ScanManager.ActionDecode` constant

5. **Broadcast Data Format**:
   - Extra key name: `"barcode"`
   - Data type: `byte[]`
   - Encoding: UTF-8

6. **Hardware Key**:
   - Physical scan button automatically triggers scanning
   - Use `LockTrigger()` to disable hardware key

7. **Scanner State**:
   - Check `GetScannerState()` before use
   - Ensure scanner is powered on

8. **Property Wrapping**:
   - `GetOutputMode()` → `scanManager.OutputMode`
   - `GetScannerState()` → `scanManager.ScannerState`
   - `GetTriggerMode()` → `scanManager.TriggerMode`
   - `GetTriggerLockState()` → `scanManager.TriggerLockState`

## Troubleshooting

### Not Receiving Scan Results
- Check if broadcast receiver is registered
- Confirm output mode is 0 (broadcast mode)
- Verify broadcast Action is correct
- Check if `BroadcastReceived` event is registered

### Scanner Not Responding
- Confirm `OpenScanner()` was called
- Check `GetScannerState()` return value
- Verify `StartDecode()` was called

### Hardware Key Not Working
- Check if trigger key is locked (`GetTriggerLockState()`)
- Use `UnlockTrigger()` to unlock

### Certain Barcode Types Cannot Be Scanned
- Use `IsSymbologyEnabled()` to check if enabled
- Use `EnableSymbology()` to enable corresponding type
- Use `IsSymbologySupported()` to confirm device support

## Version Requirements

- **.NET**: 8.0+
- **.NET MAUI**: 8.0+
- **Android SDK**: API 19+
- **Dependencies**: android.device.ScanManager

## Technical Support

For technical support, please contact UROVO technical support team.

## License

Copyright © UROVO Technology Co., Ltd.
