using FMOD;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VL.Lib.Collections;

namespace VL.FMODStudio
{
    public class System: IDisposable
    {
        //public IObservable<object> EventsChanged => _eventsChanged;

        public bool Ready { get; private set; }

        private static System instance = null;

        private FMOD.Studio.System _system;
        private OUTPUTTYPE _outputType;

        //private Subject<string> _eventsChanged;

        public static System Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new System(Utilities.initStudioSystem(false, OUTPUTTYPE.AUTODETECT));
                }

                return instance;
            }
            set
            {
                instance= null;
                instance = value;
            }
        }

        public System(bool enableLiveUpdate = false, OUTPUTTYPE outputType = OUTPUTTYPE.AUTODETECT, int driverIndex = 0, int sampleRate = 48000, SPEAKERMODE speakerMode = SPEAKERMODE.DEFAULT, int numRawSpeakers = 0, uint bufferLength = 1024, int numBuffers = 4)
        {
            Ready = false;
            _system = Utilities.initStudioSystem(enableLiveUpdate, outputType, driverIndex, sampleRate, speakerMode, numRawSpeakers, bufferLength, numBuffers);
        }

        private System(FMOD.Studio.System system)
        {
            Ready = false;
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

            Ready = true;
            Notifications.Instance.BanksLoaded.OnNext("");
        }

        public Spread<String> GetDriverInfo()
        {
            FMOD.System coreSystem;
            _system.getCoreSystem(out coreSystem);
            int numDrivers;
            int speakerModeChannels, systemRate;
            SPEAKERMODE speakerMode;
            Guid guid;
            string name;
            coreSystem.getNumDrivers(out numDrivers);
            SpreadBuilder<string> sb = new SpreadBuilder<string>();
            for (int i=0; i < numDrivers; i++)
            {
                coreSystem.getDriverInfo(i, out name, 255, out guid, out systemRate, out speakerMode, out speakerModeChannels);
                sb.Add(string.Format("{0} @ {1}, Channels: {2}, Mode: {3}", new object[] { name , systemRate , speakerModeChannels, speakerMode.ToString() }));
            }
            return sb.ToSpread();
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

        public FMOD.Studio.EventDescription GetEventDescription(string eventPath)
        {
            FMOD.Studio.EventDescription ev;
            Utilities.checkResult(_system.getEvent(eventPath, out ev));
            return ev;
        }
        public void EmitEvent(string eventPath)
        {
            FMOD.Studio.EventDescription ev;
            FMOD.Studio.EventInstance eventInstance;

            Utilities.checkResult(_system.getEvent(eventPath, out ev));
            Utilities.checkResult(ev.createInstance(out eventInstance));
            Utilities.checkResult(eventInstance.start());
            Utilities.checkResult(eventInstance.release());
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
