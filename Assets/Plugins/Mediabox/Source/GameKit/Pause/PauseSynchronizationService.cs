using System.Collections.Generic;
using Mediabox.GameKit.Pause.Actions;
using UnityEngine;

namespace Mediabox.GameKit.Pause {
    public class PauseSynchronizationService : IPauseSynchronizationService
    {
        private readonly HashSet<PauseHandle> pauseHandles = new HashSet<PauseHandle>();
        private readonly HashSet<IPauseAction> pauseActions = new HashSet<IPauseAction>();
        private PauseState pauseState;

        public void AddPauseAction(IPauseAction pauseAction)
        {
            if (this.pauseActions.Add(pauseAction) && this.IsPaused)
            {
                pauseAction.OnPause(this.pauseState);
            }
        }

        public void RemovePauseAction(IPauseAction pauseAction)
        {
            if (this.pauseActions.Remove(pauseAction) && this.IsPaused)
            {
                pauseAction.OnUnpause(this.pauseState);
            }
        }

        public bool IsPaused => pauseHandles.Count > 0;

        public PauseHandle Pause()
        {
            Debug.Log("Pause Registered");
            bool wasPaused = IsPaused;
            var pauseHandle = new PauseHandle();
            pauseHandles.Add(pauseHandle);

            if (!wasPaused)
            {
                Debug.Log("New Pause Started");
                this.pauseState = new PauseState();
                foreach (var pauseAction in this.pauseActions)
                {
                    pauseAction.OnPause(pauseState);
                }
            }
            
            return pauseHandle;
        }

        public void Unpause(PauseHandle pauseHandle) {
            Debug.Log("Unpause Registered");
            if (!this.pauseHandles.Remove(pauseHandle))
            {
                Debug.LogWarning("The PauseHandle is invalid. This usually happens, if some class invoked `OnReset()` before or if you try using the same `PauseHandle` for un-pausing twice.");
                return;
            }
            if (!this.IsPaused) {
                Debug.Log("New Pause Ended");
                UnpauseInternal();
            }
        }

        void UnpauseInternal()
        {
            foreach (var pauseAction in this.pauseActions)
            {
                pauseAction.OnUnpause(this.pauseState);
            }
        }

        public void Reset()
        {
            Debug.Log("Reset Pause Registered");
            foreach (var pauseAction in this.pauseActions)
            {
                pauseAction.OnReset(this.pauseState);
            }

            this.pauseState = null;
            this.pauseHandles.Clear();
        }
    }
}