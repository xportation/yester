using System;
using iSeconds.Domain;

namespace iSeconds.Droid
{
   public class PathServiceAndroid : IPathService
   {
      private string appPath = Android.OS.Environment.ExternalStorageDirectory + "/iSeconds";
      private string mediaPath = Android.OS.Environment.ExternalStorageDirectory + "/iSeconds/Videos";
      private string dbPath = Android.OS.Environment.ExternalStorageDirectory + "/iSeconds/Db"; 

      public PathServiceAndroid()
      {
         createPaths();
      }

      public string GetApplicationPath ()
      {
         return appPath;
      }

      public string GetMediaPath ()
      {
         return mediaPath;
      }

      public string GetDbPath ()
      {
         return System.IO.Path.Combine(dbPath, "ISeconds.db3");
      }

      void createPath (string path)
      {
         if (!System.IO.Directory.Exists (path)) {
            System.IO.Directory.CreateDirectory (path);
         }
      }

      bool sdCardMounted()
      {
         string state = Android.OS.Environment.ExternalStorageState;
         return Android.OS.Environment.MediaMounted == state;
      }

      void createPaths ()
      {
         if (!sdCardMounted())
            throw new Exception("you need a mounted sdcard"); // TODO: mostrar isso pro usuario...

         createPath(appPath);
         createPath(mediaPath);
         createPath(dbPath);
      }

   }

   /*public static string DatabaseFilePath
   {
      get
      {
         // codigo copiado dos samples do xamarin.. serve para guardar o db no lugar correto de acordo com o SO
         //          #if SILVERLIGHT
         //          var path = "MwcDB.db3";
         //          #else
         
         //          #if __ANDROID__
         //string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
         string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
         
         
         //          #else
         //          // we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
         //          // (they don't want non-user-generated data in Documents)
         //          string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
         //          string libraryPath = Path.Combine (documentsPath, "../Library/");
         //          #endif
         var path = Path.Combine(libraryPath, "ISeconds.db3");
         //          #endif      
         return path;
      }
   }*/
}

