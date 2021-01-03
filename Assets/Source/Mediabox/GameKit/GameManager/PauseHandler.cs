using UnityEngine;

namespace Mediabox.GameKit.GameManager {
    public class PauseHandler {
        float timeScaleBeforePause;
        float volumeBeforePause;
        
        public void Pause() {
            this.timeScaleBeforePause = Time.timeScale;
            Time.timeScale = 0f;
            this.volumeBeforePause = AudioListener.volume;
            AudioListener.volume = 0f;
        }

        public void Unpause() {
            Time.timeScale = this.timeScaleBeforePause;
            AudioListener.volume = this.volumeBeforePause;
        }
        
    }
}