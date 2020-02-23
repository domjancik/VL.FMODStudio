using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using FMOD;

namespace VL.FMODStudio
{
    public class System: IDisposable
    {
        //public IObservable<object> EventsChanged => _eventsChanged;
        
        private static System instance = null;

        private FMOD.Studio.System _system;
        //private Subject<string> _eventsChanged;

        public static System Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new System(Utilities.initStudioSystem());
                }

                return instance;
            }
        }

        private System(FMOD.Studio.System system)
        {
            _system = system;
            //_eventsChanged = new Subject<string>();
        }

        public void LoadBank(string path, bool loadSamples = true)
        {
            Console.WriteLine("Loading banks");

            FMOD.Studio.Bank bank;

            Utilities.checkResult(_system.loadBankFile(path, FMOD.Studio.LOAD_BANK_FLAGS.NORMAL, out bank));

            if (loadSamples) LoadBankSamples(bank);

            Notifications.Instance.EventsChanged.OnNext("loaded " + path);
            //_eventsChanged.OnNext("loaded " + path);
        }

        public void LoadBankSamples(FMOD.Studio.Bank bank)
        {
            Console.WriteLine("Loading sample data");
            Utilities.checkResult(bank.loadSampleData());
            _system.update();

            FMOD.Studio.LOADING_STATE lState;

            do
            {
                Utilities.checkResult(bank.getSampleLoadingState(out lState));
                Console.WriteLine(lState.ToString());
                Task.Delay(200).Wait();
            }
            while (lState != FMOD.Studio.LOADING_STATE.LOADED);
        }

        public void LoadBanks(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                LoadBank(path);
            }
        }

        public IEnumerable<string> ListEvents()
        {
            FMOD.Studio.Bank[] bankList;
            Utilities.checkResult(_system.getBankList(out bankList));

            LinkedList<string> paths = new LinkedList<string>();
            foreach (var bank in bankList)
            {
                FMOD.Studio.EventDescription[] evList;
                bank.getEventList(out evList);

                foreach (var ev in evList)
                {
                    string path;
                    ev.getPath(out path);
                    paths.AddLast(path);
                }
            }

            return paths;
        }

        public void EmitEvent(string eventPath)
        {
            FMOD.Studio.EventDescription ev;
            FMOD.Studio.EventInstance eventInstance;

            Utilities.checkResult(_system.getEvent(eventPath, out ev));
            Utilities.checkResult(ev.createInstance(out eventInstance));
            Utilities.checkResult(eventInstance.start());
        }

        public void EmitEvent(FMODEvent input) {
            EmitEvent(input.Value);
        }

        public void ListEventParameters()
        {
            // TODO implement listing and adjusting of event parameters
            throw new NotImplementedException();
            //FMOD.Studio.EventDescription ev;
            // ev.getParameterDescriptionCount();
        }

        public void Update()
        {
            Utilities.checkResult(_system.update());
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Console.WriteLine("FMODSystem: Disposing");

                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    instance = null;
                }

                _system.release();
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.

                
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~System()
        {
          // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
          Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
