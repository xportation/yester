using System;
using System.Collections.Generic;
using RestSharp;

namespace iSeconds.Droid
{
	public class Session
	{
      private string sessionKey;
      private Dictionary<string, int> videos= new Dictionary<string, int>();
//		private RestClient restClient = new RestClient ("http://thawing-lowlands-7118.herokuapp.com");
		private RestClient restClient = new RestClient ("http://localhost:8080");

		public Session(int id)
		{
			this.Id = id;
		}

		public int Id { get; set; }

		public delegate void UploadConcluded();
		public event UploadConcluded onUploadConcluded;

      public void StartConcatenation(IList<string> videosPath)
      {
         videos.Clear();
         foreach (string video in videosPath) {
            videos.Add(video, 0);
         }

         concat2();
      }

      private void concat()
      {
         var request = new RestRequest("begin", Method.GET);
         IRestResponse beginResponse = restClient.Execute(request);
         sessionKey = beginResponse.Content;

         RestRequest upload = new RestRequest("upload", Method.POST);
         upload.AddParameter("sessionKey", sessionKey);
         foreach (var video in videos) {
            upload.AddFile("video", video.Key);
         }

         var asyncHandler = restClient.ExecuteAsync(upload, response =>
         {
            if (response.ResponseStatus == ResponseStatus.Completed)
            {
					if (onUploadConcluded != null)
						onUploadConcluded();
            } else {
					if (onUploadConcluded != null)
						onUploadConcluded();
				}
         });
      }

      private void concat2()
      {
         var request = new RestRequest("begin", Method.GET);
         IRestResponse beginResponse = restClient.Execute(request);
         sessionKey = beginResponse.Content;

         foreach (var video in videos) {
            uploadFile(video.Key, sessionKey);
         }

         RestRequest endRequest = new RestRequest ("end", Method.POST);
         endRequest.AddParameter ("sessionKey", sessionKey);

         restClient.Execute (endRequest);
         /*byte[] rawbytes = restClient.DownloadData (endRequest);

         System.IO.FileStream _FileStream = 
            new System.IO.FileStream (result, System.IO.FileMode.Create,
                                      System.IO.FileAccess.Write);
         // Writes a block of bytes to this stream using data from
         // a byte array.
         _FileStream.Write (rawbytes, 0, rawbytes.Length);

         // close file stream
         _FileStream.Close ();

         return result;*/
      }

      private IRestResponse uploadFile(string path, string sessionKey)
      {
         RestRequest upload = new RestRequest ("upload", Method.POST);
         upload.AddParameter ("sessionKey", sessionKey);
         upload.AddFile ("videos", path);
         return restClient.Execute (upload);
      }
	}
}

