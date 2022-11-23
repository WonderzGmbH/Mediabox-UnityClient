using System;
using System.Collections.Generic;

namespace Mediabox.GameKit.Pause
{
    public class PauseState
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
            return numberOfPausesPerType[type] == 1;
        }
        
        public bool TryUnsetLastPauseState(Type type)
        {
            numberOfPausesPerType[type]--;
            return numberOfPausesPerType[type] == 0;
        }

        public bool TryResetPauseState(Type type)
        {
            if (!numberOfPausesPerType.TryGetValue(type, out var count))
            {
                return false;
            }

            if (count == 0) return false;
            numberOfPausesPerType[type] = 0;
            return true;
        }
    }
}