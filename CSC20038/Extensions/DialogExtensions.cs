using Android.App;
using CSC20038.Classes;

namespace CSC20038.Extensions
{
    public static class DialogExtensions
    {
        /// <summary>
        /// An empty string constant.
        /// </summary>
        private const string EMPTY_STRING = "";

        /// <summary>
        /// Show an alert to the user.
        /// </summary>
        /// <param name="builder">The alert builder.</param>
        /// <param name="title">The title of the alert.</param>
        /// <param name="message">The message of the alert.</param>
        public static void ShowAlert(this AlertDialog.Builder builder,
            string title = EMPTY_STRING, string message = EMPTY_STRING, DialogButton positiveButton = null)
        {
            //Create the new alert
            AlertDialog alert = builder.Create();

            //Set the title and message.
            alert.SetTitle(title);
            alert.SetMessage(message);
            
            //Set the button
            if (positiveButton != null)
            {
                alert.SetButton(positiveButton.Title, positiveButton.Action);
            }

            //Show the alert.
            alert.Show();
        }
    }
}