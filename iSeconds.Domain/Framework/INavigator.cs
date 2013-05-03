using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iSeconds.Domain.Framework
{
   public class Args
   {
      private Dictionary<string, string> values = new Dictionary<string, string> ();

      public void Put (string key, string value)
      {
         values [key] = value;
      }

      public string Get (string key)
      {
         return values [key];
      }

      public Dictionary<string, string> GetArgs ()
      {
         return values;
      }
   }

   public interface IPresenter
   {
      void Show (Args args);

      void Close ();
   }

   public class INavigator
   {
      public void NavigateTo (string uri, Args args)
      {
         if (!routes.ContainsKey(uri))
            throw new Exception("there isnt a route registered with this uri: " + uri);

         currentPresenter = routes [uri];
         currentPresenter.Show (args);
      }

      public void RegisterNavigation (string uri, IPresenter presenter)
      {
         routes [uri] = presenter;
      }

      public void NavigateBack ()
      {
         if (currentPresenter == null)
            throw new Exception("there isnt a current presenter");

         currentPresenter.Close();
      }

      private IPresenter currentPresenter = null;
      private Dictionary<string, IPresenter> routes = new Dictionary<string,IPresenter> ();
   }
}
