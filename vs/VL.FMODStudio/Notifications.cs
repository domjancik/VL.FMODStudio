using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace VL.FMODStudio
{
    class Notifications
    {
        private static Notifications instance = null;

        public static Notifications Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Notifications();
                }

                return instance;
            }
        }

        public Subject<string> EventsChanged;

        public Notifications()
        {
            EventsChanged = new Subject<string>();
        }
    }
}
