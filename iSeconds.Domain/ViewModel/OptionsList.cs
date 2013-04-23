using System;
using System.Windows.Input;
using System.Collections.Generic;

namespace iSeconds.Domain
{
	public class OptionsList
	{
		public class OptionsEntry
      {
         public OptionsEntry(string name, Action callback)
         {
            this.Name = name;
            this.Callback = callback;
         }

         public string Name { get; set; }
         public Action Callback { get; set; }
      }

      private List<OptionsEntry> entries = new List<OptionsEntry>();

      public List<OptionsEntry> OptionsEntries
      {
         get { return entries; }
      }

      public ICommand DayEntryClicked
      {
         get
         {
            return new Command((object arg) =>
					{
						int index = (int) arg;
						entries[index].Callback.Invoke();
					});
         }
      }

		public void AddEntry(OptionsEntry entry)
		{
			entries.Add(entry);
		}
	}
}
