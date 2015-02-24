using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;
using agsXMPP;
using agsXMPP.protocol.client;
using Facebook_Desktop.Logic;
using agsXMPP.protocol.iq.roster;
using Facebook_Desktop.Elements;

namespace Facebook_Desktop.Pages
{
    /// <summary>
    ///     Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void PasswordBoxPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_passwordBox.Password.Length > 0)
            {
                var fadeLabelOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.1));
                _passwordLabel.BeginAnimation(OpacityProperty, fadeLabelOutAnimation);
            }
            else
            {
                var fadeLabelInAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.1));
                _passwordLabel.BeginAnimation(OpacityProperty, fadeLabelInAnimation);
            }

            var fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.5));
            _hintLabel.BeginAnimation(OpacityProperty, fadeInAnimation);
        }

        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            Core.Username = _usernameTextBox._waterTextbox.Text;
            Core.Password = _passwordBox.Password;
            Core.XmppConnect = new XmppClientConnection("chat.facebook.com") { Port = 5222 };
            Core.XmppConnect.OnRosterItem += xmpp_OnRosterItem;
            Core.XmppConnect.OnRosterItem += (o, g) =>
            {
                if (g.GetAttribute("name") == "Facebook User")
                    return;
                var containsGroup = false;
                foreach (var group in Core.Groups.Where(group => g.GetAttribute("group") == @group.GroupName))
                    containsGroup = true;
                if (!containsGroup)
                    Core.Groups.Add(new Group(g.GetAttribute("group")));
                var item = new ChatPlayerItem
                {
                    Jid = g.Jid,
                    Group = g.GetAttribute("group"),
                    Username = g.GetAttribute("name")
                };
                foreach (var items in Core.Presences.Where(items => items.GetAttribute("name") == g.GetAttribute("name")))
                {
                    item.IsOnline = true;
                }
                if (!Core.AllPlayers.ContainsKey(g.GetAttribute("name")))
                    Core.AllPlayers.Add(g.GetAttribute("name"), item);
            };
            Core.XmppConnect.OnPresence += XmppConnectOnOnPresence;
            Core.XmppConnect.OnLogin += xmppConnect_OnLogin;
            Core.XmppConnect.OnMessage += Core.xmppConnect_OnMessage;
            Core.XmppConnect.Open(Core.Username, Core.Password);
        }

        public void XmppConnectOnOnPresence(object sender, Presence pres)
        {
            try
            {
                if (pres.GetAttribute("name") == "Facebook User")
                {
                    return;
                }
                Core.Presences.Add(pres);
                try
                {
                    var chatItem = new ChatPlayerItem
                    {
                        Group = pres.GetAttribute("group"),
                        IsOnline = true,
                        Jid = new Jid(pres.GetAttribute("jid")),
                        Messages = new List<string>(),
                        Username = pres.GetAttribute("name")
                    };
                    Core.AllPlayers.Add(pres.GetAttribute("name"), chatItem);
                }
                catch
                {
                    var item = Core.AllPlayers[pres.GetAttribute("name")];
                    Core.AllPlayers.Remove(pres.GetAttribute("name"));
                    item.IsOnline = true;
                    Core.AllPlayers.Add(pres.GetAttribute("name"), item);
                }
            }
            catch
            {
                //MessageBox.Show(ex.ToString());
            }
        }

        public void xmpp_OnRosterItem(object sender, RosterItem item)
        {
            try
            {
                Core.UserList.Add(item);
                Core.RunOnUiThread(() =>
                    {
                        if (item.GetAttribute("name") == "Facebook User")
                        {
                            return;
                        }
                        try
                        {
                            var chatItem = new ChatPlayerItem
                            {
                                Group = item.GetAttribute("group"),
                                Jid = item.Jid,
                                Messages = new List<string>(),
                                Username = item.GetAttribute("name")
                            };
                            Core.AllPlayers.Add(item.GetAttribute("name"), chatItem);
                        }
                        catch
                        {
                            var itemPlayer = Core.AllPlayers[item.GetAttribute("name")];
                            Core.AllPlayers.Remove(item.GetAttribute("name"));
                            Core.AllPlayers.Add(item.GetAttribute("name"), itemPlayer);
                        }
                        var control = new PlayerChatControl
                        {
                            _playerNameLabel = { Content = item.GetAttribute("name") },
                            Tag = Core.AllPlayers[item.GetAttribute("name")]
                        };
                        Core.PlayerChatControls.Add(item.GetAttribute("jid"), control);
                    });
            }
            catch
            {
                //MessageBox.Show(ex.ToString());
            }
        }

        public void xmppConnect_OnLogin(object sender)
        {
            Core.RunOnUiThread(() =>
                {
                    Core.SwitchPage<MainPage>(true, this);
                });
        }
    }
}