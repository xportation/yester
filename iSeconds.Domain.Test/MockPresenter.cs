using System;
using iSeconds.Domain.Framework;

namespace iSeconds.Domain.Test
{
	class MockPresenter : IPresenter
	{
		public bool wasShow = false;
		public bool wasClosed = false;
		public Args argsCalled = null;

		public void Show (Args args) 
		{
			argsCalled = args;
			wasShow = true;
		}
		public void Close ()
		{
			wasClosed= true;
		}
	}

}

