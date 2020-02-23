using System;
using System.Linq;
using System.Collections.Generic;
using VL.Lib.Collections;

namespace VL.FMODStudio
{
    public class FMODEventDefinition : DynamicEnumDefinitionBase<FMODEventDefinition>
    {
        protected override IReadOnlyDictionary<string, object> GetEntries()
        {
            Dictionary<string, object> eventNames = new Dictionary<string, object>();

            IEnumerable<string> events;
            try
            {
                events = System.Instance.ListEvents();
            } catch
            {
                string[] empty = { "Error loading" };
                events = empty;
            }
            
            foreach (var ev in events)
            {
                // The event path may be null if .strings.bank is not loaded (yet)
                if (ev != null) eventNames[ev] = null;
            }

            return eventNames;
        }

        protected override IObservable<object> GetEntriesChangedObservable()
        {
            return Notifications.Instance.EventsChanged;
            //return System.Instance.EventsChanged;
        }
    }
}
