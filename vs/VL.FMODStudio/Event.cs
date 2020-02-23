using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VL.FMODStudio
{
    public class Event: IDisposable
    {
        public bool Ready { get; private set; }

        private FMOD.Studio.EventDescription _desc;
        private FMOD.Studio.EventInstance _instance;
        private string _path;

        public Event(FMODEvent ev)
        {
            Ready = false;
            _path = ev.Value;
        }

        public void Update()
        {
            if (!Ready && System.Instance.Ready) load();
        }
        
        public void Start()
        {
            checkReady();

            Utilities.checkResult(_instance.start());
        }

        public void Stop()
        {
            checkReady();

            Utilities.checkResult(_instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT));
        }

        public void SetParameter(string id, float value)
        {
            checkReady();

            Utilities.checkResult(_instance.setParameterByName(id, value));
        }

        public IEnumerable<string> ListParameters()
        {
            checkReady();

            int count;
            Utilities.checkResult(_desc.getParameterDescriptionCount(out count));

            var descList = new LinkedList<FMOD.Studio.PARAMETER_DESCRIPTION>();
            var nameList = new LinkedList<string>();

            for (int i = 0; i < count; i++)
            {
                FMOD.Studio.PARAMETER_DESCRIPTION pDesc;
                Utilities.checkResult(_desc.getParameterDescriptionByIndex(i, out pDesc));

                if (pDesc.type == FMOD.Studio.PARAMETER_TYPE.GAME_CONTROLLED)
                    nameList.AddLast(pDesc.name);
            }

            return nameList;
        }

        private void load()
        {
            _desc = System.Instance.GetEventDescription(_path);
            Utilities.checkResult(_desc.createInstance(out _instance));

            Ready = true;
        }

        private void checkReady()
        {
            if (!Ready) throw new Exceptions.NotLoadedException();
        }

        public void Dispose()
        {
            if (Ready)
            {
                Stop();
                _instance.release();
            }
        }
    }
}
