using System;
using FMOD;

namespace VL.FMODStudio
{
    static class Utilities
    {
        public static void checkResult(RESULT result)
        {
            if (result != RESULT.OK)
            {
                var error = String.Format("FMOD error! ({0}) {1}\n", result, Error.String(result));
                Console.WriteLine(error);
                throw new Exception(error);
            }
        }

        public static FMOD.Studio.System initStudioSystem(bool enableLiveUpdate = false)
        {
            var initFlags = enableLiveUpdate ? FMOD.Studio.INITFLAGS.LIVEUPDATE : FMOD.Studio.INITFLAGS.NORMAL;

            Console.WriteLine("FMODSystem: Initializing");
            // Create the Studio System object.
            FMOD.Studio.System system;
            checkResult(FMOD.Studio.System.create(out system));
            // Initialize FMOD Studio, which will also initialize FMOD Core
            checkResult(system.initialize(512, initFlags, FMOD.INITFLAGS.NORMAL, IntPtr.Zero));

            return system;
        }
    }
}
