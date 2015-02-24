using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using agsXMPP;
using agsXMPP.protocol.client;
using Facebook_Desktop.Pages;
using agsXMPP.protocol.iq.roster;
using Facebook_Desktop.Elements;

namespace Facebook_Desktop.Logic
{
    /// <summary>
    ///     Most of the logic behind Facebook Desktop
    /// </summary>
    public class Core
    {
        //This region only relates to this program only

        #region Facebook Desktop logic

        /// <summary>
        ///     The main window that most items reside in
        /// </summary>
        internal static MainWindow Mainwin;

        /// <summary>
        ///     The Main Page
        /// </summary>
        internal static MainPage MainPage;

        /// <summary>
        ///     Use this to run UI objects
        /// </summary>
        /// <param name="function"></param>
        internal static void RunOnUiThread(Action function)
        {
            Mainwin.Dispatcher.BeginInvoke(DispatcherPriority.Input, function);
        }
        /// <summary>
        ///     Used to send notifications to the users desktop
        /// </summary>
        internal static NotifyWindow Notifywin;

        /// <summary>
        ///     The container that holds everything
        /// </summary>
        internal static ContentControl MainContainer;

        /// <summary>
        ///     The page currently being used
        /// </summary>
        internal static Type CurrentPage = typeof(LoginPage);

        /// <summary>
        ///     Used to hide windows
        /// </summary>
        /// <param name="type">The type of window to hide</param>
        internal static void Hide(Type type)
        {
            if (type == typeof(MainWindow))
            {
                Mainwin.Visibility = Visibility.Hidden;
            }
            else
            {
                Notifywin.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        ///     Used to show windows
        /// </summary>
        /// <param name="type">The type of window to show</param>
        internal static void Show(Type type)
        {
            if (type == typeof(MainWindow))
            {
                Mainwin.Visibility = Visibility.Visible;
            }
            else
            {
                Notifywin.Visibility = Visibility.Visible;
            }
        }
        internal static void SwitchPage<T>(bool fade = false, params object[] arguments)
        {
            Application.Current.Dispatcher.Invoke(() =>
                {
                    if (CurrentPage == typeof(T))
                    {
                        return;
                    }

                    var instance = (Page)Activator.CreateInstance(typeof(T), arguments);

                    if (fade)
                    {
                        var fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.25));
                        fadeOutAnimation.Completed += (x, y) =>
                        {
                            MainContainer.Content = instance.Content;
                            var fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.25));
                            MainContainer.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
                        };
                        MainContainer.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
                    }
                    else
                    {
                        MainContainer.Content = instance.Content;
                    }
                });
        }

        #endregion Facebook Desktop logic

        #region Chat

        /// <summary>
        ///     All friends on facebook friends list
        /// </summary>
        internal static List<RosterItem> UserList = new List<RosterItem>();

        /// <summary>
        ///     All of the ChatPlayerItems
        /// </summary>
        internal static Dictionary<string, ChatPlayerItem> AllPlayers = new Dictionary<string, ChatPlayerItem>();

        /// <summary>
        ///     All online friends
        /// </summary>
        internal static List<Presence> Presences = new List<Presence>();

        /// <summary>
        ///     Used to create groups (like online/offline friends)
        /// </summary>
        internal static List<Group> Groups = new List<Group>(); 

        /// <summary>
        ///     ChatItem
        /// </summary>
        internal static ChatBoxControl ChatItem;

        /// <summary>
        ///     The main xmpp Connection to facebook
        /// </summary>
        internal static XmppClientConnection XmppConnect;

        /// <summary>
        ///     The user's username
        /// </summary>
        internal static string Username = "";

        /// <summary>
        ///     The user's password
        /// </summary>
        internal static string Password = "";

        /// <summary>
        ///     The PlayerChatControl (Saved for all users)
        /// </summary>
        internal static Dictionary<string, PlayerChatControl> PlayerChatControls = new Dictionary<string, PlayerChatControl>();

        internal static void xmppConnect_OnMessage(object sender, Message msg)
        {
            RunOnUiThread(() =>
                {
                    var player = false;
                    foreach (var controlList in MainPage._playerChatStackPanel.Children.Cast<PlayerChatControl>().Where(control => msg.GetAttribute("name") == ((ChatPlayerItem)control.Tag).Username))
                        player = true;

                    if (player)
                    {
                        return;
                    }
                    MainPage._playerChatStackPanel.Children.Add(PlayerChatControls[msg.GetAttribute("from")]);
                });
        }

        #endregion Chat
    }

    public class ChatPlayerItem
    {
        /// <summary>
        ///     All the messages
        /// </summary>
        public List<string> Messages = new List<string>();

        /// <summary>
        ///     Which group they are in
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        ///     If the user is online
        /// </summary>
        public bool IsOnline { get; set; }

        /// <summary>
        ///     The user's Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     The user's JID
        /// </summary>
        public Jid Jid { get; set; }
    }

    public class Group
    {
        public Group(string s)
        {
            GroupName = s;
        }

        public string GroupName { get; set; }

        public bool IsOpen { get; set; }
    }
}