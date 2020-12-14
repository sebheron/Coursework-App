using Android.Content;
using System;

namespace CSC20038.Classes
{
    public class DialogButton
    {
        public string Title;

        public EventHandler<DialogClickEventArgs> Action;

        /// <summary>
        /// A container for a button displayed on an alert.
        /// </summary>
        /// <param name="title">The title of the button.</param>
        /// <param name="action">The method to call on click.</param>
        public DialogButton(string title, EventHandler<DialogClickEventArgs> action)
        {
            this.Title = title;
            this.Action = action;
        }
    }
}