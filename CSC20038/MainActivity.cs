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
using CSC20038.Extensions;
using AlertDialog = Android.App.AlertDialog;
using CSC20038.Classes;
using CSC20038.Handlers;
using Android.Util;
using System.Collections.Generic;

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
      private AlertDialog.Builder dialog;

      /// <summary>
      /// The list view for the app.
      /// </summary>
      private List<string> items;

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

         //Create the alert builder.
         this.dialog = new AlertDialog.Builder(this);

         //Setup the location manager.
         this.SetupLocationManager();

         //Setup the buttons.
         FindViewById<Button>(Resource.Id.addButton).Click += AddLocation;
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
            this.dialog.ShowAlert(Resources.GetString(Resource.String.missing_location_permissions_title),
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
            this.dialog.ShowAlert(Resources.GetString(Resource.String.no_location_provider_title),
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
      /// <param name="sender">Sender</param>
      /// <param name="e">Event args</param>
      private void AddLocation(object sender, System.EventArgs e)
      {
         var locationModel = new LocationModel(currentLocation.Longitude,
            currentLocation.Latitude,
            currentLocation.Altitude);
         items.Add(locationModel.ToString());
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