package iSeconds.Domain;

import java.util.Calendar;
import java.util.Date;
import java.util.GregorianCalendar;

public class ISecondsUtils {
	
	public static String stringifyDate(String prefix, Date dateTime) {
		Calendar calendar = new GregorianCalendar();
		calendar.setTime(dateTime);
		
		return String.format("%s__%d_%d_%d__%d_%d_%d"
				, prefix
				, calendar.get(Calendar.DAY_OF_MONTH)
				, calendar.get(Calendar.MONTH) + 1
				, calendar.get(Calendar.YEAR) 
				, calendar.get(Calendar.HOUR_OF_DAY)
				, calendar.get(Calendar.MINUTE)
				, calendar.get(Calendar.SECOND));
	}

}
