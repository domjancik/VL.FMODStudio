using System;
using VL.Lib.Collections;

namespace VL.FMODStudio
{
    [Serializable]
    public class FMODEvent: DynamicEnumBase<FMODEvent, FMODEventDefinition>
    {
        public FMODEvent(string value) : base(value) { }

        public static FMODEvent CreateDefault()
        {
            return CreateDefaultBase("Not Loaded");
        }
    }
}
