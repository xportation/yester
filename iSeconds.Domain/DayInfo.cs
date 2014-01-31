
using System;
using System.Collections.Generic;
using SQLite;
using System.Diagnostics;
using System.IO;

namespace iSeconds.Domain
{
	public class DayInfo : IModel
	{
      private IRepository repository = null;

		public DayInfo (DateTime date, int timelineId)
		{
			this.Date = date;
			this.TimelineId = timelineId;
			this.DefaultVideoId = -1;
		}

		public DayInfo()
		{
		}

      public void AddVideo(string url)
      {
      	Debug.Assert(repository != null); // you should bind a repository with SetRepository() method

         // TODO: ver se isso basta.. salvamos o dia apenas se ele tiver video
         this.repository.SaveItem(this);

			MediaInfo media = new MediaInfo(this.Id, url, DateTime.Now.TimeOfDay);
         this.repository.SaveItem(media);

			this.DefaultVideoId = media.Id;

			// TODO: ugly, salvando duas vezes. na primeira vez temos que salvar para associar um Id a esse dia (usado no MediaInfo depois)
			// na segunda vez temos que persistir o ChoosedMediaId que sera o id associado ao MediaInfo quando salvo no banco (dependencia ciclica..)
			this.repository.SaveItem(this); 
      }

		public void DeleteVideo(string url)
		{
			Debug.Assert(repository != null); // you should bind a repository with SetRepository() method

			MediaInfo media = this.repository.GetMediaByPath (url);

			int deleteMediaId = media.Id;

			this.repository.DeleteMedia (media);

			try {
				File.Delete(url);
				File.Delete(media.GetThumbnailPath());
			} catch (Exception) {
			}

			if (deleteMediaId == this.DefaultVideoId)
				chooseNewDefaultVideo ();
		}

		private void chooseNewDefaultVideo()
		{
			IList<MediaInfo> medias = GetVideos ();

			// se ainda houver videos no dia, o default video sera o primeiro retornado
			if (medias.Count != 0)
				SetDefaultVideo (medias [0].Id);
			else // nao havendo videos obviamente nao havera default
				SetDefaultVideo (-1);
		}

		public void SetDefaultVideo (int mediaId)
		{
			this.DefaultVideoId = mediaId;
			this.repository.SaveItem(this);
		}

		public IList<MediaInfo> GetVideos()
		{
		   return this.repository.GetMediasForDay(this);
		}

		public int GetVideoCount ()
		{
			return GetVideos().Count;
		}

		public MediaInfo GetMediaByPath (string videopath)
		{
			return this.repository.GetMediaByPath(videopath);
		}

		public string GetDefaultThumbnail ()
		{
			return DefaultVideoId == -1 ? "" : this.repository.GetMediaById(this.DefaultVideoId).GetThumbnailPath();
		}

		public string GetDefaultVideoPath ()
		{
			return DefaultVideoId == -1 ? "" : this.repository.GetMediaById(this.DefaultVideoId).Path;
		}

		public MediaInfo GetDefaultVideo()
		{
			return DefaultVideoId == -1 ? null : this.repository.GetMediaById(this.DefaultVideoId);
		}

      public void SetRepository(IRepository repository)
      {
      	this.repository = repository;
		}

		#region db
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public int TimelineId { get; set; }
		public DateTime Date { get; set; }
		public int DefaultVideoId { get; set; }
		#endregion

        
    }

}

