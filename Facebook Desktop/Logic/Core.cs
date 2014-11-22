using agsXMPP;
using Facebook_Desktop.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Facebook_Desktop.Logic
{
    /// <summary>
    /// Most of the logic behind Facebook Desktop
    /// </summary>
    public class Core
    {
        //This region only relates to this program only
        #region Facebook Desktop logic
        /// <summary>
        /// The main window that most items reside in
        /// </summary>
        internal static MainWindow mainwin;
        
        /// <summary>
        /// Used to send notifications to the users desktop
        /// </summary>
        internal static NotifyWindow notifywin;

        /// <summary>
        /// The container that holds everything
        /// </summary>
        internal static ContentControl MainContainer;

        /// <summary>
        /// The page currently being used
        /// </summary>
        internal static Type CurrentPage = typeof(LoginPage);

        /// <summary>
        /// Used to hide windows
        /// </summary>
        /// <param name="type">The type of window to hide</param>
        internal static void Hide(Type type)
        {
            if (type == typeof(MainWindow))
                mainwin.Visibility = System.Windows.Visibility.Hidden;
            else
                notifywin.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Used to show windows
        /// </summary>
        /// <param name="type">The type of window to show</param>
        internal static void Show(Type type)
        {
            if (type == typeof(MainWindow))
                mainwin.Visibility = System.Windows.Visibility.Visible;
            else
                notifywin.Visibility = System.Windows.Visibility.Visible;
        }

        internal static void SwitchPage<T>(bool Fade = false, params object[] Arguments)
        {
            if (CurrentPage == typeof(T))
                return;

            Page instance = (Page)Activator.CreateInstance(typeof(T), Arguments);

            if (Fade)
            {
                var fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.25));
                fadeOutAnimation.Completed += (x, y) =>
                {
                    MainContainer.Content = instance.Content;
                    var fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.25));
                    MainContainer.BeginAnimation(ContentControl.OpacityProperty, fadeInAnimation);
                };
                MainContainer.BeginAnimation(ContentControl.OpacityProperty, fadeOutAnimation);
            }
            else
            {
                MainContainer.Content = instance.Content;
            }
        }
        #endregion Facebook Desktop logic

        
        #region Chat
        /// <summary>
        /// The main xmpp Connection to facebook
        /// </summary>
        internal static XmppClientConnection xmppConnect;

        internal static string Username = "";

        internal static string Password = "";

        internal static void xmppConnect_OnMessage(object sender, agsXMPP.protocol.client.Message msg)
        {

        }
        #endregion Chat
    }
}
