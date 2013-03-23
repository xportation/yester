using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iSeconds.Domain.Test
{
    public class MockMediaService : IMediaService
    {
        public void TakeVideo(DateTime date, Action<string> resultAction)
        {
        }

        public void PlayVideo(string videoPath)
        {
        }
    }
}
