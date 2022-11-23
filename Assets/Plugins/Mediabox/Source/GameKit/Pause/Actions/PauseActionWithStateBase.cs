namespace Mediabox.GameKit.Pause.Actions
{
    public abstract class PauseActionWithStateBase<TPauseState> : IPauseAction
    {
        public void OnPause(PauseState pauseState)
        {
            var type = GetType();
            if (pauseState.TrySetNewPauseState(type))
            {
                pauseState.pauseStatesPerType[GetType()] = Pause();
            }
        }

        public void OnUnpause(PauseState pauseState)
        {
            var type = GetType();
            if (pauseState.TryUnsetLastPauseState(type))
            {
                var state = (TPauseState) pauseState.pauseStatesPerType[GetType()];
                pauseState.pauseStatesPerType.Remove(GetType());
                Unpause(state);
            }
        }

        public void OnReset(PauseState pauseState)
        {
            if (pauseState == null)
            {
                Reset(default, false);
            }
            else
            {
                var state = (TPauseState) pauseState.pauseStatesPerType[GetType()];
                pauseState.pauseStatesPerType.Remove(GetType());
                Reset(state, true);
            }
        }

        protected abstract TPauseState Pause();
        protected abstract void Unpause(TPauseState stateBeforePause);
        protected abstract void Reset(TPauseState pauseState, bool hasPauseState);
    }
}