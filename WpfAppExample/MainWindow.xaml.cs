﻿using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.Discovery;
using RWS;
using RWS.IRC5.SubscriptionServices;
using RWS.OmniCore;
using System.Linq;
using System.Threading;
using System.Windows;


namespace WpfAppExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            Main();


        }

        private async void Main()
        {

            var scanner = new NetworkScanner();
            scanner.Scan();
            ControllerInfoCollection controllers = scanner.Controllers;
            var vc7 = controllers.FirstOrDefault(c => c.IsVirtual && c.VersionName.Contains("7."));
            var vc6 = controllers.FirstOrDefault(c => c.IsVirtual && c.VersionName.Contains("6."));


            //Testing RWS2.0 

            var rwsCs7 = new OmniCoreSession(new Address($"{vc7.IPAddress}:{vc7.WebServicesPort}"));
            //var info7 = await rwsCs7.RobotWareService.GetSystemInformationAsync();
            var ios7 = await rwsCs7.RobotWareService.GetIOSignalsAsync();
            var io7 = ios7.Embedded.Resources.FirstOrDefault(io => io.Name.Contains("doSigTest"));

            var rwsCs6 = new IRC5Session(new Address($"{vc6.IPAddress}:{vc6.WebServicesPort}"));
            //var info6 = await rwsCs6.RobotWareService.GetSystemInformationAsync();
            var ios6 = await rwsCs6.RobotWareService.GetIOSignalsAsync();
            var io6 = ios6.Embedded.State.FirstOrDefault(io => io.Name.Contains("doSigTest"));

            io6.OnValueChanged += IOSignal_ValueChanged;
            io7.OnValueChanged += IOSignal_ValueChanged;

            //var ios = await rwsCs1.RobotWareService.GetSystemInformationAsync();


            // var ios = await rwsCs1.RobotWareService.GetIOSignalsAsync();



            // var io = ios.Embedded.Resources.FirstOrDefault(io => io.Name.Contains("doSigTest"));

            //io.OnValueChanged += IOSignal_ValueChanged;




            //var dev = await rwsCs1.RobotWareService.GetIODevicesAsync();
            //var dev2 = await rwsCs1.RobotWareService.GetIODevicesAsync();


            //  rwsCs1.UserService.RequestRmmpAsync(Enums.Privilege.MODIFY);
            //var rmmpState = await rwsCs1.UserService.GetRmmpStateAsync().ConfigureAwait(false);
            //await rwsCs1.UserService.RegisterUserAsync("SEPARIA", "RobotStudio", "SWE", Enums.LoginType.LOCAL).ConfigureAwait(false);
            //await rwsCs1.UserService.GrantOrDenyRmmpAsync(rmmpState.Embedded.State.First().UserID, Enums.Privilege.MODIFY).ConfigureAwait(false);
            //rwsCs1.RobotWareService.MastershipRequest();
            //rwsCs1.UserService.CancelHeldOrRequestedRmmp();
            //rwsCs1.ControllerService.Restart(Enums.RestartMode.RESTART);





        }

        private void IOSignal_ValueChanged(object source, IOEventArgs args)
        {
            CheckBox_sig.IsChecked = args.ValueChanged == 1 ? true : false;

        }


    }
}
