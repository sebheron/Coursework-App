using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CSC20038.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSC20038.Adapters
{
   public class LocationListAdapter : BaseAdapter<string>
   {
      private List<LocationModel> items;
      private Activity context;

      /// <summary>
      /// Create a new location list adapter for displaying location models.
      /// </summary>
      /// <param name="context">The application context.</param>
      /// <param name="items">The items.</param>
      public LocationListAdapter(Activity context, List<LocationModel> items) : base()
      {
         this.context = context;
         this.items = items;
      }

      public override long GetItemId(int position)
      {
         return position;
      }

      public override string this[int position]
      {
         get { return items[position].ToString(); }
      }

      public override int Count
      {
         get { return items.Count; }
      }

      public override View GetView(int position, View convertView, ViewGroup parent)
      {
         View view = convertView;
         if (view == null)
            view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
         view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = items[position].ToString();
         return view;
      }
   }
}