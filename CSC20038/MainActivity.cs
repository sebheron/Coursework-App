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

namespace CSC20038
{
    [Activity(Label = "@string/csc20038", Theme = "@style/AppTheme", MainLauncher = true)]
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
        /// The alert dialog builder.
        /// </summary>
        private AlertDialog.Builder dialog;

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

                    //Get the best provider.
                    var provider = this.locationProviderHandler.GetBestProvider();

                    //If we can't access a provider.
                    if (provider == null)
                    {
                        //Tell the user there's no providers available.
                        dialog.ShowAlert("@string/no_location_provider_title",
                            "@string/no_location_provider_message",
                            new DialogButton("Close", CloseApp));
                    }
                    else
                    {
                        //Request location updates.
                        this.locationManager.RequestLocationUpdates(provider, 1000, 1, this);
                    }
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
                dialog.ShowAlert("@string/missing_location_permissions_title",
                    "@string/missing_location_permissions_message",
                    new DialogButton("Close", CloseApp));
            }
        }

        private void SetupLocationProvider()
        {
            //Get the best provider.
            var provider = this.locationProviderHandler.GetBestProvider();

            //If we can't access a provider.
            if (provider == null)
            {
                //Tell the user there's no providers available.
                dialog.ShowAlert("@string/no_location_provider_title",
                    "@string/no_location_provider_message",
                    new DialogButton("Close", CloseApp));
            }
            else
            {
                //Request location updates.
                this.locationManager.RequestLocationUpdates(provider, 1000, 1, this);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnPause()
        {
            base.OnPause();
            locationManager.RemoveUpdates(this);
        }

        protected override void OnResume()
        {
            base.OnResume();
            this.SetupLocationProvider();
        }

        public void OnLocationChanged(Location location)
        {
            this.currentLocation = location;
            Log.Debug("app", this.currentLocation.ToString());
        }

        public void OnProviderDisabled(string provider)
        {
            locationManager.RemoveUpdates(this);
            this.SetupLocationProvider();
        }

        public void OnProviderEnabled(string provider)
        {
            locationManager.RemoveUpdates(this);
            this.SetupLocationProvider();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Can be tied to a button to close the app.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event args.</param>
        public void CloseApp(object sender, DialogClickEventArgs e)
        {
            //Finish affinity will close the app.
            this.FinishAffinity();
        }
    }
}