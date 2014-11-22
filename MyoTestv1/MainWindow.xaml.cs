using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MyoSharp.Device;
using MyoSharp.ConsoleSample.Internal;
using Microsoft.WindowsAzure.MobileServices;

namespace MyoTestv2
{
    using MyoTestv2.Internal;
    using MyoSharp.Poses;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window 
    {
        public MainWindow()
        {
            InitializeComponent();

            // create a hub that will manage Myo devices for us
            using (var hub = Hub.Create())
            {
                // listen for when the Myo connects
                hub.MyoConnected += (sender, e) =>
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        statusTbx.Text = "Myo has connected!";
                        e.Myo.Vibrate(VibrationType.Short);
                        // setup for the pose we want to watch for
                        var pose = HeldPose.Create(e.Myo, Pose.Fist, Pose.FingersSpread);
                        // set the interval for the event to be fired as long as 
                        // the pose is held by the user
                        pose.Interval = TimeSpan.FromSeconds(4);
                        pose.Start();
                        pose.Triggered += Pose_Triggered;
                        
                    }));                      

                };


                // listen for when the Myo disconnects
                hub.MyoDisconnected += (sender, e) =>
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        statusTbx.Text = "Myo has disconnected!";
                        e.Myo.PoseChanged -= Pose_Triggered;

                    }));   
                };


                // wait on user input
                //ConsoleHelper.UserInputLoop(hub);
            }

            
        }

        

        #region Event Handlers

        int fingersSpreadScoreCntr = 0;
        int fistSpreadScoreCntr = 0;

        private void Pose_Triggered(object sender, PoseEventArgs e)
        {

            App.Current.Dispatcher.Invoke((Action)(() =>
            {  
                eventTbx.Text = "{0} arm Myo holding pose {1}" + e.Myo.Arm + e.Myo.Pose;
                

                if(e.Myo.Pose == Pose.Fist )
                {

                    e.Myo.Vibrate(VibrationType.Short);
                    fistSpreadScoreCntr++;
                    lblFistScoreCntr.Content = fistSpreadScoreCntr;
                    gestureImg.Source = new BitmapImage(new Uri
                   ("/MyoTestv1;component/Images/gesture_icons/blue_outline_LH_fist.png", UriKind.Relative));

                }
               
                else if (e.Myo.Pose == Pose.FingersSpread)
                {

                    e.Myo.Vibrate(VibrationType.Short);
                    fingersSpreadScoreCntr++;
                    lblFingersSpreadScoreCntr.Content = fingersSpreadScoreCntr;
                    gestureImg.Source = new BitmapImage(new Uri
                   ("/MyoTestv1;component/Images/gesture_icons/blue_outline_LH_spread_fingers.png", UriKind.Relative));


                }
                
            }));          
        }


       
        #endregion

       
        private async void submitProgressBtn_Click(object sender, RoutedEventArgs e)
        {
            Item item = new Item { Repititions = " " + fingersSpreadScoreCntr.ToString(), Date = " " + DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt"), User = "Brian Varley" , Exercise = "Fingers Spread"};
            await App.MobileService.GetTable<Item>().InsertAsync(item);

        }


    }
}
