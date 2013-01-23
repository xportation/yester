namespace iSeconds.Domain
{
	public interface IMediaService
	{
		/// <summary>
		///	Take a picture
		/// </summary>
		/// <param name="pictureName"></param>
		/// <param name="pictureDirectory"></param>
		/// <returns></returns>
		string TakePicture(string pictureName, string pictureDirectory);

		/// <summary>
		///	Take a movie
		/// </summary>
		/// <param name="movieName"></param>
		/// <param name="movieDirectory"></param>
		/// <param name="timeSeconds"></param>
		/// <returns></returns>
		string TakeMovie(string movieName, string movieDirectory, int timeSeconds);

		/// <summary>
		///    Pick a picture
		/// </summary>
		/// <returns>the picture path</returns>
		string PickPicture();

		/// <summary>
		///    Pick a movie
		/// </summary>
		/// <returns>the movie path</returns>
		string PickMovie();
	}
}