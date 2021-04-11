using Mediabox.API;

namespace Mediabox.GameManager.Simulation {
	public interface ISimulationNativeAPI : INativeAPI {
		void AutoSimulate(string bundleName);
		void StopSimulation();
		void OnGUI(string bundleName);
		string[] AllAvailableGameDefinitions { get; }
	}
}