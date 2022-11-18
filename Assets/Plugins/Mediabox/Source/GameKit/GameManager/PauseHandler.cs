using UnityEngine;

namespace Mediabox.GameKit.GameManager {
    public class PauseHandler {
        float timeScaleBeforePause = 1;
        float volumeBeforePause = 1;
        bool isPaused = false;

        public delegate void OnPause(bool pausing);
        public event OnPause onPause;
        
        
        public void Pause() {
            
            
            if (this.isPaused){
                Debug.LogWarning("Pause() has been called multiple times. To avoid unintended behavior, it is recommended to ensure calling Unpause() before Pausing again.");
            }
            
            this.isPaused = true;
            this.volumeBeforePause = AudioListener.volume;
            AudioListener.volume = 0f;
            
            if (onPause != null){
                if (onPause.GetInvocationList().Length > 0){
                    onPause.Invoke(true);
                    return;
                }
            }
            
            this.timeScaleBeforePause = Time.timeScale;
            Time.timeScale = 0f;
           
        }

        public void Unpause() {

            if (!this.isPaused){
                Debug.LogWarning("Unpause() has been called multiple times. To avoid unintended behavior, it is recommended to ensure calling Pause() before Unpausing again.");
            }
            
            this.isPaused = false;
            AudioListener.volume = this.volumeBeforePause;
            if (onPause != null){
                if (onPause.GetInvocationList().Length > 0){
                    onPause.Invoke(false);
                    return;
                }
            }
            Time.timeScale = this.timeScaleBeforePause;
        }

        public void ResetPauseValues(){
            onPause = null;
            timeScaleBeforePause = 1;
            volumeBeforePause = 1;
        }
        
    }
}