using Android.Content;
using System;

namespace CSC20038.Classes
{
   public class DialogButton
   {
      /// <summary>
      /// The title of the button.
      /// </summary>
      public string Title;

      /// <summary>
      /// The event to be invoked when clicking the button.
      /// </summary>
      public EventHandler<DialogClickEventArgs> Action;

      /// <summary>
      /// A container for a button displayed on an alert.
      /// </summary>
      /// <param name="title">The title of the button.</param>
      /// <param name="action">The method to invoke on click.</param>
      public DialogButton(string title, EventHandler<DialogClickEventArgs> action = null)
      {
         this.Title = title;
         this.Action = action;
      }
   }
}