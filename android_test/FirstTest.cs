using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace android_test
{
	[TestClass()]
	public class FirstTest
	{
		public FirstTest ()
		{
		}

		[TestMethod()]
		public void ATest()
		{
			Assert.Fail();
		}
	}
}

