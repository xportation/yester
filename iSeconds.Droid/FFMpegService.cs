using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;
using Java.Lang;
using Java.IO;
using Android.Util;
using Android.Content.Res;
using System.Threading;

 // tive que adicionar essa dependencia por causa de um util para criar o thumbnail.. se for necessário podemos remover
using iSeconds.Domain;


namespace iSeconds.Droid
{
	/// <summary>
	/// FF MPEG service.
	/// A service that acts as a wrapper for ffmpeg native executable.
	/// Is is WIP and at least for now, the only operation supported is the concat.
	/// </summary>
	[Service]
	[IntentFilter (new string[]{ "com.broditech.iseconds.FFMpegService" })]
	public class FFMpegService : IntentService
	{
		public const string ConcatFinishedIntent = "ConcatFinishedIntent";

		protected override void OnHandleIntent (Intent intent)
		{
			string command = intent.Extras.GetString ("ffmpeg.command");

			if (command == "concat") {
				string outputFilePath = intent.Extras.GetString ("ffmpeg.concat.output");
				IList<string> filesToConcat = intent.Extras.GetStringArrayList ("ffmpeg.concat.filelist");

				// this is very important because there is only one path on the entire system 
				// that applications are able to set 'execute' permissions and this is supposed to be it.
				// according to this http://stackoverflow.com/questions/5531289/copy-the-shared-preferences-xml-file-from-data-on-samsung-device-failed
				// this can vary, so... we need to test it and follow that guidelines if needed.
				basePathAbsolute = "/data/data/" +
				this.BaseContext.PackageName + "/shared_prefs";

				// this should be put at a more general place, for now it is ok to be here as this is the only function supported.
				LoadBinariesAndChangePermissions (basePathAbsolute);

				FFMpegConcat (filesToConcat, outputFilePath);
			}
		}

		void notifyEnd (string filename)
		{
			var stocksIntent = new Intent (ConcatFinishedIntent); 

			Bundle bundle = new Bundle ();	
			bundle.PutString ("ffmpeg.concat.result", filename);

			stocksIntent.PutExtras (bundle);
			SendOrderedBroadcast (stocksIntent, null);
		}

		private readonly string CHMOD_755_COMMAND = "/system/bin/chmod 755";
		/// <summary>
		/// This is the list of ffmpeg binaries, the ffmpeg executable 
		/// and the shared libbraries that it depends on.
		/// </summary>
		private readonly string[] FFMPEG_BINARIES = { 
			"ffmpeg", 
			"libavcodec-55.so",
			"libavfilter-3.so", 
			"libavformat-55.so",
			"libavutil-52.so",
			"libswresample-0.so",
			"libswscale-2.so"
		};
		private string basePathAbsolute;

		void FFMpegConcat (IList<string> filesToConcat, string outputFilePath)
		{
			// create a filelist (to make ffmpeg executable happy)
			// as stated in the docs this is the most general concatenation option (the recommended one).
			string fileListPath = Path.Combine (basePathAbsolute, "filelist.txt");

			// each file's line should have the form "file 'file_path'"
			using (var streamWriter = new StreamWriter (fileListPath, false)) {
				foreach (string file in filesToConcat) {
					streamWriter.WriteLine ("file '" + file + "'");
				}
			}

			// we have to set LD_LIBRARY_PATH so tht linux can find the shared libraries that ffmpeg depends on, 
			// otherwise it won't find it even if on the same path. I had to find it out the hard way :(
			string[] envp = { "LD_LIBRARY_PATH=" + basePathAbsolute + ":$LD_LIBRARY_PATH" };

			// here we create the command line... quite self explanatory
			var command = "." +
			               Path.Combine (basePathAbsolute, "ffmpeg") +
			               " -f concat -i " +
			               fileListPath +
			               " -c copy " +
			               outputFilePath;

			// execute it as a native command with the new environment setting LD_LIBRARY_PATH.
			executeNativeCommand (command, envp);

			// we don't need it anymore.
			System.IO.File.Delete (fileListPath);

			saveThumbnail (outputFilePath);

			//service.notifyEnd (outputFilePath);
			notifyEnd (outputFilePath);
		}

		void saveThumbnail (string outputFilePath)
		{
			string extension = ".png";
			string directory = Path.GetDirectoryName(outputFilePath);
			string thumbnailPath = Path.GetFileNameWithoutExtension(outputFilePath);
			AndroidMediaUtils.SaveVideoThumbnail(Path.Combine(directory, thumbnailPath + extension), outputFilePath);
		}


		/// <summary>
		/// ffmpeg binaries are copies from the assets to a folder in the OS where it lets us execute it.
		/// This functions does all the tricks.
		/// </summary>
		/// <param name="basePath">The path where we are able to set x permissions to binaries.</param>
		void LoadBinariesAndChangePermissions (string basePath)
		{
			var baseDir = new Java.IO.File (basePath);

			if (!baseDir.Exists ()) {
				baseDir.Mkdirs ();
			}

			foreach (string file in FFMPEG_BINARIES) {
				LoadBinaryAndChangePermissions (basePath, file);
			}
		}

		void LoadBinaryAndChangePermissions (string basePath, string file)
		{
			string filename = Path.Combine (basePath, file);

			// otimizaçao... nao precisamos fazer todo o processo se o arquivo ja foi extraido
			if (new Java.IO.File (filename).Exists ())
				return;

			var stream = Assets.Open (file);

			using (var streamWriter = new StreamWriter (filename, false)) {
				ReadWriteStream (stream, streamWriter.BaseStream);
			}

			ChangeFilePermissions (filename);
		}

		private void ReadWriteStream (Stream readStream, Stream writeStream)
		{
			int Length = 256;
			byte[] buffer = new byte[Length];
			int bytesRead = readStream.Read (buffer, 0, Length);
			// write the required bytes
			while (bytesRead > 0) {
				writeStream.Write (buffer, 0, bytesRead);
				bytesRead = readStream.Read (buffer, 0, Length);
			}
			readStream.Close ();
			writeStream.Close ();
		}

		void ChangeFilePermissions (string filename)
		{
			var command = CHMOD_755_COMMAND + " " + filename;

			executeNativeCommand (command);
		}

		/// <summary>
		/// Executes a native command line with an optional environemnt setting.
		/// </summary>
		/// <returns>The OS return value.</returns>
		/// <param name="command">the command line</param>
		/// <param name="envp">the environment variables</param>
		static int executeNativeCommand (string command, string[] envp = null)
		{
			try {
				// Executes the command.
				Java.Lang.Process process = Runtime.GetRuntime ().Exec (command, envp);

				// Reads stdout.
				// NOTE: You can write to stdin of the command using
				//       process.getOutputStream().
				BufferedReader reader = new BufferedReader (new InputStreamReader (process.InputStream));

				int bytesRead;
				char[] buffer = new char[4096];
				while ((bytesRead = reader.Read (buffer)) > 0) {
					string line = new string (buffer, 0, bytesRead);
					Log.Debug ("cmd line input stream: ", line);
				}
				reader.Close ();

				// Waits for the command to finish.
				process.WaitFor ();

				int exitValue = process.ExitValue ();
				Log.Debug ("exitvalue", exitValue.ToString ());

				return exitValue;
			} catch (Java.IO.IOException e) {
				throw new RuntimeException (e);
			} catch (InterruptedException e) {
				throw new RuntimeException (e);
			}
		}
	}
}

