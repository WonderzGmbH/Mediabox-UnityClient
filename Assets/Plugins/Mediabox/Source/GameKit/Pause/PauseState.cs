using System;
using System.Collections.Generic;

namespace Mediabox.GameKit.Pause
{
    public class PauseState : Dictionary<Type, object>
    {
        public Dictionary<Type, int> numberOfPausesPerType = new Dictionary<Type, int>();
        public Dictionary<Type, object> pauseStatesPerType = new Dictionary<Type, object>();

        public bool TrySetNewPauseState(Type type)
        {
            if (!numberOfPausesPerType.ContainsKey(type))
            {
                numberOfPausesPerType[type] = 1;
                return true;
            }

            numberOfPausesPerType[type]++;
            return numberOfPausesPerType[type] > 1;
        }
        
        public bool TryUnsetLastPauseState(Type type)
        {
            numberOfPausesPerType[type]--;
            return numberOfPausesPerType[type] == 0;
        }
    }
}