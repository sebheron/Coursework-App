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

namespace CSC20038
{
   [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
   public class MainActivity : AppCompatActivity, ILocationListener
   {
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

         //Set the context view to the main view.
         this.SetContentView(Resource.Layout.activity_main);

         //Setup the permissions required for running.
         this.SetupPermissions();

         //Create the alert builder.
         this.dialogHandler = new DialogHandler(this);

         //Create the location database handler.
         this.locationDatabaseHandler = new LocationDatabaseHandler();

         //Setup the location manager.
         this.SetupLocationManager();

         //Setup the buttons.
         this.SetupButtons();

         //Setup the list view.
         this.SetupListView();
      }

      /// <summary>
      /// Setup the permissions for the application.
      /// </summary>
      private void SetupPermissions()
      {
         //We'll get the permissions by creating a permissions list and then asking for the permissions all at once.
         var permissions = new List<string>();
         //This works by checking to see if we have the permission we require.
         if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Permission.Granted)
         {
            //And if not we'll add it to the requests.
            permissions.Add(Manifest.Permission.AccessFineLocation);
         }
         if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Permission.Granted)
         {
            permissions.Add(Manifest.Permission.AccessCoarseLocation);
         }
         if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != Permission.Granted)
         {
            permissions.Add(Manifest.Permission.ReadExternalStorage);
         }
         if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != Permission.Granted)
         {
            permissions.Add(Manifest.Permission.WriteExternalStorage);
         }
         if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessWifiState) != Permission.Granted)
         {
            permissions.Add(Manifest.Permission.AccessWifiState);
         }
         if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessNetworkState) != Permission.Granted)
         {
            permissions.Add(Manifest.Permission.AccessNetworkState);
         }

         //If any permissions are required.
         if (permissions.Count > 0)
         {
            //Then request the permissions.
            this.RequestPermissions(permissions.ToArray(), 1);
         }
      }

      /// <summary>
      /// Setup the location manager.
      /// </summary>
      private void SetupLocationManager()
      {
         //Check to see if we have location permission
         if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Permission.Granted)
         {
            //And then get the location manager.
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
         else
         {
            //Tell the user we don't have location permissions and need access.
            this.dialogHandler.ShowAlert(Resources.GetString(Resource.String.missing_location_permissions_title),
                Resources.GetString(Resource.String.missing_location_permissions_message),
                new DialogButton(Resources.GetString(Resource.String.close_button_title), CloseApp));
            ;
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
                new DialogButton(Resources.GetString(Resource.String.close_button_title), CloseApp));
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
      /// Setup the buttons for the main layout.
      /// </summary>
      private void SetupButtons()
      {
         //Setup the buttons.
         FindViewById<Button>(Resource.Id.addButton).Click += AddLocation;
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
      /// <param name="sender">Sender.</param>
      /// <param name="e">Event args.</param>
      private void AddLocation(object sender, System.EventArgs e)
      {
         //Create a new location model with the title set to default and the longitude, latitude and altitude all set tot their respecitve values.
         var locationModel = new LocationModel
         {
            Title = "New Location",
            Longitude = currentLocation.Longitude,
            Latitude = currentLocation.Latitude,
            Altitude = currentLocation.Altitude
         };
         //Once we've created the item we'll open it for editing.
         this.OpenEdit(locationModel);
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
         this.OpenItemSettings(locationModel);
      }

      /// <summary>
      /// Open the settings for the location model.
      /// </summary>
      /// <param name="locationModel">The location model to open.</param>
      private void OpenItemSettings(LocationModel locationModel)
      {
         //Show the settings dialog.
         //This dialog shows the selected item and offers, delete, edit and back options.
         this.dialogHandler.ShowAlert("Settings",
            locationModel.ToString(),
            new DialogButton("Delete", (s, e) => DeleteLocationModel(locationModel)),
            new DialogButton("Edit", (s, e) => OpenEdit(locationModel)),
            new DialogButton("Back"));
      }

      /// <summary>
      /// Open the edit layout.
      /// </summary>
      /// <param name="locationModel">The location model to edit.</param>
      public void OpenEdit(LocationModel locationModel)
      {
         //Create a new intent for displaying.
         Intent intent = new Intent(this, typeof(DisplayLocationActivity));
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
            //Reset the list.
            this.ResetList();
            this.locationListAdapter.NotifyDataSetChanged();
         }
      }

      /// <summary>
      /// Delete a location model from the stored locations.
      /// </summary>
      /// <param name="locationModel">THe location model to delete.</param>
      public void DeleteLocationModel(LocationModel locationModel)
      {
         //Check the location model still exists.
         if (this.locationModels.Contains(locationModel))
         {
            //Delete it and reset the list.
            this.locationDatabaseHandler.Delete(locationModel);
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