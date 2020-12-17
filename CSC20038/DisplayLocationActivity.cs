using Android.App;
using Android.OS;
using Android.Widget;
using CSC20038.Classes;
using CSC20038.Handlers;

namespace CSC20038
{
   [Activity(Label = "DisplayLocationActivity")]
   public class DisplayLocationActivity : Activity
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
      /// The on create method.
      /// </summary>
      /// <param name="savedInstanceState">The saved instance state.</param>
      protected override void OnCreate(Bundle savedInstanceState)
      {
         //Start by calling the base create method.
         base.OnCreate(savedInstanceState);

         //Initialize the essentials with Androids activity and bundle.
         Xamarin.Essentials.Platform.Init(this, savedInstanceState);

         //Set the context view to location item view.
         SetContentView(Resource.Layout.location_item);

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
         this.locationTitleText = FindViewById<EditText>(Resource.Id.locationTitleText);
         this.noteText = FindViewById<EditText>(Resource.Id.noteText);
         //Set the values for the textboxes.
         locationTitleText.Text = locationModel.Title;
         noteText.Text = locationModel.Note;

         //Show the longitude, latitude and alitude values.
         FindViewById<TextView>(Resource.Id.longitudeTextView).Text = $"Longitude: {locationModel.Longitude}";
         FindViewById<TextView>(Resource.Id.latitudeTextView).Text = $"Latitude: {locationModel.Latitude}";
         FindViewById<TextView>(Resource.Id.altitudeTextView).Text = $"Altitude: {locationModel.Altitude}";

         //Get the buttons.
         var submitButton = FindViewById<Button>(Resource.Id.submitButton);
         var cancelButton = FindViewById<Button>(Resource.Id.cancelButton);

         //Setup the buttons methods.
         submitButton.Click += (s, e) => SubmitLocationModel(locationModel, locationTitleText.Text, noteText.Text);
         cancelButton.Click += (s, e) => CloseLocationModelEdit();
      }

      /// <summary>
      /// Submit the location model.
      /// </summary>
      /// <param name="locationModel"></param>
      /// <param name="title"></param>
      /// <param name="note"></param>
      private void SubmitLocationModel(LocationModel locationModel, string title, string note)
      {
         //Set the titles for the location models.
         locationModel.Title = title;
         locationModel.Note = note;

         //If the location database has the location model already in it.
         if (this.locationDatabaseHandler.Has(locationModel))
         {
            //Update the location model in the database.
            this.locationDatabaseHandler.Update(locationModel);
         }
         //If it doesn't have the location model.
         else
         {
            //Add the location model to the database.
            this.locationDatabaseHandler.Insert(locationModel);
         }
         //Close this activity with result code 1 for submitting.
         this.FinishActivity(1);
      }

      /// <summary>
      /// Close the location model editing without saving.
      /// </summary>
      private void CloseLocationModelEdit()
      {
         //Close this activity with result code 0 for cancelling.
         this.FinishActivity(0);
      }
   }
}