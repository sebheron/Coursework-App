using Android.Locations;

namespace CSC20038.Handlers
{
   public class LocationProviderHandler
   {
      /// <summary>
      /// The criterias we'll use for getting the respective providers.
      /// </summary>
      private Criteria fineCriteria, coarseCriteria;

      /// <summary>
      /// The location manager we'll use to get the best provider.
      /// </summary>
      private LocationManager locationManager;

      /// <summary>
      /// The last location provider retrieved.
      /// </summary>
      private string currentLocationProvider;

      /// <summary>
      /// The provider handler will help us get the most suitable location provider.
      /// </summary>
      /// <param name="locationManager">The location manager for the application.</param>
      public LocationProviderHandler(LocationManager locationManager)
      {
         //Create the fine location criteria.
         this.fineCriteria = new Criteria();
         this.fineCriteria.Accuracy = Accuracy.Fine;
         this.fineCriteria.PowerRequirement = Power.Medium;

         //Create the coarse location criteria.
         this.coarseCriteria = new Criteria();
         this.coarseCriteria.Accuracy = Accuracy.Coarse;
         this.coarseCriteria.PowerRequirement = Power.Medium;

         //Set the location manager.
         this.locationManager = locationManager;
      }

      /// <summary>
      /// Get the best provider for getting the location.
      /// </summary>
      /// <returns>The best available provider. Null if none are available.</returns>
      public string GetBestProvider()
      {
         //Using the fine criteria, we'll attempt to get the fine location provider of the user.
         this.currentLocationProvider = this.locationManager.GetBestProvider(this.fineCriteria, true);

         //If we can't get the fine location provider.
         if (this.currentLocationProvider == null)
         {
            //Then we'll set the provider to the coarse location provider using the coarse criteria.
            this.currentLocationProvider = this.locationManager.GetBestProvider(this.coarseCriteria, true);
         }

         //Then we return the new provider.
         return this.currentLocationProvider;
      }

      /// <summary>
      /// Gets whether a location provider is the same as the current provider.
      /// </summary>
      /// <param name="provider">The provider to check.</param>
      /// <returns>Whether the providers are the same.</returns>
      public bool IsCurrentProvider(string provider)
      {
         //Is the new provider the same as the current provider.
         if (provider.Equals(this.currentLocationProvider))
         {
            return true;
         }
         //If it isn't the same return false.
         return false;
      }
   }
}