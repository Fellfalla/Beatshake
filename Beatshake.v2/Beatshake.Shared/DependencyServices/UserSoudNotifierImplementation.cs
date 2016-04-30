using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Beatshake.DependencyServices;

    class UserSoudNotifierImplementation : IUserSoudNotifier
    {
        public void Notify()
        {
            ToastTemplateType toastTemplate = ToastTemplateType.ToastText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode("You've got 5 seconds"));

            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            ((XmlElement)toastNode).SetAttribute("launch", "MainPage.xaml");

            XmlElement audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", "ms-winsoundevent:Notification.Reminder");
            toastNode.AppendChild(audio);

            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
}
