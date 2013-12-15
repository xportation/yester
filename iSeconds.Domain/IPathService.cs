using System;

namespace iSeconds.Domain
{
   public interface IPathService
   {
      string GetApplicationPath();
      string GetMediaPath();
      string GetDbPath();
		string GetCompilationPath();
   }
}

