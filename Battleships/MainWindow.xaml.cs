using Battleships.Interfaces.UI;
using Common;
using Common.Structures.Common;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Battleships
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SynchronizationContext SyncContext { get; private set; }

        UserControl currentControl;

        public MainWindow()
        {
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            ProtoClient.OnConnectionLost += ProtoClient_OnConnectionLost;
            ProtoClient.Bind();
            this.SyncContext = SynchronizationContext.Current;
            this.Loaded += MainWindow_Loaded;
            InitializeComponent();
        }

        private void ProtoClient_OnConnectionLost(Connection data)
        {
            this.Dispatcher.Invoke((Action)delegate 
            {
                MessageBox.Show(this, "Connection to server lost. Please restart the app.");
                Environment.Exit(0);
            });
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            using (StreamWriter sw = new StreamWriter("test.txt", true))
            {
                sw.WriteLine(e.Exception.StackTrace + e.Exception + e.Exception.Message + e.Exception.Source);
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.TransitionInto(currentControl = new LoginView());
        }

        private double CalculateWindowWidth(double width)
        {
            return width + 25;
        }

        private double CalculateWindowHeight(double height)
        {
            var titleHeight = SystemParameters.WindowCaptionHeight + SystemParameters.ResizeFrameHorizontalBorderHeight;
            return (height + titleHeight + 25);
        }

        public void TransitionIn(ContentControl content)
        {
            content.Opacity = 0;
            bool enableContent = content.IsEnabled;
            content.IsEnabled = false;
            this.Content = content;
            var startHandler = content as IEmbeddedViewShowAnimationStart;
            if(startHandler != null)
            {
                startHandler.OnShowAnimationStart();
            }
            var sizeAnimation = new DoubleAnimation(CalculateWindowWidth(content.Width), TimeSpan.FromSeconds(0.25));
            /* Soo animating the Window's height and width simultaneously is not possible,
             * since it's not a WPF component. It uses P/invoke behind the scenes.
             */
            sizeAnimation.Completed += (s, e) =>
            {
               
                this.MinWidth = this.Width;
                var heightAnimation = new DoubleAnimation(CalculateWindowHeight((this.Content as ContentControl).Height), TimeSpan.FromSeconds(0.25));
                heightAnimation.Completed += (s2, e2) =>
                {
                    this.MinHeight = this.Height;
                };
                this.BeginAnimation(Window.HeightProperty, heightAnimation);
            };
            this.BeginAnimation(Window.WidthProperty, sizeAnimation);

            var opacityAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.5));
            opacityAnimation.Completed += (s, e) =>
            {
                (this.Content as ContentControl).IsEnabled = enableContent;
                var startHandlerEnd = this.Content as IEmbeddedViewShowAnimationEnd;
                if(startHandlerEnd != null)
                {
                    startHandlerEnd.OnShowAnimationEnd();
                }
            };
            content.BeginAnimation(ContentControl.OpacityProperty, opacityAnimation);
        }

        public void TransitionInto(ContentControl content)
        {
            if (!this.HasContent)
            {
                TransitionIn(content);
                return;
            }
            var startHandler = this.Content as IEmbeddedViewHideAnimationStart;
            if(startHandler != null)
            {
                startHandler.OnHideAnimationStart();
            }
            (this.Content as ContentControl).IsEnabled = false;
            this.MinWidth = Math.Min(CalculateWindowWidth(content.Width), this.Width);
            this.MinHeight = Math.Min(CalculateWindowHeight(content.Height), this.Height);
            DoubleAnimation da = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.5)));
            da.Completed += (s, e) =>
            { 
                TransitionIn(content);
            };
            (this.Content as ContentControl).BeginAnimation(ContentControl.OpacityProperty, da);
        }
    }
}
