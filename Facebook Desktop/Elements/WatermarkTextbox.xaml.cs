using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Facebook_Desktop.Elements
{
    /// <summary>
    ///     Interaction logic for WatermarkTextbox.xaml
    /// </summary>
    /// <summary>
    ///     Interaction logic for WatermarkTextbox.xaml
    /// </summary>
    public partial class WatermarkTextbox
    {
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register(
            "Watermark", typeof(string), typeof(WatermarkTextbox), new PropertyMetadata(string.Empty));

        public WatermarkTextbox()
        {
            InitializeComponent();

            DataContext = this;
        }

        public string Watermark
        {
            get { return (string) GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        private void WaterTextboxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_waterTextbox.Text.Length > 0)
            {
                var fadeLabelOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.1));
                _hintLabel.BeginAnimation(OpacityProperty, fadeLabelOutAnimation);
            }
            else
            {
                var fadeLabelInAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.1));
                _hintLabel.BeginAnimation(OpacityProperty, fadeLabelInAnimation);
            }
        }
    }
}