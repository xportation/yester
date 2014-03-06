using System;
using SQLite;

namespace iSeconds.Domain
{
   public class ServiceCompilation : IModel
   {
      public const string Idle= "Idle";
      public const string Error= "Error";
      public const string Completed= "Completed";
      public const string Compiling= "Compiling";

      public ServiceCompilation()
      {
         this.Id = 0;
         this.Status = Idle;
      }

      [PrimaryKey, AutoIncrement]
      public int Id { get; set; }
      
      public string Path { get; set; }

      public string Status { get; set; }
   }
}

