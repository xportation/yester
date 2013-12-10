using System;
using iSeconds.Domain;
using Java.IO;
using Android.OS;

namespace iSeconds.Droid
{
	internal class MemoryUtils
	{
		private static bool externalMemoryAvailable() 
		{
			string state = Android.OS.Environment.ExternalStorageState;
			return Android.OS.Environment.MediaMounted == state;
		}

		private static long availableMemorySize(File path) {
			StatFs stat = new StatFs(path.Path);
			long blockSize = stat.BlockSize;
			long availableBlocks = stat.AvailableBlocks;
			return availableBlocks * blockSize;
		}

		public static bool IsExternalMemoryBestChoice() {
			if (MemoryUtils.externalMemoryAvailable())
				return availableMemorySize (Android.OS.Environment.ExternalStorageDirectory) > 
					availableMemorySize (Android.OS.Environment.DataDirectory);

			return false;
		}
	}

   public class PathServiceAndroid : IPathService
   {
      private string appPath;
      private string mediaPath;
      private string dbPath; 

      public PathServiceAndroid()
      {
			appPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "iSeconds");
			if (MemoryUtils.IsExternalMemoryBestChoice())
				appPath = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.Path, "iSeconds");

			mediaPath = System.IO.Path.Combine(appPath, "Videos");
			dbPath = System.IO.Path.Combine(appPath, "Db"); 

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

      void createPaths ()
      {
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

