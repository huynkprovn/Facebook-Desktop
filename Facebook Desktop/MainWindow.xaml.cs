using System;
using System.Windows.Media.Animation;
using Facebook_Desktop.Logic;
using Facebook_Desktop.Pages;
using System.Windows;

namespace Facebook_Desktop
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Application.Current.Resources.Source = new Uri("/Facebook Desktop;component/Styles.xaml", UriKind.RelativeOrAbsolute);
            Core.Mainwin = this;
            Core.MainContainer = _container;
            var waitAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.5));
            waitAnimation.Completed += (o, e) => { _container.Content = new LoginPage().Content; };
            _container.BeginAnimation(OpacityProperty, waitAnimation);
        }
    }
}