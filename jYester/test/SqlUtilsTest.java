import java.util.Calendar;
import java.util.Date;
import java.util.GregorianCalendar;

import iSeconds.Domain.SqlUtils;

import org.junit.Before;
import org.junit.BeforeClass;
import org.junit.Test;
import org.junit.runner.RunWith;

@RunWith(JUnit4.class)
public class SqlUtilsTest {
	
	private boolean assertDate(Date date, int year, int month, int day) {
		Calendar calendar = new GregorianCalendar();
		calendar.setTime(date);
		assertEquals(year, calendar.get(Calendar.YEAR));
		assertEquals(month, calendar.get(Calendar.MONTH));
		assertEquals(day, calendar.get(Calendar.DAY_OF_MONTH));
	}
	
	@Test
	public void parseDate() 
	{
		Date date= SqlUtils.parseDate("2013-06-05 00:00 PM");
		assertDate(date, 2013, 6, 5);
		
		date= SqlUtils.parseDate("2013-12-30 00:00 PM");
		assertDate(date, 2013, 12, 30);
		
		date= SqlUtils.parseDate("2014-07-31 00:00 PM");
		assertDate(date, 2014, 7, 31);
	}
}
