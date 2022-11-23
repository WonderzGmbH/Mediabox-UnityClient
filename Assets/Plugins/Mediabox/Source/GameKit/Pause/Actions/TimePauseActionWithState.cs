using UnityEngine;

namespace Mediabox.GameKit.Pause.Actions
{
    public class TimePauseActionWithState : PauseActionWithStateBase<float>
    {
        protected override float Pause()
        {
            float current = Time.timeScale;
            Time.timeScale = 0f;
            return current;
        }

        protected override void Unpause(float stateBeforePause)
        {
            Time.timeScale = stateBeforePause;
        }

        protected override void Reset(float pauseState, bool hasPauseState)
        {
            Time.timeScale = 1f;
        }

    }
}