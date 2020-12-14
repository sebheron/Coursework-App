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
            string title = EMPTY_STRING,
            string message = EMPTY_STRING,
            params DialogButton[] dialogButtons)
        {
            //Check how many buttons we've been supplied.
            //If more than 3 have been supplied then too many exist.
            if (dialogButtons.Length > 3)
            {
                throw new System.Exception("Too many buttons supplied. Maxmimum amount of buttons is 3.");
            }

            //Zero buttons is also incorrect we need at least one button.
            else if (dialogButtons.Length <= 0)
            {
                throw new System.Exception("At least one button must be supplied.");
            }

            //Create the new alert
            AlertDialog alert = builder.Create();

            //Set the title and message.
            alert.SetTitle(title);
            alert.SetMessage(message);
            
            //Set the buttons
            for (int i = 1; i <= dialogButtons.Length; i++)
            {
                alert.SetButton(i, dialogButtons[i - 1].Title, dialogButtons[i - 1].Action);
            }

            //Show the alert.
            alert.Show();
        }
    }
}