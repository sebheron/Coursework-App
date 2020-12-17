using SQLite;

namespace CSC20038.Classes
{
   public class LocationModel
   {
      /// <summary>
      /// The ID of the stored location.
      /// </summary>
      [PrimaryKey, AutoIncrement]
      [Column("id")]
      public int ID { get; set; }

      /// <summary>
      /// The title of the stored location.
      /// </summary>
      [Column("title")]
      public string Title { get; set; }

      /// <summary>
      /// The note for the stored location.
      /// </summary>
      [Column("note")]
      public string Note { get; set; }

      /// <summary>
      /// The longitude of the location.
      /// </summary>
      [Column("longitude")]
      public double Longitude { get; set; }

      /// <summary>
      /// The latitude of the location.
      /// </summary>
      [Column("latitude")]
      public double Latitude { get; set; }

      /// <summary>
      /// The altitude of the location.
      /// </summary>
      [Column("altitude")]
      public double Altitude { get; set; }

      /// <summary>
      /// Creates a readable format version of a location model.
      /// </summary>
      /// <returns>The readable format location.</returns>
      public override string ToString()
      {
         return $"{this.Title} at Latitude:{this.Latitude}, Longitude:{this.Longitude}, Altitude:{this.Altitude}";
      }
   }
}