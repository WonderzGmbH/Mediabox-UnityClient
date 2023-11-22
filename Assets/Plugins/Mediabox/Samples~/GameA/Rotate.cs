using UnityEngine;
using UnityEngine.EventSystems;

namespace Mediabox.Samples.GameA {
	public class Rotate : MonoBehaviour, IPointerClickHandler {
		public Vector3 rotation;

		public void Update() {
			this.transform.Rotate(this.rotation * Time.deltaTime);
		}


		public void OnPointerClick(PointerEventData eventData)
		{
			var game = FindObjectOfType<Game>();
			game.API.ReportNewUserScore(game.Score+1);
		}
	}
}