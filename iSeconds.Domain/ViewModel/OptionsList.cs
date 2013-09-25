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

      public ICommand EntryClicked
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

		public string[] ListNames()
		{
			string[] names= new string[OptionsEntries.Count];
			for (int i = 0; i < OptionsEntries.Count; i++)
				names[i] = OptionsEntries[i].Name;

			return names;
		}
	}
}
