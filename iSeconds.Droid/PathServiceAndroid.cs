using System;
using iSeconds.Domain;
using Java.IO;
using Android.OS;
using Android.Content;

namespace iSeconds.Droid
{
	internal class MemoryUtils
	{
		public static bool ExternalMemoryAvailable() 
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

		public static bool IsExternalMemoryHigherFreeSpace() {
			if (MemoryUtils.ExternalMemoryAvailable())
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
		private string compilationPath;
      private bool pathsGood = false;

		public PathServiceAndroid()
      {
         appPath = string.Empty;
         
         if (MemoryUtils.ExternalMemoryAvailable()) {
				appPath = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.Path, "Yester.Droid");
            pathsGood = true;
         }

			System.Console.WriteLine("External Memory Available: " + pathsGood.ToString());
			mediaPath = System.IO.Path.Combine(appPath, "Videos");
         dbPath = System.IO.Path.Combine(appPath, "Db"); 
			compilationPath = System.IO.Path.Combine(appPath, "Compilations"); 

         if (pathsGood)
            createPaths();
      }

      public bool IsGood()
      {
         return pathsGood;
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
			return System.IO.Path.Combine(dbPath, "Yester.db3");
      }

		public string GetFFMpegDbPath()
		{
			return System.IO.Path.Combine(dbPath, "Yester.FFMpeg.db3");
		}

		public string GetCompilationPath()
		{
			return compilationPath;
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
			createPath(compilationPath);
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

