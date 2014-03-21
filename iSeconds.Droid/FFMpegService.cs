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
using Java.Lang;
using Android.Util;
using Android.Content.Res;
using System.Threading;
using System.Text.RegularExpressions;
using System.Globalization;
using SQLite;
using iSeconds.Domain;


namespace iSeconds.Droid
{
    /// <summary>
    /// FF MPEG service.
    /// A service that acts as a wrapper for ffmpeg native executable.
    /// Is is WIP and at least for now, the only operation supported is the concat.
    /// </summary>
    [Service]
    [IntentFilter(new string[] { "com.broditech.iseconds.FFMpegService" })]
    public class FFMpegService : IntentService
    {
        public const string ConcatFinishedIntent = "ConcatFinishedIntent";

        protected override void OnHandleIntent(Intent intent)
        {
            string command = intent.Extras.GetString("ffmpeg.command");

            if (command == "concat")
            {
                string outputFilePath = intent.Extras.GetString("ffmpeg.concat.output");
                IList<string> filesToConcat = intent.Extras.GetStringArrayList("ffmpeg.concat.filelist");
                IList<string> subtitles = intent.Extras.GetStringArrayList("ffmpeg.concat.subtitles");

                Java.IO.File baseFilePath = this.GetDir("shared_prefs", FileCreationMode.Private);
                basePathAbsolute = baseFilePath.Path;

                // we have to set LD_LIBRARY_PATH so that linux can find the shared libraries that ffmpeg depends on, 
                // otherwise it won't find it even if on the same path. I had to find it out the hard way :(
                envp = new string[] {
               "LD_LIBRARY_PATH=" + basePathAbsolute + ":$LD_LIBRARY_PATH"
            };

                System.IO.FileStream fileLock = createFilelock(outputFilePath);

                // this should be put at a more general place, for now it is ok to be here as this is the only function supported.
                LoadBinariesAndChangePermissions(basePathAbsolute);

                FFMpegConcat(filesToConcat, subtitles, outputFilePath);

                fileLock.Close();
                System.IO.File.Delete(fileLock.Name);
            }
        }

        System.IO.FileStream createFilelock(string outputFilePath)
        {
            System.IO.FileStream fileLock = new System.IO.FileStream(outputFilePath + ".lock", System.IO.FileMode.CreateNew,
                    System.IO.FileAccess.Write, System.IO.FileShare.None);
            return fileLock;
        }

        void notifyEnd(string filename, bool errors)
        {
            System.Console.WriteLine("Compilation ends - result: " + filename + " - error: " + errors.ToString());
            var stocksIntent = new Intent(ConcatFinishedIntent);

            Bundle bundle = new Bundle();
            bundle.PutString("ffmpeg.concat.result", filename);
            bundle.PutBoolean("ffmpeg.concat.result.errors", errors);

            stocksIntent.PutExtras(bundle);
            SendOrderedBroadcast(stocksIntent, null);
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
			"libexpat.so.1",
			"libfreetype.so.6",
			"libswresample-0.so",
			"libswscale-2.so",
			"libxml2.so.2"
		};

        private struct VideoAttributes
        {
            public int maxWidth;
            public int minWidth;
            public int maxHeight;
            public int minHeight;
            public double maxFps;
            public double maxBitrate;
            public int minAudioFreqHz;
            public double minAudioBitRateKbs;
        }

        private string[] envp;
        private string basePathAbsolute;

        /// <summary>
        /// Concatenate movies
        /// </summary>
        /// <returns><c>true</c>, if no erros, <c>false</c> otherwise.</returns>
        /// <param name="filesToConcat">Files to concatenate.</param>
        /// <param name="subtitles">Subtitles.</param>
        /// <param name="outputFilePath">Output file path.</param>
        void FFMpegConcat(IList<string> filesToConcat, IList<string> subtitles, string outputFilePath)
        {
            IList<VideoFileInformation> videoFiles = getVideoFileInformationForListOfPaths(filesToConcat);
            if (videoFiles.Count == 0)
                return;

            var tempDir = System.IO.Path.Combine(basePathAbsolute, "temp");
            if (!System.IO.Directory.Exists(tempDir))
                System.IO.Directory.CreateDirectory(tempDir);

            var rotatedTempDir = System.IO.Path.Combine(tempDir, "rotated");
            if (!System.IO.Directory.Exists(rotatedTempDir))
                System.IO.Directory.CreateDirectory(rotatedTempDir);

            int subtitleIndex = 1;
            long accumulatedTimeMs = 0L;
            string previousTimeLabel = "00:00:00,000";

            // create a filelist (to make ffmpeg executable happy)
            // as stated in the docs this is the most general concatenation option, using the demuxer.
            string fileListPath = System.IO.Path.Combine(basePathAbsolute, "filelist.txt");
            string subtitlePath = System.IO.Path.Combine(basePathAbsolute, "subtitle.srt");

            // for the files that needs padding due to multiple orientations/sizes.
            // we'll accumulate the names here in order to remove them at the end of the process.
            IList<string> temporaryFiles = new List<string>();

            bool errors = false;
            VideoAttributes videoAttributes;

            using (System.IO.StreamWriter fileListWriter = new System.IO.StreamWriter(fileListPath, false),
                   subtitleWriter = new System.IO.StreamWriter(subtitlePath, false))
            {

                // check if we need to fix any rotations
                for (int i = 0; i < videoFiles.Count; i++)
                {
                    if (videoFiles[i].Rotation != 0)
                    {

                        var filePath = System.IO.Path.Combine(rotatedTempDir, System.IO.Path.GetFileName(videoFiles[i].Path));

                        var filter = "";

                        switch (videoFiles[i].Rotation)
                        {
                            case 90:
                                filter = "transpose=1";
                                break;
                            case 180:
                                filter = "transpose=2,transpose=2";
                                break;
                            case 270:
                                filter = "transpose=2";
                                break;
                        }

                        var cmd = string.Format(".{0} -y -i {1} -vf " +
                            "{2} -metadata:s:v:0 rotate=0 " +
                            "-strict -2 -c:v mpeg4 -b:v {3}k -r {4} -c:a aac -strict experimental -ar {5} -b:a {6}k -sn {7}",
                            System.IO.Path.Combine(basePathAbsolute, "ffmpeg"),
                            videoFiles[i].Path,
                            filter,
                            (int)videoFiles[i].BitRate,
                            getFpsAsString(videoFiles[i].Fps),
                            videoFiles[i].AudioFrequencyInHz(),
                            (int)videoFiles[i].AudioBitRateInKBits(),
                            filePath);

                        System.Console.WriteLine(cmd);

                        NativeCommandResult result = executeNativeCommand(cmd, envp);
                        if (result.exitValue != 0)
                        {
                            //TODO: [ronald] what to do with errors?... eg.: out of memory
                            //for now we are just returning that an error occurred
                            errors = true;
                            break;
                        }

                        temporaryFiles.Add(filePath);

                        videoFiles[i] = getVideoFileInformationForPath(filePath);
                    }
                }

                videoAttributes = GetVideoAttributes(videoFiles);

                if (!errors)
                {

                    for (int i = 0; i < videoFiles.Count; i++)
                    {

                        var file = videoFiles[i];
                        var subtitle = subtitles[i];
                        var filePath = file.Path;

                        if ((videoAttributes.maxWidth != file.Width) || (videoAttributes.maxHeight != file.Height))
                        {

                            filePath = System.IO.Path.Combine(tempDir, System.IO.Path.GetFileName(filePath));

                            var cmd = GenerateCommandScaleVideoWithBlackPaddings(videoAttributes, file, filePath);

                            System.Console.WriteLine(cmd);

                            NativeCommandResult result = executeNativeCommand(cmd, envp);
                            if (result.exitValue != 0)
                            {
                                //TODO: [ronald] what to do with errors?... eg.: out of memory
                                //for now we are just returning that an error occurred
                                errors = true;
                                break;
                            }

                            temporaryFiles.Add(filePath);
                        }

                        // each file's line should have the form "file 'file_path'"
                        fileListWriter.WriteLine("file '" + filePath + "'");

                        accumulatedTimeMs += (long)file.Duration.TotalMilliseconds;
                        var newTimeLabel = timeInMsToSrtTimeFormat(accumulatedTimeMs);
                        writeSubtitleEntry(subtitleWriter, subtitleIndex, previousTimeLabel, subtitle, newTimeLabel);
                        previousTimeLabel = newTimeLabel;
                        subtitleIndex++;
                    }
                }
            }

            if (!errors)
            {
                // -loglevel debug
                var command = string.Format(".{0} -y -f concat -i {1} -vf subtitles='{2}' -strict -2 -c:v mpeg4 " +
                                             "-b:v {3}k -r {4} -c:a aac -strict experimental -ar {5} -b:a {6}k -sn {7}",
                                             System.IO.Path.Combine(basePathAbsolute, "ffmpeg"),
                                             fileListPath,
                                             subtitlePath,
                                             (int)videoAttributes.maxBitrate,
                                             getFpsAsString(videoAttributes.maxFps),
                                             videoAttributes.minAudioFreqHz,
                                             (int)videoAttributes.minAudioBitRateKbs,
                                             outputFilePath);

                System.Console.WriteLine(command);

                NativeCommandResult res = executeNativeCommand(command, envp);
                if (res.exitValue != 0)
                {
                    errors = true;
                    //TODO: [ronald] what to do with errors?... eg.: out of memory
                    //for now we are just returning that an error occurred
                }
            }

            // we don't need any of this anymore.
            System.IO.File.Delete(fileListPath);
            System.IO.File.Delete(subtitlePath);
            foreach (var file in temporaryFiles)
                System.IO.File.Delete(file);

            saveThumbnail(outputFilePath);
            notifyEnd(outputFilePath, errors);
        }

        string getFpsAsString(double fps)
        {
            string fpsString = fps.ToString();
            return fpsString.Replace(',', '.');
        }

        void writeSubtitleEntry(
            System.IO.StreamWriter subtitleWriter,
            int subtitleIndex,
            string previousTimeLabel,
            string subtitle,
            string newTimeLabel
        )
        {
            subtitleWriter.WriteLine(subtitleIndex);
            subtitleWriter.WriteLine(previousTimeLabel + " --> " + newTimeLabel);
            subtitleWriter.WriteLine(subtitle);
            subtitleWriter.WriteLine();
        }

        string GenerateCommandScaleVideoWithBlackPaddings(
            VideoAttributes videoAttributes,
            VideoFileInformation file,
            string filePath
        )
        {
            //-loglevel debug
            return string.Format(".{0} -y -i {1} -vf " +
                "scale=iw*sar*min({2}/(iw*sar)\\,{3}/ih):ih*min({4}/(iw*sar)\\,{5}/ih)," +
                "pad={6}:{7}:(ow-iw)/2:(oh-ih)/2:black " +
                "-strict -2 -c:v mpeg4 -b:v {8}k -r {9} -c:a aac -strict experimental -ar {10} -b:a {11}k -sn {12}",
                                 System.IO.Path.Combine(basePathAbsolute, "ffmpeg"),
                                 file.Path,
                                 videoAttributes.minWidth,
                                 videoAttributes.minHeight,
                                 videoAttributes.minWidth,
                                 videoAttributes.minHeight,
                                 videoAttributes.minWidth,
                                 videoAttributes.minHeight,
                                 (int)videoAttributes.maxBitrate,
                                 getFpsAsString(videoAttributes.maxFps),
                                 videoAttributes.minAudioFreqHz,
                                 (int)videoAttributes.minAudioBitRateKbs,
                                 filePath);
        }

        VideoAttributes GetVideoAttributes(IList<VideoFileInformation> videoFiles)
        {
            VideoAttributes videoAttributes = new VideoAttributes();

            foreach (var videoFile in videoFiles)
            {

                if (videoFile.BitRate > videoAttributes.maxBitrate)
                    videoAttributes.maxBitrate = videoFile.BitRate;

                if (videoFile.Width > videoAttributes.maxWidth)
                    videoAttributes.maxWidth = videoFile.Width;

                if (videoFile.Width < videoAttributes.minWidth || videoAttributes.minWidth == 0)
                    videoAttributes.minWidth = videoFile.Width;

                if (videoFile.Height > videoAttributes.maxHeight)
                    videoAttributes.maxHeight = videoFile.Height;

                if (videoFile.Height < videoAttributes.minHeight || videoAttributes.minHeight == 0)
                    videoAttributes.minHeight = videoFile.Height;

                if (videoFile.Fps > videoAttributes.maxFps)
                    videoAttributes.maxFps = videoFile.Fps;

                var audioFreqHz = videoFile.AudioFrequencyInHz();
                if (audioFreqHz < videoAttributes.minAudioFreqHz || videoAttributes.minAudioFreqHz == 0)
                    videoAttributes.minAudioFreqHz = audioFreqHz;

                var audioBitRateInKbs = videoFile.AudioBitRateInKBits();
                if (audioBitRateInKbs < videoAttributes.minAudioBitRateKbs || videoAttributes.minAudioBitRateKbs < 0.1)
                    videoAttributes.minAudioBitRateKbs = audioBitRateInKbs;
            }

            return videoAttributes;
        }

        string timeInMsToSrtTimeFormat(long milis)
        {
            var span = TimeSpan.FromMilliseconds(milis);

            return string.Format("{0}:{1}:{2},{3}",
                                 span.Hours.ToString("00"),
                                 span.Minutes.ToString("00"),
                                 span.Seconds.ToString("00"),
                                 span.Milliseconds.ToString("000"));
        }

        void saveThumbnail(string outputFilePath)
        {
            string extension = ".png";
            string directory = System.IO.Path.GetDirectoryName(outputFilePath);
            string thumbnailPath = System.IO.Path.GetFileNameWithoutExtension(outputFilePath);
            AndroidMediaUtils.SaveVideoThumbnail(System.IO.Path.Combine(directory, thumbnailPath + extension), outputFilePath);
        }


        /// <summary>
        /// ffmpeg binaries are copies from the assets to a folder in the OS where it lets us execute it.
        /// This functions does all the tricks.
        /// </summary>
        /// <param name="basePath">The path where we are able to set x permissions to binaries.</param>
        void LoadBinariesAndChangePermissions(string basePath)
        {
            var baseDir = new Java.IO.File(basePath);

            if (!baseDir.Exists())
            {
                baseDir.Mkdirs();
            }

            foreach (string file in FFMPEG_BINARIES)
            {
                LoadBinaryAndChangePermissions(basePath, file);
            }
        }

        void LoadBinaryAndChangePermissions(string basePath, string file)
        {
            string filename = System.IO.Path.Combine(basePath, file);

            // otimizacao... nao precisamos fazer todo o processo se o arquivo ja foi extraido
            if (new Java.IO.File(filename).Exists())
                return;

            var stream = Assets.Open(file);

            using (var streamWriter = new System.IO.StreamWriter(filename, false))
            {
                ReadWriteStream(stream, streamWriter.BaseStream);
            }

            ChangeFilePermissions(filename);
        }

        private void ReadWriteStream(System.IO.Stream readStream, System.IO.Stream writeStream)
        {
            int Length = 256;
            byte[] buffer = new byte[Length];
            int bytesRead = readStream.Read(buffer, 0, Length);
            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, Length);
            }
            readStream.Close();
            writeStream.Close();
        }

        void ChangeFilePermissions(string filename)
        {
            var command = CHMOD_755_COMMAND + " " + filename;

            executeNativeCommand(command);
        }

        struct NativeCommandResult
        {
            public int exitValue;
            public string stderr;
            public string stdout;
        }

        /// <summary>
        /// Executes a native command line with an optional environemnt setting.
        /// </summary>
        /// <returns>The OS return value.</returns>
        /// <param name="command">the command line</param>
        /// <param name="envp">the environment variables</param>
        static NativeCommandResult executeNativeCommand(string command, string[] envp = null)
        {
            try
            {
                // Executes the command.
                Java.Lang.Process process = Runtime.GetRuntime().Exec(command, envp);

                NativeCommandResult result = new NativeCommandResult();

                result.stderr = "";
                result.stdout = "";

                // Reads stderr.
                Java.IO.BufferedReader reader = new Java.IO.BufferedReader(new Java.IO.InputStreamReader(process.ErrorStream));
                result.stderr = readOutputStream(reader);

                // Reads stdout.
                reader = new Java.IO.BufferedReader(new Java.IO.InputStreamReader(process.InputStream));
                result.stdout = readOutputStream(reader);

                // Waits for the command to finish.
                process.WaitFor();
                result.exitValue = process.ExitValue();
                Log.Debug("exitvalue", result.exitValue.ToString());

                return result;
            }
            catch (Java.IO.IOException e)
            {
                throw new RuntimeException(e);
            }
            catch (InterruptedException e)
            {
                throw new RuntimeException(e);
            }
        }

        static string readOutputStream(Java.IO.BufferedReader reader)
        {
            int bytesRead;
            string output = "";

            char[] buffer = new char[4096];

            while ((bytesRead = reader.Read(buffer)) > 0)
            {
                string line = new string(buffer, 0, bytesRead);
                System.Console.WriteLine(line);
                output += line;
            }
            reader.Close();

            return output;
        }

        List<VideoFileInformation> getVideoFileInformationForListOfPaths(IList<string> files)
        {
            List<VideoFileInformation> videoFiles = new List<VideoFileInformation>();

            foreach (var file in files)
                videoFiles.Add(getVideoFileInformationForPath(file));

            return videoFiles;
        }

        VideoFileInformation getVideoFileInformationForPath(string file)
        {
            // a REALLY helpful source of information for dealing with ffmpeg command line output.
            // http://jasonjano.wordpress.com/2010/02/09/a-simple-c-wrapper-for-ffmpeg/

            var videoInfo = new VideoFileInformation();

            videoInfo.Path = file;

            var command = string.Format(".{0} -y -i {1}",
                                        System.IO.Path.Combine(basePathAbsolute, "ffmpeg"),
                                        file);

            System.Console.WriteLine(command);

            // execute it as a native command with the new environment setting LD_LIBRARY_PATH.
            NativeCommandResult result = executeNativeCommand(command, envp);

            //get duration
            Regex re = new Regex("[D|d]uration:.((\\d|:|\\.)*)");
            Match m = re.Match(result.stderr);
            if (m.Success)
            {
                System.Console.WriteLine(m.Groups[1].Value);

                string duration = m.Groups[1].Value;
                string[] timepieces = duration.Split(new char[] { ':', '.' });
                if (timepieces.Length == 4)
                    videoInfo.Duration = new TimeSpan(0,
                                                  Convert.ToInt16(timepieces[0]),
                                                  Convert.ToInt16(timepieces[1]),
                                                  Convert.ToInt16(timepieces[2]),
                                                  Convert.ToInt16(timepieces[3]) * 10);
            }

            //get audio bit rate
            re = new Regex("[B|b]itrate:.((\\d|:)*)");
            m = re.Match(result.stderr);
            double kb = 0.0;
            if (m.Success)
            {
                System.Console.WriteLine(m.Groups[1].Value);
                kb = double.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
            }
            videoInfo.BitRate = kb;

            //get the audio format
            re = new Regex("[A|a]udio:.*");
            m = re.Match(result.stderr);
            if (m.Success)
                videoInfo.AudioFormat = m.Value;

            //get the video format
            re = new Regex("[V|v]ideo:.*");
            m = re.Match(result.stderr);
            if (m.Success)
                videoInfo.VideoFormat = m.Value;

            //get the video format
            re = new Regex("(\\d{2,4})x(\\d{2,4})");
            m = re.Match(result.stderr);
            if (m.Success)
            {
                int width = 0; int height = 0;
                int.TryParse(m.Groups[1].Value, out width);
                int.TryParse(m.Groups[2].Value, out height);
                videoInfo.Width = width;
                videoInfo.Height = height;
            }

            //get the fps
            re = new Regex(",.([0-9]{0,2}\\.[0-9]{0,2}).fps,");
            m = re.Match(result.stderr);
            double fps = 0.0;
            if (m.Success)
            {
                System.Console.WriteLine(m.Groups[1].Value);
                fps = double.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
            }
            videoInfo.Fps = fps;

            //get video rotation (if any)
            re = new Regex("rotate\\s*:.(\\d{1,3})");
            m = re.Match(result.stderr);
            int rotation = 0;
            if (m.Success)
                rotation = int.Parse(m.Groups[1].Value);
            System.Console.WriteLine("Rotation: " + rotation);
            videoInfo.Rotation = rotation;

            return videoInfo;
        }
    }
}
