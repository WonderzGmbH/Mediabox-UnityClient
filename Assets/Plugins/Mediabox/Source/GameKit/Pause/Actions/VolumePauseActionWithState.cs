using UnityEngine;

namespace Mediabox.GameKit.Pause.Actions
{
    public class VolumePauseActionWithState : PauseActionWithStateBase<float>
    {
        protected override float Pause()
        {
            float current = AudioListener.volume;
            AudioListener.volume = 0f;
            return current;
        }

        protected override void Unpause(float stateBeforePause)
        {
            AudioListener.volume = stateBeforePause;
        }

        protected override void Reset(float pauseState, bool hasPauseState)
        {
            AudioListener.volume = 1f;
        }
    }
}