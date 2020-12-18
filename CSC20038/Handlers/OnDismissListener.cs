using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSC20038.Handlers
{
   public class OnDismissListener : Java.Lang.Object, IDialogInterfaceOnDismissListener
   {
      private readonly Action action;

      public OnDismissListener(Action action)
      {
         this.action = action;
      }

      public void OnDismiss(IDialogInterface dialog)
      {
         this.action();
      }
   }
}