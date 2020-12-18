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

      /// <summary>
      /// Get the item id at the position.
      /// </summary>
      /// <param name="position"></param>
      /// <returns></returns>
      public override long GetItemId(int position)
      {
         return this.items[position].ID;
      }

      /// <summary>
      /// Get the item at the position.
      /// </summary>
      /// <param name="position">The requested position.</param>
      /// <returns>The item at the position.</returns>
      public override string this[int position]
      {
         get { return items[position].Title; }
      }

      /// <summary>
      /// Gets the total number of items.
      /// </summary>
      public override int Count
      {
         get { return items.Count; }
      }

      /// <summary>
      /// Gets the view for the listview item.
      /// </summary>
      /// <param name="position">The requested position.</param>
      /// <param name="convertView">The view for the item.</param>
      /// <param name="parent">The parent group.</param>
      /// <returns></returns>
      public override View GetView(int position, View convertView, ViewGroup parent)
      {
         View view = convertView;
         if (view == null)
            view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
         view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = this[position];
         return view;
      }
   }
}