using UnityEngine;

namespace Mediabox.GameKit.Pause.Actions
{
    public class TimePauseActionWithState : PauseActionWithStateBase<float>
    {
        protected override float Pause()
        {
            Debug.Log("Pause Time");
            float current = Time.timeScale;
            Time.timeScale = 0f;
            return current;
        }

        protected override void Unpause(float stateBeforePause)
        {
            Debug.Log("Unpause Time");
            Time.timeScale = stateBeforePause;
        }

        protected override void Reset(float pauseState, bool hasPauseState)
        {
            Debug.Log("Reset Time");
            Time.timeScale = 1f;
        }

    }
}