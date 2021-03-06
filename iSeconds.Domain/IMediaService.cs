﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iSeconds.Domain
{
	public interface IMediaService
	{
		event EventHandler OnVideoRecorded;
		event EventHandler OnThumbnailSaved;

		void TakeVideo(DateTime date, Action<string> resultAction);

		void SaveVideoThumbnail(string thumbnailPath, string videoPath);

		void ConcatMovies(string compilationPath, DateTime startDate, DateTime endDate, int timelineId, bool onlyDefaultMovies);

		void ShareVideo(string filename, string dialogTitle);

		// used by Camcorder to commit or revert recording to application
		void CommitVideo (string videoPath);
		void RevertVideo ();
	}
}
