namespace Facebook_Desktop.Elements
{
    /// <summary>
    /// Interaction logic for ChatItemMessage.xaml
    /// </summary>
    public partial class ChatItemMessage
    {
        public ChatItemMessage(string message)
        {
            InitializeComponent();
            MessageLabel.Content = message;
        }
    }
}
