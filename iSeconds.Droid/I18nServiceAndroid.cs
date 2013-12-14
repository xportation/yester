using System;
using iSeconds.Domain;
using Android.Content;

namespace iSeconds.Droid
{
	public class I18nServiceAndroid : I18nService
	{
		private Context context = null;

		public I18nServiceAndroid(Context context)
		{
			this.context = context;
		}

		#region I18nService implementation

		public string Msg(string identifier)
		{
			if (identifier == "Default Timeline")
				return context.Resources.GetString(Resource.String.raw_default_timeline);
			else if (identifier == "This is a default timeline")
				return context.Resources.GetString(Resource.String.raw_this_is_a_default_timeline);
			else if (identifier == "Edit Timeline")
				return context.Resources.GetString(Resource.String.raw_edit_timeline);
			else if (identifier == "Set as current")
				return context.Resources.GetString(Resource.String.raw_set_as_current);
			else if (identifier == "Delete")
				return context.Resources.GetString(Resource.String.raw_delete);
			else if (identifier == "Cancel")
				return context.Resources.GetString(Resource.String.raw_cancel);
			else if (identifier == "Are you sure? This operation cannot be undone.")
				return context.Resources.GetString(Resource.String.raw_are_you_sure_this_operation_cannot_be_undone);
			else if (identifier == "Set as default")
				return context.Resources.GetString(Resource.String.raw_set_as_default);
			else if (identifier == "Delete video")
				return context.Resources.GetString(Resource.String.raw_delete_video);

			return "*"+identifier+"*";
		}

		#endregion
	}
}
