using Facebook_Desktop.Logic;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Facebook_Desktop.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password.Length > 0)
            {
                var fadeLabelOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.1));
                PasswordLabel.BeginAnimation(Label.OpacityProperty, fadeLabelOutAnimation);
            }
            else
            {
                var fadeLabelInAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.1));
                PasswordLabel.BeginAnimation(Label.OpacityProperty, fadeLabelInAnimation);
            }

            var fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.5));
            HintLabel.BeginAnimation(Label.OpacityProperty, fadeInAnimation);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Core.Username = UsernameTextBox.WaterTextbox.Text;
            Core.Password = PasswordBox.Password;
            Core.xmppConnect = new agsXMPP.XmppClientConnection("chat.facebook.com");
            Core.xmppConnect.Port = 5222;
            Core.xmppConnect.OnLogin += xmppConnect_OnLogin;
            Core.xmppConnect.OnMessage += Core.xmppConnect_OnMessage;
            Core.xmppConnect.Open(Core.Username, Core.Password);
        }

        

        void xmppConnect_OnLogin(object sender)
        {
            Core.SwitchPage<MainPage>();
        }
    }
}
