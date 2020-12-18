using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using CSC20038.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSC20038.Handlers
{
   public class DialogHandler
   {
      /// <summary>
      /// An empty string constant.
      /// </summary>
      private const string EMPTY_STRING = "";

      /// <summary>
      /// The dialog builder.
      /// </summary>
      private readonly AlertDialog.Builder dialogBuilder;

      /// <summary>
      /// The application context.
      /// </summary>
      private readonly Activity context;

      /// <summary>
      /// The dialog handler will handle creation of new dialogs.
      /// </summary>
      /// <param name="context">The application context.</param>
      public DialogHandler(Activity context)
      {
         this.context = context;
         this.dialogBuilder = new AlertDialog.Builder(this.context);
      }

      /// <summary>
      /// Show an alert to the user.
      /// </summary>
      /// <param name="title">The title of the alert.</param>
      /// <param name="message">The message of the alert.</param>
      /// <param name="button1">The first button.</param>
      /// <param name="button2">The second button.</param>
      /// <param name="button3">The third button.</param>
      public void ShowAlert(string title = EMPTY_STRING, string message = EMPTY_STRING, DialogButton button1 = null, DialogButton button2 = null, DialogButton button3 = null)
      {
         //Create the new alert
         AlertDialog alert = dialogBuilder.Create();

         //Set the title and message.
         alert.SetTitle(title);
         alert.SetMessage(message);

         //Set the buttons
         if (button1 != null)
         {
            alert.SetButton(button1.Title, button1.Action);
         }

         if (button2 != null)
         {
            alert.SetButton2(button2.Title, button2.Action);
         }

         if (button3 != null)
         {
            alert.SetButton3(button3.Title, button3.Action);
         }

         //Show the alert.
         alert.Show();
      }

      /// <summary>
      /// Show an alert which simply notifies the user with only 1 optional change in action.
      /// </summary>
      /// <param name="title">The title of the alert.</param>
      /// <param name="message">The message of the alert.</param>
      public void ShowInertAlert(string title = EMPTY_STRING, string message = EMPTY_STRING, EventHandler e = null, DialogButton button1 = null)
      {
         //Create the new alert
         AlertDialog alert = dialogBuilder.Create();

         //Set the title and message.
         alert.SetTitle(title);
         alert.SetMessage(message);

         //Setup the dismiss listener if not null
         if (e != null)
         {
            alert.SetOnDismissListener(new OnDismissListener(() =>
            {
               //Invoke the event attached.
               e.Invoke(this, EventArgs.Empty);
            }));

            //Setup the dismiss button.
            alert.SetButton(context.Resources.GetString(Resource.String.item_options_close), (s, e) => alert.Dismiss());

            //Setup the button.
            if (button1 != null)
            {
               alert.SetButton2(button1.Title, button1.Action);
            }
         }
         else if (button1 != null)
         {
            alert.SetButton(button1.Title, button1.Action);
         }

         //Show the alert.
         alert.Show();
      }
   }
}