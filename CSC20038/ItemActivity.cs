using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using CSC20038.Classes;
using CSC20038.Handlers;

namespace CSC20038
{
   [Activity(Label = "@string/item_name")]
   public class ItemActivity : AppCompatActivity
   {
      /// <summary>
      /// The location database handler we'll use to store and retrieve locations.
      /// </summary>
      private LocationDatabaseHandler locationDatabaseHandler;

      /// <summary>
      /// The location model being edited.
      /// </summary>
      private LocationModel locationModel;

      /// <summary>
      /// The title text box.
      /// </summary>
      private EditText locationTitleText;

      /// <summary>
      /// The note text box.
      /// </summary>
      private EditText noteText;

      /// <summary>
      /// The alert dialog builder.
      /// </summary>
      private DialogHandler dialogHandler;

      /// <summary>
      /// The on create method.
      /// </summary>
      /// <param name="savedInstanceState">The saved instance state.</param>
      protected override void OnCreate(Bundle savedInstanceState)
      {
         //Start by calling the base create method.
         base.OnCreate(savedInstanceState);

         //Initialize the essentials with Androids activity and bundle.
         Xamarin.Essentials.Platform.Init(this, savedInstanceState);

         //Create the dialog handler.
         this.dialogHandler = new DialogHandler(this);

         //Set the context view to location item view.
         SetContentView(Resource.Layout.activity_item);

         //Set the tool bar.
         this.SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar));

         //Get the intents and extras.
         Bundle extras = this.Intent.Extras;
         //If no extras are present.
         if (extras == null)
         {
            //Finish the activity.
            this.FinishActivity(0);
         }

         //Get the id from the extras.
         int id = extras.GetInt("ID");

         //Create the location database handler.
         this.locationDatabaseHandler = new LocationDatabaseHandler();

         //Retrieve the location model from the database.
         this.locationModel = this.locationDatabaseHandler.GetByID(id);

         //Get the textboxes.
         this.locationTitleText = this.FindViewById<EditText>(Resource.Id.locationTitleText);
         this.noteText = this.FindViewById<EditText>(Resource.Id.noteText);

         //Set the values for the textboxes.
         this.locationTitleText.Text = locationModel.Title;
         this.noteText.Text = locationModel.Note;

         //Show the longitude, latitude and alitude values.
         this.FindViewById<TextView>(Resource.Id.longitudeTextView).Text = string.Format(Resources.GetString(Resource.String.longitude_format), locationModel.Longitude);
         this.FindViewById<TextView>(Resource.Id.latitudeTextView).Text = string.Format(Resources.GetString(Resource.String.latitude_format), locationModel.Latitude);
         this.FindViewById<TextView>(Resource.Id.altitudeTextView).Text = string.Format(Resources.GetString(Resource.String.altitude_format), locationModel.Altitude);
      }

      /// <summary>
      /// Creates the options menu.
      /// </summary>
      /// <param name="menu">The menu.</param>
      /// <returns>Success</returns>
      public override bool OnCreateOptionsMenu(IMenu menu)
      {
         this.MenuInflater.Inflate(Resource.Menu.menu_item, menu);
         return true;
      }

      /// <summary>
      /// When an option is selected.
      /// </summary>
      /// <param name="item">The menu item selected.</param>
      /// <returns></returns>
      public override bool OnOptionsItemSelected(IMenuItem item)
      {
         switch (item.ItemId)
         {
            case Resource.Id.accept_location:
               this.Accept();
               return true;
            case Resource.Id.delete_location:
               this.CheckDelete();
               return true;
         }
         return base.OnOptionsItemSelected(item);
      }

      /// <summary>
      /// On back pressed.
      /// </summary>
      public override void OnBackPressed()
      {
         //We'll accept on back pressed.
         this.Accept();
      }

      /// <summary>
      /// Save the changes to the location
      /// </summary>
      public void Accept()
      {
         //Set the titles for the location models.
         this.locationModel.Title = locationTitleText.Text;
         this.locationModel.Note = noteText.Text;

         //Update the location model in the database.
         this.locationDatabaseHandler.Update(locationModel);

         //Close this activity.
         this.Finish();
      }

      /// <summary>
      /// Checks to see if the user wants to delete all.
      /// </summary>
      private void CheckDelete()
      {
         //Show a Yes/No dialog and set the callback for Yes to be the delete method.
         this.dialogHandler.ShowAlert(Resources.GetString(Resource.String.delete_title),
             Resources.GetString(Resource.String.delete_individual_message),
             new DialogButton(Resources.GetString(Resource.String.no_button_title)),
             new DialogButton(Resources.GetString(Resource.String.yes_button_title), (s, e) => this.Delete()));
      }

      /// <summary>
      /// Delete the location.
      /// </summary>
      public void Delete()
      {
         //Delete the location model from the database.
         this.locationDatabaseHandler.Delete(locationModel);

         //Close this activity.
         this.Finish();
      }
   }
}