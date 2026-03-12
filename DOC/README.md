# UROVO Scanner SDK for .NET MAUI

UROVO条码扫描器SDK的.NET MAUI封装，提供条码扫描功能支持。

## 平台支持

- Android

## 功能特性

- **扫描控制**: 启动/停止扫描、打开/关闭扫描器
- **广播接收**: 通过Android广播接收扫描结果
- **输出模式**: 广播模式、文本框输出模式
- **触发控制**: 触发模式设置、触发键锁定
- **符号管理**: 启用/禁用特定条码类型
- **参数配置**: 整数和字符串参数配置
- **事件驱动**: 通过EventHandler接收扫描结果

## 环境要求

- **.NET**: 8.0+
- **MAUI**: .NET MAUI workload
- **Android SDK**: API 19+
- **依赖**: android.device.ScanManager

## 项目结构

```
Services/
  └── IMAUIScannerManager.cs    # 扫描器服务接口定义

Platforms/
  └── Android/
      └── MAUIScannerManager.cs  # Android平台扫描器实现
```

## 快速开始

### 1. 定义服务接口

```csharp
using Android.Device.Scanner.Configuration;
using System;

namespace YourApp.Services
{
    public interface IMAUIScannerManager
    {
        // 广播管理
        void RegisterBroadcastReceiver(string action);
        void UnregisterBroadcastReceiver();
        event EventHandler<string> BroadcastReceived;

        // 扫描器控制
        bool OpenScanner();
        bool CloseScanner();
        bool GetScannerState();
        bool StartDecode();
        bool StopDecode();

        // 输出模式
        bool SwitchOutputMode(int mode);
        int GetOutputMode();

        // 触发控制
        void SetTriggerMode(Triggering mode);
        Triggering GetTriggerMode();
        bool LockTrigger();
        bool UnlockTrigger();
        bool GetTriggerLockState();

        // 符号管理
        void EnableAllSymbologies(bool enable);
        void EnableSymbology(Symbology barcodeType, bool enable);
        bool IsSymbologyEnabled(Symbology barcodeType);
        bool IsSymbologySupported(Symbology barcodeType);

        // 参数配置
        int SetParameterInts(int[] idBuffer, int[] valueBuffer);
        int[] GetParameterInts(int[] idBuffer);
        bool SetParameterString(int[] idBuffer, string[] valueBuffer);
        string[] GetParameterString(int[] idBuffer);
        bool ResetScannerParameters();
    }
}
```

### 2. 实现Android平台服务

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

        // 注册广播接收器
        public void RegisterBroadcastReceiver(string action)
        {
            _action = action;
            IntentFilter filter = new(action);
            Application.Context.RegisterReceiver(this, filter);
            Log.Debug("Scanner", "RegisterBroadcastReceiver");
        }

        // 注销广播接收器
        public void UnregisterBroadcastReceiver()
        {
            Log.Debug("Scanner", "UnregisterBroadcastReceiver");
            Application.Context.UnregisterReceiver(this);
        }

        // 接收广播
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

        // 打开扫描器
        public bool OpenScanner()
        {
            return scanManager.OpenScanner();
        }

        // 关闭扫描器
        public bool CloseScanner()
        {
            return scanManager.CloseScanner();
        }

        // 获取扫描器状态
        public bool GetScannerState()
        {
            return scanManager.ScannerState;
        }

        // 启动扫描
        public bool StartDecode()
        {
            return scanManager.StartDecode();
        }

        // 停止扫描
        public bool StopDecode()
        {
            return scanManager.StopDecode();
        }

        // 切换输出模式
        public bool SwitchOutputMode(int mode)
        {
            return scanManager.SwitchOutputMode(mode);
        }

        // 获取输出模式
        public int GetOutputMode()
        {
            return scanManager.OutputMode;
        }

        // 设置触发模式
        public void SetTriggerMode(Triggering mode)
        {
            scanManager.TriggerMode = mode;
        }

        // 获取触发模式
        public Triggering GetTriggerMode()
        {
            return scanManager.TriggerMode;
        }

        // 锁定触发键
        public bool LockTrigger()
        {
            return scanManager.LockTrigger();
        }

        // 解锁触发键
        public bool UnlockTrigger()
        {
            return scanManager.UnlockTrigger();
        }

        // 获取触发键锁定状态
        public bool GetTriggerLockState()
        {
            return scanManager.TriggerLockState;
        }

        // 启用/禁用所有符号
        public void EnableAllSymbologies(bool enable)
        {
            scanManager.EnableAllSymbologies(enable);
        }

        // 启用/禁用指定符号
        public void EnableSymbology(Symbology barcodeType, bool enable)
        {
            scanManager.EnableSymbology(barcodeType, enable);
        }

        // 检查符号是否已启用
        public bool IsSymbologyEnabled(Symbology barcodeType)
        {
            return scanManager.IsSymbologyEnabled(barcodeType);
        }

        // 检查是否支持指定符号
        public bool IsSymbologySupported(Symbology barcodeType)
        {
            return scanManager.IsSymbologySupported(barcodeType);
        }

        // 设置整数参数
        public int SetParameterInts(int[] idBuffer, int[] valueBuffer)
        {
            return scanManager.SetParameterInts(idBuffer, valueBuffer);
        }

        // 获取整数参数
        public int[] GetParameterInts(int[] idBuffer)
        {
            return scanManager.GetParameterInts(idBuffer);
        }

        // 设置字符串参数
        public bool SetParameterString(int[] idBuffer, string[] valueBuffer)
        {
            return scanManager.SetParameterString(idBuffer, valueBuffer);
        }

        // 获取字符串参数
        public string[] GetParameterString(int[] idBuffer)
        {
            return scanManager.GetParameterString(idBuffer);
        }

        // 重置扫描器参数
        public bool ResetScannerParameters()
        {
            return scanManager.ResetScannerParameters();
        }
    }
}
```

### 3. 在页面中使用

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
            
            // 获取扫描器服务
            scannerManager = DependencyService.Get<IMAUIScannerManager>();
            
            // 注册事件
            scannerManager.BroadcastReceived += OnBroadcastReceived;
            
            // 设置输出模式为广播模式
            scannerManager.SwitchOutputMode(0);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            // 注册广播接收器
            scannerManager?.RegisterBroadcastReceiver("android.intent.ACTION_DECODE_DATA");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            
            // 注销广播接收器
            scannerManager?.UnregisterBroadcastReceiver();
        }

        // 扫描结果回调
        private void OnBroadcastReceived(object sender, string data)
        {
            isScanning = false;
            InfoLabel.Text = $"扫描结果: {data}";
        }

        // 启动/停止扫描按钮
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
            ScanButton.Text = isScanning ? "停止扫描" : "开始扫描";
        }
    }
}
```

## API文档

### IMAUIScannerManager接口

#### 事件

##### BroadcastReceived
扫描结果事件

```csharp
event EventHandler<string> BroadcastReceived;
```

**事件参数:**
- `string`: 扫描到的条码内容

#### 广播管理

##### RegisterBroadcastReceiver(string action)
注册广播接收器

```csharp
scannerManager.RegisterBroadcastReceiver("android.intent.ACTION_DECODE_DATA");
```

**参数:**
- `action`: 广播Action，通常为`"android.intent.ACTION_DECODE_DATA"`

**说明:** 必须在页面显示时调用（`OnAppearing`）

##### UnregisterBroadcastReceiver()
注销广播接收器

```csharp
scannerManager.UnregisterBroadcastReceiver();
```

**说明:** 必须在页面隐藏时调用（`OnDisappearing`），避免内存泄漏

#### 扫描器控制

##### OpenScanner()
打开扫描器电源

```csharp
bool isOpen = scannerManager.OpenScanner();
```

**返回值:** `true`=成功, `false`=失败

##### CloseScanner()
关闭扫描器电源

```csharp
bool isClosed = scannerManager.CloseScanner();
```

**返回值:** `true`=成功, `false`=失败

##### GetScannerState()
获取扫描器电源状态

```csharp
bool isOn = scannerManager.GetScannerState();
```

**返回值:** `true`=已开启, `false`=已关闭

##### StartDecode()
启动扫描

```csharp
bool started = scannerManager.StartDecode();
```

**返回值:** `true`=成功, `false`=失败

**说明:** 扫描结果通过`BroadcastReceived`事件返回

##### StopDecode()
停止扫描

```csharp
bool stopped = scannerManager.StopDecode();
```

**返回值:** `true`=成功, `false`=失败

#### 输出模式

##### SwitchOutputMode(int mode)
切换输出模式

```csharp
bool switched = scannerManager.SwitchOutputMode(0);
```

**参数:**
- `mode`: 输出模式
  - `0`: 广播模式（推荐）
  - `1`: 文本框输出模式

**返回值:** `true`=成功, `false`=失败

##### GetOutputMode()
获取当前输出模式

```csharp
int mode = scannerManager.GetOutputMode();
```

**返回值:** `0`=广播模式, `1`=文本框模式

#### 触发控制

##### SetTriggerMode(Triggering mode)
设置触发模式

```csharp
using Android.Device.Scanner.Configuration;

scannerManager.SetTriggerMode(Triggering.Host);
```

**参数:**
- `Triggering.Host`: 主机触发（通过代码）
- `Triggering.Level`: 电平触发
- `Triggering.Pulse`: 脉冲触发

##### GetTriggerMode()
获取当前触发模式

```csharp
Triggering mode = scannerManager.GetTriggerMode();
```

**返回值:** 当前触发模式

##### LockTrigger()
锁定扫描触发键

```csharp
bool locked = scannerManager.LockTrigger();
```

**返回值:** `true`=成功, `false`=失败

**说明:** 锁定后物理扫描按键将被禁用

##### UnlockTrigger()
解锁扫描触发键

```csharp
bool unlocked = scannerManager.UnlockTrigger();
```

**返回值:** `true`=成功, `false`=失败

##### GetTriggerLockState()
获取触发键锁定状态

```csharp
bool isLocked = scannerManager.GetTriggerLockState();
```

**返回值:** `true`=已锁定, `false`=未锁定

#### 符号管理

##### EnableAllSymbologies(bool enable)
启用/禁用所有条码类型

```csharp
scannerManager.EnableAllSymbologies(true);   // 启用所有
scannerManager.EnableAllSymbologies(false);  // 禁用所有
```

**参数:**
- `enable`: `true`=启用, `false`=禁用

##### EnableSymbology(Symbology barcodeType, bool enable)
启用/禁用指定条码类型

```csharp
using Android.Device.Scanner.Configuration;

scannerManager.EnableSymbology(Symbology.Code128, true);
scannerManager.EnableSymbology(Symbology.QrCode, true);
```

**参数:**
- `barcodeType`: 条码类型（`Symbology`枚举）
- `enable`: `true`=启用, `false`=禁用

**常见条码类型:**
- `Symbology.Code128`
- `Symbology.Code39`
- `Symbology.QrCode`
- `Symbology.Ean13`
- `Symbology.Upca`
- 更多参考`Symbology`枚举

##### IsSymbologyEnabled(Symbology barcodeType)
检查指定条码类型是否已启用

```csharp
bool isEnabled = scannerManager.IsSymbologyEnabled(Symbology.Code128);
```

**返回值:** `true`=已启用, `false`=未启用

##### IsSymbologySupported(Symbology barcodeType)
检查设备是否支持指定条码类型

```csharp
bool isSupported = scannerManager.IsSymbologySupported(Symbology.DataMatrix);
```

**返回值:** `true`=支持, `false`=不支持

#### 参数配置

##### SetParameterInts(int[] idBuffer, int[] valueBuffer)
设置整数类型参数

```csharp
int[] ids = { paramId1, paramId2 };
int[] values = { value1, value2 };
int result = scannerManager.SetParameterInts(ids, values);
```

**返回值:** `0`=成功, 其他值=失败

##### GetParameterInts(int[] idBuffer)
获取整数类型参数

```csharp
int[] ids = { paramId1, paramId2 };
int[] values = scannerManager.GetParameterInts(ids);
```

**返回值:** 参数值数组

##### SetParameterString(int[] idBuffer, string[] valueBuffer)
设置字符串类型参数

```csharp
int[] ids = { paramId1 };
string[] values = { "value1" };
bool result = scannerManager.SetParameterString(ids, values);
```

**返回值:** `true`=成功, `false`=失败

##### GetParameterString(int[] idBuffer)
获取字符串类型参数

```csharp
int[] ids = { paramId1 };
string[] values = scannerManager.GetParameterString(ids);
```

**返回值:** 参数值数组

##### ResetScannerParameters()
重置所有参数为出厂默认值

```csharp
bool reset = scannerManager.ResetScannerParameters();
```

**返回值:** `true`=成功, `false`=失败

## 完整示例

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
            
            // 获取服务
            scannerManager = DependencyService.Get<IMAUIScannerManager>();
            
            // 注册事件
            scannerManager.BroadcastReceived += OnBroadcastReceived;
            
            // 初始化扫描器
            InitScanner();
        }

        private void InitScanner()
        {
            // 设置广播输出模式
            scannerManager.SwitchOutputMode(0);
            
            // 打开扫描器
            bool isOpen = scannerManager.OpenScanner();
            if (!isOpen)
            {
                InfoLabel.Text = "打开扫描器失败";
                return;
            }
            
            // 配置条码类型
            scannerManager.EnableSymbology(Symbology.Code128, true);
            scannerManager.EnableSymbology(Symbology.QrCode, true);
            scannerManager.EnableSymbology(Symbology.Ean13, true);
            
            InfoLabel.Text = "扫描器已就绪";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            // 注册广播
            scannerManager?.RegisterBroadcastReceiver("android.intent.ACTION_DECODE_DATA");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            
            // 停止扫描
            if (isScanning)
            {
                scannerManager?.StopDecode();
            }
            
            // 注销广播
            scannerManager?.UnregisterBroadcastReceiver();
        }

        // 扫描结果回调
        private void OnBroadcastReceived(object sender, string data)
        {
            isScanning = false;
            ScanButton.Text = "开始扫描";
            
            // 显示结果
            string time = DateTime.Now.ToString("HH:mm:ss");
            InfoLabel.Text = $"[{time}] 扫描结果:\n{data}";
        }

        // 启动/停止扫描
        private void OnScanClicked(object sender, EventArgs e)
        {
            if (isScanning)
            {
                bool stopped = scannerManager.StopDecode();
                if (stopped)
                {
                    isScanning = false;
                    ScanButton.Text = "开始扫描";
                    InfoLabel.Text = "扫描已停止";
                }
            }
            else
            {
                bool started = scannerManager.StartDecode();
                if (started)
                {
                    isScanning = true;
                    ScanButton.Text = "停止扫描";
                    InfoLabel.Text = "扫描中...";
                }
            }
        }

        // 锁定/解锁触发键
        private void OnLockTriggerClicked(object sender, EventArgs e)
        {
            bool isLocked = scannerManager.GetTriggerLockState();
            
            if (isLocked)
            {
                bool unlocked = scannerManager.UnlockTrigger();
                if (unlocked)
                {
                    LockButton.Text = "锁定按键";
                    InfoLabel.Text = "扫描按键已解锁";
                }
            }
            else
            {
                bool locked = scannerManager.LockTrigger();
                if (locked)
                {
                    LockButton.Text = "解锁按键";
                    InfoLabel.Text = "扫描按键已锁定";
                }
            }
        }

        // 启用/禁用所有条码类型
        private void OnToggleSymbologiesClicked(object sender, EventArgs e)
        {
            // 检查当前状态
            bool code128Enabled = scannerManager.IsSymbologyEnabled(Symbology.Code128);
            
            if (code128Enabled)
            {
                scannerManager.EnableAllSymbologies(false);
                InfoLabel.Text = "已禁用所有条码类型";
            }
            else
            {
                scannerManager.EnableAllSymbologies(true);
                InfoLabel.Text = "已启用所有条码类型";
            }
        }

        // 获取扫描器状态
        private void OnGetStatusClicked(object sender, EventArgs e)
        {
            bool scannerState = scannerManager.GetScannerState();
            int outputMode = scannerManager.GetOutputMode();
            Triggering triggerMode = scannerManager.GetTriggerMode();
            bool triggerLocked = scannerManager.GetTriggerLockState();
            
            string status = $"扫描器状态:\n" +
                          $"- 电源: {(scannerState ? "开启" : "关闭")}\n" +
                          $"- 输出模式: {(outputMode == 0 ? "广播" : "文本框")}\n" +
                          $"- 触发模式: {triggerMode}\n" +
                          $"- 按键锁定: {(triggerLocked ? "是" : "否")}";
            
            InfoLabel.Text = status;
        }
    }
}
```

## 工作原理

### 广播接收机制

```
1. 注册广播接收器
   RegisterBroadcastReceiver("android.intent.ACTION_DECODE_DATA")
           ↓
2. 扫描器扫描到条码
   StartDecode()
           ↓
3. 系统发送广播
   Intent: android.intent.ACTION_DECODE_DATA
   Extra: "barcode" (byte[])
           ↓
4. OnReceive()接收广播
   byte[] → UTF8 String
           ↓
5. 触发事件
   BroadcastReceived?.Invoke(this, str)
           ↓
6. 应用处理结果
   OnBroadcastReceived(sender, data)
```

### BroadcastReceiver继承

`MAUIScannerManager`继承自`BroadcastReceiver`，实现了`OnReceive`方法：

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

## 注意事项

1. **广播注册生命周期**:
   - 必须在`OnAppearing()`中注册广播
   - 必须在`OnDisappearing()`中注销广播
   - 避免内存泄漏

2. **输出模式**:
   - 模式0（广播模式）是推荐方式
   - 构造函数中调用`SwitchOutputMode(0)`

3. **DependencyService注册**:
   - 在Android项目中使用`[assembly: Dependency]`注册
   - 使用`DependencyService.Get<IMAUIScannerManager>()`获取实例

4. **广播Action**:
   - 固定为`"android.intent.ACTION_DECODE_DATA"`
   - 对应`ScanManager.ActionDecode`常量

5. **广播数据格式**:
   - Extra键名: `"barcode"`
   - 数据类型: `byte[]`
   - 编码: UTF-8

6. **硬件按键**:
   - 物理扫描按键会自动触发扫描
   - 使用`LockTrigger()`可禁用硬件按键

7. **扫描器状态**:
   - 使用前检查`GetScannerState()`
   - 确保扫描器已打开

8. **属性包装**:
   - `GetOutputMode()` → `scanManager.OutputMode`
   - `GetScannerState()` → `scanManager.ScannerState`
   - `GetTriggerMode()` → `scanManager.TriggerMode`
   - `GetTriggerLockState()` → `scanManager.TriggerLockState`

## 故障排查

### 收不到扫描结果
- 检查是否注册了广播接收器
- 确认输出模式为0（广播模式）
- 验证广播Action是否正确
- 检查`BroadcastReceived`事件是否已注册

### 扫描器无响应
- 确认调用了`OpenScanner()`
- 检查`GetScannerState()`返回值
- 验证是否调用了`StartDecode()`

### 硬件按键不工作
- 检查触发键是否被锁定（`GetTriggerLockState()`）
- 使用`UnlockTrigger()`解锁

### 某些条码类型无法扫描
- 使用`IsSymbologyEnabled()`检查是否启用
- 使用`EnableSymbology()`启用对应类型
- 使用`IsSymbologySupported()`确认设备支持

## 版本要求

- **.NET**: 8.0+
- **.NET MAUI**: 8.0+
- **Android SDK**: API 19+
- **依赖库**: android.device.ScanManager

## 技术支持

如需技术支持,请联系UROVO技术支持团队。

## License

Copyright © UROVO Technology Co., Ltd.
