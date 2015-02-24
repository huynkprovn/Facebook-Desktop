using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using agsXMPP.protocol.client;
using Facebook_Desktop.Elements;
using Facebook_Desktop.Logic;
using Presence = agsXMPP.protocol.component.Presence;

namespace Facebook_Desktop.Pages
{
    /// <summary>
    ///     Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage
    {
        private List<SmallChatItem> onlineList = new List<SmallChatItem>();
        private List<SmallChatItem> offlineList = new List<SmallChatItem>();
        bool loaded;
        internal ChatBoxControl ChatControl;
        public MainPage(LoginPage loginPage)
        {
            InitializeComponent();
            Core.MainPage = this;
            if (Core.UserList.Count != 0)
            {
                Core.XmppConnect.OnRosterItem -= loginPage.xmpp_OnRosterItem;
                Core.XmppConnect.OnPresence -= loginPage.XmppConnectOnOnPresence;
                Core.XmppConnect.OnPresence += XmppConnectOnOnPresence;
                Core.XmppConnect.OnLogin -= loginPage.xmppConnect_OnLogin;
            }
            _nameLabel.Content = Core.XmppConnect.MyJID.User;
            var temp = new Thread(() =>
            {
                    while (!loaded)
                    {
                        Thread.Sleep(10000);
                        Load();
                    }
                });
            temp.Start();
        }

        private void Load()
        {
            Core.RunOnUiThread(() =>
            {
                ChatStackPanel.Children.Clear();
                onlineList.Clear();
                offlineList.Clear();
                foreach (var chatItem in from userList in Core.UserList where userList.GetAttribute("name") != "Facebook User" select new SmallChatItem
                {
                    NameLabel = { Content = userList.GetAttribute("name") },
                    StatusLabel = { Content = userList.GetAttribute("jid") },
                    Tag = Core.AllPlayers[userList.GetAttribute("name")]
                }) {
                    chatItem.MouseDoubleClick += ItemMouseDoubleClick;
                    var converter = new BrushConverter();
                    offlineList.Add(chatItem);
                    var item = chatItem;
                    foreach (var onlineUsers in Core.Presences.Where(onlineUsers => onlineUsers.From.User == ((ChatPlayerItem)item.Tag).Username))
                    {
                        chatItem.StatusEllipse.Fill = (Brush)converter.ConvertFrom("#2ecc71");
                        onlineList.Add(chatItem);
                        offlineList.Remove(chatItem);
                    }

                    loaded = true;
                }
            });
            Core.RunOnUiThread(() =>
            {
                onlineList = onlineList.OrderBy(o => o.NameLabel.Content).ToList();
                offlineList = offlineList.OrderBy(o => o.NameLabel.Content).ToList();

                foreach (var small in onlineList)
                    ChatStackPanel.Children.Add(small);
                foreach (var small in offlineList)
                    ChatStackPanel.Children.Add(small);


                foreach (PlayerChatControl controls in _playerChatStackPanel.Children)
                {
                    var converter = new BrushConverter();
                    controls._statusEllipse.Fill = (Brush)converter.ConvertFrom("#FFA0A0A0");
                    var control = controls;
                    foreach (var online in onlineList.Where(online => control._playerNameLabel == online.NameLabel))
                    {
                        controls._statusEllipse.Fill = (Brush)converter.ConvertFrom("#2ecc71");
                    }
                }
            });
        }
        private void ChangeStatus(object sender, MouseButtonEventArgs e)
        {
            var status = (Ellipse) sender;

            //Client.IsAway = false;
            var moveAnimation = new ThicknessAnimation(new Thickness(22, 0, 0, 2), TimeSpan.FromSeconds(0.25));
            switch (status.Name) {
                case "_online":
                    Core.XmppConnect.Send(new Presence(ShowType.chat, "Web"));
                    break;
                case "_onlineSome":
                    Core.XmppConnect.Send(new Presence(ShowType.dnd, "Web"));
                    moveAnimation = new ThicknessAnimation(new Thickness(88, 0, 0, 2), TimeSpan.FromSeconds(0.25));
                    break;
                default:
                    Core.XmppConnect.Send(new Presence(ShowType.xa, "Web"));
                    moveAnimation = new ThicknessAnimation(new Thickness(152.5, 0, 0, 2), TimeSpan.FromSeconds(0.25));
                    break;
            }
            _statusRectangle.BeginAnimation(MarginProperty, moveAnimation);
        }

        public void XmppConnectOnOnPresence(object sender, agsXMPP.protocol.client.Presence pres)
        {
            try
            {
                if (pres.GetAttribute("name") == "Facebook User")
                    return;
                switch (pres.Type) 
                {
                    case PresenceType.available:
                        if (Core.Presences.Contains(pres))
                            Core.Presences.Remove(pres);
                        Core.Presences.Add(pres);
                        
                        var itemOnline = Core.AllPlayers[pres.GetAttribute("name")];
                        Core.AllPlayers.Remove(pres.GetAttribute("name"));
                        itemOnline.IsOnline = true;
                        Core.AllPlayers.Add(pres.GetAttribute("name"), itemOnline);
                        break;
                    case PresenceType.unsubscribe:
                    case PresenceType.unsubscribed:
                    case PresenceType.invisible:
                        if (Core.Presences.Contains(pres))
                            Core.Presences.Remove(pres);
                        
                        var itemOffline = Core.AllPlayers[pres.GetAttribute("name")];
                        Core.AllPlayers.Remove(pres.GetAttribute("name"));
                        itemOffline.IsOnline = false;
                        Core.AllPlayers.Add(pres.GetAttribute("name"), itemOffline);
                        break;
                }
                Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        void ItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = (SmallChatItem)sender;
            var player = (ChatPlayerItem)item.Tag;

            if (_playerChatStackPanel.Children.Cast<PlayerChatControl>().Any(items => (string)items._playerNameLabel.Content == player.Username && items.Visibility != Visibility.Collapsed)) {
                return;
            }

            var playerControl = new PlayerChatControl
            {
                Tag = player,
                _playerNameLabel = { Content = player.Username },
                _statusEllipse = { Fill = item.StatusEllipse.Fill },
                Margin = new Thickness(5, 0, 0, 0)
            };
            playerControl.MouseDown += PlayerControlMouseDown;
            _playerChatStackPanel.Children.Add(playerControl);
        }
        void PlayerControlMouseDown(object sender, MouseButtonEventArgs e)
        {
            var playerControl = (PlayerChatControl)sender;
            var item = (ChatPlayerItem)playerControl.Tag;
            if (ChatControl == null)
            {
                ChatControl = new ChatBoxControl();
                _holderGrid.Children.Add(ChatControl);
            }
            else
            {
                var currentName = (string)ChatControl._nameLabel.Content;
                if (currentName == item.Username)
                {
                    _holderGrid.Children.Remove(ChatControl);
                    ChatControl = null;
                    return;
                }
            }

            Panel.SetZIndex(ChatControl, 1);

            ChatControl._nameLabel.Content = item.Username;

            ChatControl.HorizontalAlignment = HorizontalAlignment.Left;
            ChatControl.VerticalAlignment = VerticalAlignment.Bottom;
            var relativePoint = playerControl.TransformToAncestor(Core.Mainwin).Transform(new Point(0, 0));
            ChatControl.Margin = new Thickness(relativePoint.X, 0, 0, 30);
        }

        private void HeaderTriggerGridMouseEnter(object sender, MouseEventArgs e)
        {
            ShowHeader();
        }

        private void HeaderGridMouseLeave(object sender, MouseEventArgs e)
        {
            if (Core.CurrentPage == typeof(MainPage))
            {
                return;
            }
            HideHeader();
        }

        private void ChatTriggerGridMouseEnter(object sender, MouseEventArgs e)
        {
            _chatTriggerGrid.Visibility = Visibility.Hidden;

            var moveAnimation = new ThicknessAnimation(
                new Thickness(0, _chatGrid.Margin.Top, 0, 30), TimeSpan.FromSeconds(0.25));
            _chatGrid.BeginAnimation(MarginProperty, moveAnimation);
        }

        private void ChatGridMouseLeave(object sender, MouseEventArgs e)
        {
            _chatTriggerGrid.Visibility = Visibility.Visible;

            var moveAnimation = new ThicknessAnimation(
                new Thickness(0, _chatGrid.Margin.Top, -190, 30), TimeSpan.FromSeconds(0.25));
            _chatGrid.BeginAnimation(MarginProperty, moveAnimation);
        }

        private void ShowHeader()
        {
            var moveAnimation = new ThicknessAnimation(new Thickness(0, 30, 0, 0), TimeSpan.FromSeconds(0.25));
            _headerGrid.BeginAnimation(MarginProperty, moveAnimation);
            moveAnimation = new ThicknessAnimation(
                new Thickness(0, 130, _chatGrid.Margin.Right, 30), TimeSpan.FromSeconds(0.25));
            _chatGrid.BeginAnimation(MarginProperty, moveAnimation);

            var fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.25));
            _trianglePoly.BeginAnimation(OpacityProperty, fadeOutAnimation);
        }

        private void HideHeader()
        {
            var moveAnimation = new ThicknessAnimation(new Thickness(0, -60, 0, 0), TimeSpan.FromSeconds(0.25));
            _headerGrid.BeginAnimation(MarginProperty, moveAnimation);
            moveAnimation = new ThicknessAnimation(
                new Thickness(0, 40, _chatGrid.Margin.Right, 30), TimeSpan.FromSeconds(0.25));
            _chatGrid.BeginAnimation(MarginProperty, moveAnimation);

            var fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.25));
            _trianglePoly.BeginAnimation(OpacityProperty, fadeInAnimation);
        }
    }
}