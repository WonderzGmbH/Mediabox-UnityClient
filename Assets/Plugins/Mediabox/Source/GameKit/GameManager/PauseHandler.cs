using UnityEngine;

namespace Mediabox.GameKit.GameManager {
    public class PauseHandler {
        float timeScaleBeforePause;
        float volumeBeforePause;
        bool isPaused;
        
        public void Pause() {
            if (!this.isPaused) {
                this.timeScaleBeforePause = Time.timeScale;
                this.volumeBeforePause = AudioListener.volume;
                this.isPaused = true;
            } else {
                Debug.LogWarning("Pause() has been called multiple times. To avoid unintended behavior, it is recommended to ensure calling Unpause() before Pausing again.");
            }
            Time.timeScale = 0f;
            AudioListener.volume = 0f;
        }

        public void Unpause() {
            if (this.isPaused) {
                Time.timeScale = this.timeScaleBeforePause;
                AudioListener.volume = this.volumeBeforePause;
                this.isPaused = false;
            } else {
                Debug.LogWarning("Unpause() has been called multiple times. To avoid unintended behavior, it is recommended to ensure calling Pause() before Unpausing again.");
            }
        }
        
    }
}