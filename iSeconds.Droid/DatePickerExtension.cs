using System;
using Android.Widget;

namespace iSeconds.Droid
{
   public static class DatePickerExtension
   {
      public static DatePicker SetDateTime(this DatePicker value, DateTime dateTime)
      {
			value.UpdateDate(dateTime.Year, dateTime.Month-1, dateTime.Day);
         return value;
      }
   }
}

