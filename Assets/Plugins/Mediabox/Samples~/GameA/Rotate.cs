using UnityEngine;

namespace Mediabox.Samples.GameA {
	public class Rotate : MonoBehaviour {
		public Vector3 rotation;

		public void Update() {
			this.transform.Rotate(this.rotation * Time.deltaTime);
		}
	}
}