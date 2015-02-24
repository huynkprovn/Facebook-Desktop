using System;
using System.Windows;
using Facebook_Desktop.Logic;
using Facebook_Desktop.Pages;

namespace Facebook_Desktop
{
    /// <summary>
    ///     Interaction logic for NotifyWindow.xaml
    /// </summary>
    public partial class NotifyWindow
    {
        public NotifyWindow(NotifyWindowPage contentPage)
        {
            InitializeComponent();
            _mainContent.Content = contentPage.Content;
            Resources.Source = new Uri("/Facebook Desktop;component/Styles.xaml", UriKind.RelativeOrAbsolute);  
            Core.Notifywin = this;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width;
            Top = desktopWorkingArea.Bottom - Height;
        }
    }
}