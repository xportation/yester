using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iSeconds.Domain
{
	public interface IMediaService
	{
		void TakeVideo(DateTime date, Action<string> resultAction);

		void PlayVideo(string videoPath);

		void SaveVideoThumbnail(string thumbnailPath, string videoPath);

		void ConcatMovies(string compilationPath, DateTime startDate, DateTime endDate, int timelineId, bool onlyDefaultMovies);
	}
}
