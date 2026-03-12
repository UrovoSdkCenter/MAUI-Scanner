using MauiScanner_net8.Services;

namespace MauiScanner_net8
{
    public partial class MainPage : ContentPage
    {

        IMAUIScannerManager scannerManager;
        bool isScanning = false;
        public MainPage()
        {
            InitializeComponent();
            scannerManager = DependencyService.Get<IMAUIScannerManager>();
            scannerManager.BroadcastReceived += OnBroadcastReceived;
            //
            scannerManager.SwitchOutputMode(0);//intent output
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            string model = DeviceInfo.Model;
            string manufacturer = DeviceInfo.Manufacturer;

            tv_info.Text = "Model:" + model + "\nManufacturer:" + manufacturer;

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
            CounterBtn.Text = isScanning ? "stop scan" : "start scan";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            scannerManager?.RegisterBroadcastReceiver("android.intent.ACTION_DECODE_DATA");

        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            scannerManager?.UnregisterBroadcastReceiver();
        }
        private void OnBroadcastReceived(object sender, string data)
        {
            // handle scan result
            isScanning = false;
            CounterBtn.Text = isScanning ? "stop scan" : "start scan";

            tv_info.Text = "scan result：" + data;
        }
    }

}
