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
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void ChangeStatus(object sender, MouseButtonEventArgs e)
        {
            Ellipse Status = (Ellipse)sender;

            //Client.IsAway = false;
            var moveAnimation = new ThicknessAnimation(new Thickness(22, 0, 0, 2), TimeSpan.FromSeconds(0.25));
            if (Status.Name == "OnlineStatusEllipse")
            {
                //Client.CurrentPresence = PresenceType.available;
            }
            else if (Status.Name == "BusyStatusEllipse")
            {
                moveAnimation = new ThicknessAnimation(new Thickness(88, 0, 0, 2), TimeSpan.FromSeconds(0.25));
                //Client.IsAway = true;
                //Client.CurrentPresence = PresenceType.available;
            }
            else
            {
                moveAnimation = new ThicknessAnimation(new Thickness(152.5, 0, 0, 2), TimeSpan.FromSeconds(0.25));
                //Client.CurrentPresence = PresenceType.invisible;
            }
            StatusRectangle.BeginAnimation(Rectangle.MarginProperty, moveAnimation);
            //Client.SetPresence();
        }

        private void HeaderTriggerGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowHeader();
        }

        private void HeaderGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Core.CurrentPage == typeof(MainPage))
                return;

            HideHeader();
        }

        private void ChatTriggerGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            ChatTriggerGrid.Visibility = Visibility.Hidden;

            var moveAnimation = new ThicknessAnimation(new Thickness(0, ChatGrid.Margin.Top, 0, 30), TimeSpan.FromSeconds(0.25));
            ChatGrid.BeginAnimation(Grid.MarginProperty, moveAnimation);
        }

        private void ChatGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            ChatTriggerGrid.Visibility = Visibility.Visible;

            var moveAnimation = new ThicknessAnimation(new Thickness(0, ChatGrid.Margin.Top, -190, 30), TimeSpan.FromSeconds(0.25));
            ChatGrid.BeginAnimation(Grid.MarginProperty, moveAnimation);
        }

        private void ShowHeader()
        {
            var moveAnimation = new ThicknessAnimation(new Thickness(0, 30, 0, 0), TimeSpan.FromSeconds(0.25));
            HeaderGrid.BeginAnimation(Grid.MarginProperty, moveAnimation);
            moveAnimation = new ThicknessAnimation(new Thickness(0, 130, ChatGrid.Margin.Right, 30), TimeSpan.FromSeconds(0.25));
            ChatGrid.BeginAnimation(Grid.MarginProperty, moveAnimation);

            var fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.25));
            TrianglePoly.BeginAnimation(Polygon.OpacityProperty, fadeOutAnimation);
        }

        private void HideHeader()
        {
            var moveAnimation = new ThicknessAnimation(new Thickness(0, -60, 0, 0), TimeSpan.FromSeconds(0.25));
            HeaderGrid.BeginAnimation(Grid.MarginProperty, moveAnimation);
            moveAnimation = new ThicknessAnimation(new Thickness(0, 40, ChatGrid.Margin.Right, 30), TimeSpan.FromSeconds(0.25));
            ChatGrid.BeginAnimation(Grid.MarginProperty, moveAnimation);

            var fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.25));
            TrianglePoly.BeginAnimation(Polygon.OpacityProperty, fadeInAnimation);
        }
    }
}
