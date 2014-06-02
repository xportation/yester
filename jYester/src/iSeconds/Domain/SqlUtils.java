package iSeconds.Domain;

import java.util.Date;
import java.util.GregorianCalendar;

public class SqlUtils {
	
	// convert o DateTime para o formato do sqlite
	public static String formatToSqliteDate(Date date) {
		String w = "" + (1900 + date.getYear()) + "-" + prependZero(date.getMonth()) + "-"
				+ prependZero(date.getDay());
		return w;
	}
	
	// temos que colocar 0 na frente quando o mes ou o dia sao menores que 10
	// ex: 2013-1-1 tem que virar 2013-01-01
	private static String prependZero(int value) {
		String valueAsString = "" + value;
		if (valueAsString.length() == 1)
			valueAsString = "0" + valueAsString;

		return valueAsString;
	}
	
	public static Date parseDate(String dateAsStr) throws Exception {
		
		int space = dateAsStr.indexOf(" "); // isso eh para tratar o formato de data do db antigo (2013-08-08 00:00:00).. temos que remover o horário
		if (space != 0)
			dateAsStr = dateAsStr.substring(0, space);
		
		String[] s = dateAsStr.split("-");
		if (s.length != 3)
			throw new Exception("wrong date -> " + dateAsStr);
		
		int year = Integer.parseInt(s[0]);
		int month = Integer.parseInt(s[1]);
		int day = Integer.parseInt(s[2]);
		
		Date date = new GregorianCalendar(year, month, day).getTime();
		return date;
	}

}
