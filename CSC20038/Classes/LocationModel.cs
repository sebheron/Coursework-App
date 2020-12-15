using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSC20038.Classes
{
   public class LocationModel
   {
      private string title, note;

      /// <summary>
      /// The title of the stored location.
      /// </summary>
      public string Title { get => title; }

      /// <summary>
      /// The note for the stored location.
      /// </summary>
      public string Note { get => note; }

      /// <summary>
      /// The longitude of the location.
      /// </summary>
      public double Longitude { get; }
      
      /// <summary>
      /// The latitude of the location.
      /// </summary>
      public double Latitude { get; }

      /// <summary>
      /// The altitude of the location.
      /// </summary>
      public double Altitude { get; }

      /// <summary>
      /// The ID of the item.
      /// </summary>
      public Guid ID { get; }

      /// <summary>
      /// Create a location model based on a previously created location model.
      /// </summary>
      /// <param name="title">The title of the stored location.</param>
      /// <param name="note">The note for the stored location.</param>
      /// <param name="longitude">The longitude for the stored location.</param>
      /// <param name="latitude">The latitude for the stored location.</param>
      /// <param name="altitude">The altitude for the stored location.</param>
      /// <param name="id">The id, null if new location model.</param>
      public LocationModel(
         string title,
         string note,
         double longitude,
         double latitude,
         double altitude,
         Guid id = null)
      {
         this.ID = id;
         this.title = title;
         this.note = note;
         this.Longitude = longitude;
         this.Latitude = latitude;
         this.Altitude = altitude;
      }

      /// <summary>
      /// Create a new location model. With a new ID.
      /// </summary>
      /// <param name="longitude"></param>
      /// <param name="latitude"></param>
      /// <param name="altitude"></param>
      public LocationModel(
         double longitude,
         double latitude,
         double altitude)
      {
         this.ID = Guid.NewGuid();
         this.title = "New Location";
         this.note = "";
         this.Longitude = longitude;
         this.Latitude = latitude;
         this.Altitude = altitude;
      }

      /// <summary>
      /// Set the title to a new title.
      /// </summary>
      /// <param name="newTitle">The new title.</param>
      public void SetTitle(string newTitle)
      {
         this.title = newTitle;
      }

      /// <summary>
      /// Set the note to a new note.
      /// </summary>
      /// <param name="newNote">The new note.</param>
      public void SetNote(string newNote)
      {
         this.note = newNote;
      }

      /// <summary>
      /// Creates a readable format version of a location model.
      /// </summary>
      /// <returns>The readable format location.</returns>
      public override string ToString()
      {
         return $"{this.Title} at Lat:{this.Latitude}, Lon:{this.Longitude}, Alt:{this.Altitude}";
      }
   }
}