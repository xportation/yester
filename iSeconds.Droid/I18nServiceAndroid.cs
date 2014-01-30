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
			else if (identifier == "A compilation for timeline {0} from {1} to {2}")
				return context.Resources.GetString(Resource.String.raw_compilation_default_description);
			else if (identifier == "Your compilation is now being processed...")
				return context.Resources.GetString(Resource.String.raw_compilation_processing_message);
			else if (identifier == "Delete compilation")
				return context.Resources.GetString(Resource.String.raw_delete_compilation);
			else if (identifier == "Share")
				return context.Resources.GetString(Resource.String.raw_share);
			else if (identifier == "Edit compilation")
				return context.Resources.GetString(Resource.String.raw_edit_compilation);
			else if (identifier == "Share via:")
				return context.Resources.GetString(Resource.String.raw_share_via);
			else if (identifier == "Sunday")
				return context.Resources.GetString(Resource.String.raw_sunday);
			else if (identifier == "Monday")
				return context.Resources.GetString(Resource.String.raw_monday);
			else if (identifier == "Tuesday")
				return context.Resources.GetString(Resource.String.raw_tuesday);
			else if (identifier == "Wednesday")
				return context.Resources.GetString(Resource.String.raw_wednesday);
			else if (identifier == "Thursday")
				return context.Resources.GetString(Resource.String.raw_thursday);
			else if (identifier == "Friday")
				return context.Resources.GetString(Resource.String.raw_friday);
			else if (identifier == "Saturday")
				return context.Resources.GetString(Resource.String.raw_saturday);
			else if (identifier == "January")
				return context.Resources.GetString(Resource.String.raw_january);
			else if (identifier == "February")
				return context.Resources.GetString(Resource.String.raw_february);
			else if (identifier == "March")
				return context.Resources.GetString(Resource.String.raw_march);
			else if (identifier == "April")
				return context.Resources.GetString(Resource.String.raw_april);
			else if (identifier == "May")
				return context.Resources.GetString(Resource.String.raw_may);
			else if (identifier == "June")
				return context.Resources.GetString(Resource.String.raw_june);
			else if (identifier == "July")
				return context.Resources.GetString(Resource.String.raw_july);
			else if (identifier == "August")
				return context.Resources.GetString(Resource.String.raw_august);
			else if (identifier == "September")
				return context.Resources.GetString(Resource.String.raw_september);
			else if (identifier == "October")
				return context.Resources.GetString(Resource.String.raw_october);
			else if (identifier == "November")
				return context.Resources.GetString(Resource.String.raw_november);
			else if (identifier == "December")
				return context.Resources.GetString(Resource.String.raw_december);
			else if (identifier == "Play")
				return context.Resources.GetString(Resource.String.raw_play);

			return "*"+identifier+"*";
		}

		#endregion
	}
}

