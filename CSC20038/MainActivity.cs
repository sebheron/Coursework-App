using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Locations;
using Android.Content;
using Android;
using Android.Content.PM;
using Android.Support.V4.Content;
using CSC20038.Classes;
using CSC20038.Handlers;
using System.Collections.Generic;
using CSC20038.Adapters;
using Android.Views;
using System.Linq;

namespace CSC20038
{
   [Activity(Label = "@string/main_name", MainLauncher = true)]
   public class MainActivity : AppCompatActivity, ILocationListener
   {
      /// <summary>
      /// Set according to whether the user is editing or not.
      /// </summary>
      private bool editing;

      /// <summary>
      /// The location manager for the application.
      /// </summary>
      private LocationManager locationManager;

      /// <summary>
      /// The location provider handler we'll use to get the best location provider.
      /// </summary>
      private LocationProviderHandler locationProviderHandler;

      /// <summary>
      /// The location database handler we'll use to store and retrieve locations.
      /// </summary>
      private LocationDatabaseHandler locationDatabaseHandler;

      /// <summary>
      /// The current location of the user.
      /// </summary>
      private Location currentLocation;

      /// <summary>
      /// Whether we're capable of getting the location.
      /// </summary>
      private Availability locationAvailability;

      /// <summary>
      /// The alert dialog builder.
      /// </summary>
      private DialogHandler dialogHandler;

      /// <summary>
      /// The list view for the app.
      /// </summary>
      private List<LocationModel> locationModels;

      /// <summary>
      /// The location list adapter.
      /// </summary>
      private LocationListAdapter locationListAdapter;

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

         //Set the context view to the main view.
         this.SetContentView(Resource.Layout.activity_main);

         //Set the tool bar.
         this.SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar));

         //From here on in we need permissions so we need to check and see if we have them.
         //If we do we'll continue, if not we'll wait until the permissions have been resolved.
         if (this.CheckPermissions())
         {
            //Create the location database handler.
            this.locationDatabaseHandler = new LocationDatabaseHandler();

            //Setup the location manager.
            this.SetupLocationManager();

            //Setup the list view.
            this.SetupListView();
         }
      }

      /// <summary>
      /// Creates the options menu.
      /// </summary>
      /// <param name="menu">The menu.</param>
      /// <returns>Success</returns>
      public override bool OnCreateOptionsMenu(IMenu menu)
      {
         this.MenuInflater.Inflate(Resource.Menu.menu_main, menu);
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
            case Resource.Id.add_location:
               this.AddLocation();
               return true;
            case Resource.Id.clear_location:
               this.CheckDeleteAll();
               return true;
         }
         return base.OnOptionsItemSelected(item);
      }

      /// <summary>
      /// After the permissions result has been determined.
      /// </summary>
      /// <param name="requestCode">Request code.</param>
      /// <param name="permissions">Permissions requested.</param>
      /// <param name="grantResults">Grant results.</param>
      public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
      {
         Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
         base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

         //Create a new list for unaccepted permissions.
         var unacceptedPermissions = new List<string>();

         //Loop through the permissions.
         for (int i = 0; i < permissions.Length; i++)
         {
            //If a permission has been denied.
            if (grantResults[i] == Permission.Denied)
            {
               //We'll add it to the unaccepted list so we can ask again.
               unacceptedPermissions.Add(permissions[i]);
            }
         }

         //If we've no permissions left to request
         if (unacceptedPermissions.Count <= 0)
         {
            //Create the location database handler.
            this.locationDatabaseHandler = new LocationDatabaseHandler();

            //Setup the location manager.
            this.SetupLocationManager();

            //Setup the list view.
            this.SetupListView();

            //Return and do nothing.
            return;
         }

         //Format a display string to show the user.
         var message = Resources.GetString(Resource.String.android_permissions_denied)
            + string.Format(Resources.GetString(Resource.String.android_permissions_request),
            string.Join("\n", unacceptedPermissions.Select(x => x.Split(".")[2])));

         //Display a dialog notifying the user we'll be asking for permissions. And then request them.
         this.dialogHandler.ShowInertAlert(Resources.GetString(Resource.String.missing_permissions_title), message,
            (s, e) => this.RequestPermissions(unacceptedPermissions.ToArray(), 0),
            new DialogButton(Resources.GetString(Resource.String.exit_button_title), CloseApp));   
      }

      /// <summary>
      /// Check we have the permissions required.
      /// </summary>
      private bool CheckPermissions()
      {
         //We'll get the permissions by creating a permissions list and then asking for the permissions all at once.
         var permissions = new List<string>();
         //This works by checking to see if we have the permission we require.
         if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Permission.Denied)
         {
            //And if not we'll add it to the requests.
            permissions.Add(Manifest.Permission.AccessFineLocation);
         }
         if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) == Permission.Denied)
         {
            permissions.Add(Manifest.Permission.AccessCoarseLocation);
         }
         if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) == Permission.Denied)
         {
            permissions.Add(Manifest.Permission.ReadExternalStorage);
         }
         if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) == Permission.Denied)
         {
            permissions.Add(Manifest.Permission.WriteExternalStorage);
         }
         if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessWifiState) == Permission.Denied)
         {
            permissions.Add(Manifest.Permission.AccessWifiState);
         }
         if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessNetworkState) == Permission.Denied)
         {
            permissions.Add(Manifest.Permission.AccessNetworkState);
         }

         //Check to see if we have any permissions to request.
         if (permissions.Count > 0)
         {
            //Format a display string to show the user.
            var message = string.Format(Resources.GetString(Resource.String.android_permissions_request), string.Join("\n", permissions.Select(x => x.Split(".")[2])));

            //Display a dialog notifying the user we'll be asking for permissions. And then request them.
            this.dialogHandler.ShowInertAlert(Resources.GetString(Resource.String.missing_permissions_title), message,
               (s, e) => this.RequestPermissions(permissions.ToArray(), 0));

            //Return false as permissions were requested.
            return false;
         }
         //Return true as no permissions were requested.
         return true;
      }

      /// <summary>
      /// Setup the location manager.
      /// </summary>
      private void SetupLocationManager()
      {
         //Get the location manager.
         var locationManager = this.GetSystemService(Context.LocationService) as LocationManager;

         //If the location manager exists.
         if (locationManager != null)
         {
            //Assign the location manager.
            this.locationManager = locationManager;

            //Create the provider handler.
            this.locationProviderHandler = new LocationProviderHandler(this.locationManager);

            //Setup the location provider.
            this.SetupLocationProvider();
         }
         else
         {
            //If it doesn't exist.
            throw new System.Exception("Location manager does not exist/is inaccessibile.");
         }
      }

      /// <summary>
      /// Sets up the location provider.
      /// </summary>
      private void SetupLocationProvider()
      {
         //Get the best provider.
         var provider = this.locationProviderHandler.GetBestProvider();

         //If we can't access a provider.
         if (provider == null)
         {
            //Tell the user there's no providers available.
            this.dialogHandler.ShowAlert(Resources.GetString(Resource.String.no_location_provider_title),
                Resources.GetString(Resource.String.no_location_provider_message),
               new DialogButton(Resources.GetString(Resource.String.exit_button_title), CloseApp),
               new DialogButton(Resources.GetString(Resource.String.ok_button_title), (s, e) => this.SetupLocationManager()));
         }
         else
         {
            //Request location updates.
            this.locationManager.RequestLocationUpdates(provider, 1000, 1, this);
         }
      }

      /// <summary>
      /// Setup the main list view for the main layout.
      /// </summary>
      private void SetupListView()
      {
         //Setup the list of location models.
         this.locationModels = new List<LocationModel>();

         //Reset the list
         this.ResetList();

         //Setup the location list adapter.
         this.locationListAdapter = new LocationListAdapter(this, this.locationModels);

         //Find the list view in the layout.
         var listView = FindViewById<ListView>(Resource.Id.locationList);

         //Add the new adapter.
         listView.Adapter = this.locationListAdapter;

         //Assign the Select Location method.
         listView.ItemClick += SelectLocation;
      }

      /// <summary>
      /// Reset the list, this acts as a refresh to show deletions and additions.
      /// </summary>
      private void ResetList()
      {
         //Clear the list
         this.locationModels.Clear();

         //Get all the stored models.
         var storedModels = this.locationDatabaseHandler.GetAll();

         //Loop through them and add them to the location models.
         foreach (LocationModel locationModel in storedModels)
         {
            this.locationModels.Add(locationModel);
         }
      }

      /// <summary>
      /// When the location of the user has changed.
      /// </summary>
      /// <param name="location">The new location.</param>
      public void OnLocationChanged(Location location)
      {
         //Set the current location to the new location.
         this.currentLocation = location;
      }

      /// <summary>
      /// When the location provider is disabled.
      /// </summary>
      /// <param name="provider">The provider that has been disabled.</param>
      public void OnProviderDisabled(string provider)
      {
         //If the provider disabled is our current provider.
         if (this.locationProviderHandler.IsCurrentProvider(provider))
         {
            //Then we'll find a new provider.
            this.locationManager.RemoveUpdates(this);
            this.SetupLocationProvider();
         }
      }

      /// <summary>
      /// When the location provider is enabled.
      /// </summary>
      /// <param name="provider">The provider that's been enabled.</param>
      public void OnProviderEnabled(string provider)
      {
         //For this we'll assume the new provider "could be better" than our current provider.
         //And then find a new provider, which could be the newly enabled provider.
         this.locationManager.RemoveUpdates(this);
         this.SetupLocationProvider();
      }

      /// <summary>
      /// Occurs when the location status changes.
      /// </summary>
      /// <param name="provider">Location provider.</param>
      /// <param name="status">Location status.</param>
      /// <param name="extras">Extras.</param>
      public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
      {
         //If the location services are no longer available.
         if (this.locationAvailability == Availability.Available && status != Availability.Available)
         {
            this.locationManager.RemoveUpdates(this);
         }
         //If the location services has become available.
         else if (this.locationAvailability != Availability.Available && status == Availability.Available)
         {
            //Setup the location provider.
            this.SetupLocationProvider();
         }

         //Set the current availability.
         this.locationAvailability = status;
      }

      /// <summary>
      /// Add a new location to the list.
      /// </summary>
      private void AddLocation()
      {
         //Create a new location model with the title set to default and the longitude, latitude and altitude all set tot their respecitve values.
         var locationModel = new LocationModel
         {
            Title = Resources.GetString(Resource.String.title_default),
            Longitude = currentLocation.Longitude,
            Latitude = currentLocation.Latitude,
            Altitude = currentLocation.Altitude
         };
         //Add the location model to the database.
         this.locationDatabaseHandler.Insert(locationModel);
         //Once we've created the item we'll open it for editing.
         this.OpenEdit(locationModel);
      }

      /// <summary>
      /// Checks to see if the user wants to delete all.
      /// </summary>
      private void CheckDeleteAll()
      {
         //Show a Yes/No dialog and set the callback for Yes to be the delete all method.
         this.dialogHandler.ShowAlert(Resources.GetString(Resource.String.delete_title),
             Resources.GetString(Resource.String.delete_all_message),
             new DialogButton(Resources.GetString(Resource.String.no_button_title)),
             new DialogButton(Resources.GetString(Resource.String.yes_button_title), (s, e) => this.DeleteAll()));
      }

      /// <summary>
      /// Deletes all stored location models.
      /// </summary>
      private void DeleteAll()
      {
         //Clear the database.
         this.locationDatabaseHandler.Clear();
         //Reset the list.
         this.ResetList();
         this.locationListAdapter.NotifyDataSetChanged();
      }

      /// <summary>
      /// Select a location.
      /// </summary>
      /// <param name="sender">Sender.</param>
      /// <param name="e">Event args.</param>
      private void SelectLocation(object sender, AdapterView.ItemClickEventArgs e)
      {
         //Get the selected location model.
         var locationModel = this.locationModels[e.Position];
         //Open the settings for it.
         this.ShowLocation(locationModel);
      }

      /// <summary>
      /// Shows the location and gives the option to edit.
      /// </summary>
      /// <param name="locationModel">The location to show.</param>
      public void ShowLocation(LocationModel locationModel)
      {
         //We'll use a dialog to display an item.
         this.dialogHandler.ShowAlert(locationModel.Title,
             locationModel.ToString(),
             new DialogButton(Resources.GetString(Resource.String.item_options_close)),
             null,
             new DialogButton(Resources.GetString(Resource.String.item_options_edit), (s, e) => OpenEdit(locationModel)));
      }

      /// <summary>
      /// Open the edit layout.
      /// </summary>
      /// <param name="locationModel">The location model to edit.</param>
      public void OpenEdit(LocationModel locationModel)
      {
         //Check if editing, if not set accordingly.
         //We do this to prevent multiple edit windows being opened.
         if (editing) return;
         editing = true;
         //Create a new intent for displaying.
         Intent intent = new Intent(this, typeof(ItemActivity));
         //Put the ID for the location as an extra.
         intent.PutExtra("ID", locationModel.ID);
         //Start the activity.
         this.StartActivityForResult(intent, 0);
      }

      /// <summary>
      /// Called when the result an activity has ended and returned a result.
      /// </summary>
      /// <param name="requestCode">The request code.</param>
      /// <param name="resultCode">The result code.</param>
      /// <param name="data">The intent data passed across.</param>
      protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
      {
         //If we get request code 0.
         if (requestCode == 0)
         {
            //We're no longer editing.
            editing = false;
            //Reset the list.
            this.ResetList();
            this.locationListAdapter.NotifyDataSetChanged();
         }
      }

      /// <summary>
      /// Can be tied to a button to close the app.
      /// </summary>
      /// <param name="sender">Sender</param>
      /// <param name="e">Event args.</param>
      private void CloseApp(object sender, DialogClickEventArgs e)
      {
         //Finish affinity will close the app.
         this.FinishAffinity();
      }
   }
}