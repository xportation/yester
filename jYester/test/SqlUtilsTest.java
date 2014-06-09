import static org.junit.Assert.assertEquals;
import iSeconds.Domain.SqlUtils;

import java.util.Calendar;
import java.util.Date;
import java.util.GregorianCalendar;

import junit.framework.Assert;

import org.junit.Test;
import org.junit.runner.RunWith;
import org.junit.runners.JUnit4;

@RunWith(JUnit4.class)
public class SqlUtilsTest {
	
	private void assertDate(Date date, int year, int month, int day) {
		org.junit.Assert.assertNotNull(date);
		
		Calendar calendar = new GregorianCalendar();
		calendar.setTime(date);
		assertEquals(year, calendar.get(Calendar.YEAR));
		assertEquals(month-1, calendar.get(Calendar.MONTH));
		assertEquals(day, calendar.get(Calendar.DAY_OF_MONTH));
	}
	
	private Date getDate(String dateStr) {
		try {
			return SqlUtils.parseDate(dateStr);
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return null;
	}
	
	@Test
	public void parseDate() 
	{
		Date date= getDate("2013-06-05 00:00 PM");
		assertDate(date, 2013, 6, 5);
		
		date= getDate("2013-12-30 00:00 PM");
		assertDate(date, 2013, 12, 30);
		
		date= getDate("2014-07-31 00:00 PM");
		assertDate(date, 2014, 7, 31);
	}
}
