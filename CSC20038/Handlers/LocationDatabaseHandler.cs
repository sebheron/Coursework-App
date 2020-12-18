using CSC20038.Classes;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;

namespace CSC20038.Handlers
{
   public class LocationDatabaseHandler : IDisposable
   {
      /// <summary>
      /// The database directory.
      /// </summary>
      public static readonly string DB_DIRECTORY = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
      
      /// <summary>
      /// The database path.
      /// </summary>
      public static readonly string DB_PATH = Path.Combine(DB_DIRECTORY, "locations.db3");

      /// <summary>
      /// The database.
      /// </summary>
      private SQLiteConnection database;

      /// <summary>
      /// Create a database handler, which will open a connection to a database and close it when disposed.
      /// </summary>
      public LocationDatabaseHandler()
      {
         string dir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
         Directory.CreateDirectory(dir);
         //For my database implementation I'm using an SQLite implementation which works efficiently with C#.
         //This allows for easier adding of columns due to everything being attributed to the model.
         //It also allows for easy access to items with Linq statements.
         this.database = new SQLiteConnection(DB_PATH);
         this.database.CreateTable<LocationModel>();
      }

      /// <summary>
      /// Get an entry in the database by its ID.
      /// </summary>
      /// <param name="id">The ID of the entry to retrieve.</param>
      /// <returns>The location model corresponding to the ID supplied.</returns>
      public LocationModel GetByID(int id)
      {
         return this.database.Table<LocationModel>().Where(x => x.ID == id).First();
      }

      /// <summary>
      /// Checks to see whether the database has an entry.
      /// </summary>
      /// <param name="locationModel">The location model to check.</param>
      /// <returns>True if the model exists in the database. False if it isn't.</returns>
      public bool Has(LocationModel locationModel)
      {
         var storedModel = this.database.Table<LocationModel>().FirstOrDefault(x => x.ID == locationModel.ID);
         return storedModel != null;
      }

      /// <summary>
      /// Gets all the location models.
      /// </summary>
      /// <returns>A list of location models stored in the database.</returns>
      public List<LocationModel> GetAll()
      {
         return this.database.Table<LocationModel>().ToList();
      }

      /// <summary>
      /// Insert a new entry.
      /// </summary>
      /// <param name="locationModel">The location model to insert.</param>
      /// <returns></returns>
      public bool Insert(LocationModel locationModel)
      {
         var i = this.database.Insert(locationModel);
         if (i < 0) return false;
         return true;
      }

      /// <summary>
      /// Update a entry in the database.
      /// </summary>
      /// <param name="locationModel">The location model to update.</param>
      public void Update(LocationModel locationModel)
      {
         this.database.Update(locationModel);
      }

      /// <summary>
      /// Delete a entry from the database.
      /// </summary>
      /// <param name="locationModel">The location model to delete.</param>
      public void Delete(LocationModel locationModel)
      {
         this.database.Delete(locationModel);
      }

      /// <summary>
      /// Clear the databse.
      /// </summary>
      public void Clear()
      {
         //Drop the database table and create a new table.
         this.database.DropTable<LocationModel>();
         this.database.CreateTable<LocationModel>();
      }

      /// <summary>
      /// Close the database on disposal.
      /// </summary>
      public void Dispose()
      {
         this.database.Close();
      }
   }
}