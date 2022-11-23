using Mediabox.GameKit.Pause.Actions;

namespace Mediabox.GameKit.Pause
{
    public interface IPauseSynchronizationService
    {
        void AddPauseAction(IPauseAction pauseAction);
        void RemovePauseAction(IPauseAction pauseAction);
        /// <summary>
        /// Set to `true` if there is more than one pause is active currently.
        /// </summary>
        bool IsPaused { get; }
        /// <summary>
        /// Pauses the game.
        /// </summary>
        /// <returns>A <see cref="PauseHandle"/> used for un-pausing the Game again.</returns>
        PauseHandle Pause();
        /// <summary>
        /// Undoes the changes applied by the given Pause Handle.
        /// </summary>
        /// <param name="handle">The handle that you received when invoking <see cref="Pause"/></param>
        void Unpause(PauseHandle handle);
        /// <summary>
        /// This resets all configuration and invalidates all PauseHandles.
        /// </summary>
        void Reset();
    }
}