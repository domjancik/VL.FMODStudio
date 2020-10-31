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

        /*public static FMOD.Studio.System initStudioSystem(bool enableLiveUpdate = false, OUTPUTTYPE outputType = OUTPUTTYPE.AUTODETECT, int driverIndex = 0)
        {
            var initFlags = enableLiveUpdate ? FMOD.Studio.INITFLAGS.LIVEUPDATE : FMOD.Studio.INITFLAGS.NORMAL;

            Console.WriteLine("FMODSystem: Initializing");
            // Create the Studio System object.
            FMOD.Studio.System system;
            checkResult(FMOD.Studio.System.create(out system));
            FMOD.System coreSystem;
            system.getCoreSystem(out coreSystem);
            coreSystem.setOutput(outputType);
            coreSystem.setDriver(driverIndex);
            // Initialize FMOD Studio, which will also initialize FMOD Core
            checkResult(system.initialize(512, initFlags, FMOD.INITFLAGS.NORMAL, IntPtr.Zero));

            return system;
        }*/

        public static FMOD.Studio.System initStudioSystem(bool enableLiveUpdate = false, OUTPUTTYPE outputType = OUTPUTTYPE.AUTODETECT, int driverIndex = 0, int sampleRate = 48000, SPEAKERMODE speakerMode = SPEAKERMODE.DEFAULT, int numRawSpeakers = 0, uint bufferLength = 1024, int numBuffers = 4)
        {
            var initFlags = enableLiveUpdate ? FMOD.Studio.INITFLAGS.LIVEUPDATE : FMOD.Studio.INITFLAGS.NORMAL;

            Console.WriteLine("FMODSystem: Initializing");
            // Create the Studio System object.
            FMOD.Studio.System system;
            checkResult(FMOD.Studio.System.create(out system));
            FMOD.System coreSystem;
            system.getCoreSystem(out coreSystem);
            coreSystem.setOutput(outputType);
            coreSystem.setDriver(driverIndex);
            coreSystem.setSoftwareFormat(sampleRate, speakerMode, numRawSpeakers);
            coreSystem.setDSPBufferSize(bufferLength, numBuffers);
            // Initialize FMOD Studio, which will also initialize FMOD Core
            checkResult(system.initialize(512, initFlags, FMOD.INITFLAGS.NORMAL, IntPtr.Zero));

            return system;
        }
    }
}
