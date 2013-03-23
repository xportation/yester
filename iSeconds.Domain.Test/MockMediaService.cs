using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iSeconds.Domain.Test
{
    public class MockMediaService : IMediaService
    {
        public bool takeVideoWasCalled = false;
        public bool playVideoWasCalled = false;

        public void TakeVideo(DateTime date, Action<string> resultAction)
        {
            this.takeVideoWasCalled = true;

            resultAction.Invoke("video.path");
        }

        public void PlayVideo(string videoPath)
        {
            this.playVideoWasCalled = true;
        }

        public void Reset()
        {
            takeVideoWasCalled = false;
            playVideoWasCalled = false;
        }
    }
}
