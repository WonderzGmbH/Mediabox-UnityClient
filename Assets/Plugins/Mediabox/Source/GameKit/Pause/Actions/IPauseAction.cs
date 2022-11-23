namespace Mediabox.GameKit.Pause.Actions
{
    public interface IPauseAction
    {
        void OnPause(PauseState pauseState);

        void OnUnpause(PauseState pauseState);

        /// <summary>
        /// Called when a `OnReset` of the PauseState is invoked.
        /// </summary>
        /// <param name="pauseState">May be `null`, if the App has not been in Pause.</param>
        void OnReset(PauseState pauseState);
    }
}