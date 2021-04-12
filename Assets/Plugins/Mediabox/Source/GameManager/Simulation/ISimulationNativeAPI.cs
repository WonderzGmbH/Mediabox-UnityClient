using Mediabox.API;

namespace Mediabox.GameManager.Simulation {
	public interface ISimulationNativeAPI : INativeAPI {
		/// <summary>
		/// AutoSimulate will automatically handle all required events to get the Game for the passed bundleName running.
		/// </summary>
		/// <param name="bundleName">The currently selected bundleName</param>
		void AutoSimulate(string bundleName);
		/// <summary>
		/// Stops the currently running Game, if one is running.
		/// </summary>
		void StopSimulation();
		/// <summary>
		/// OnGUI needs to be called within Unity's OnGUI-Event.
		/// It will Render an interactable GUI for this Simulator.
		/// </summary>
		/// <param name="bundleName">Pass the currently selected Bundle Name into this function.</param>
		void OnGUI(string bundleName);
		/// <summary>
		/// Returns all Game Definitions that are installed and can be passed into OnGUI or AutoSimulate.
		/// </summary>
		string[] AllAvailableGameDefinitions { get; }
	}
}