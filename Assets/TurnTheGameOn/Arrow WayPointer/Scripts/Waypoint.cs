namespace TurnTheGameOn.ArrowWaypointer
{
	using UnityEngine;		
	public class Waypoint : MonoBehaviour
	{
		public GameManager gameManager;
		public int radius;
		[HideInInspector] public WaypointController waypointController;
		public int waypointNumber;
		public int index;
		public int atCityId;

		void Update(){
			if (waypointController.player) {
				if(Vector3.Distance(transform.position, waypointController.player.position) < radius){
					// waypointController.ChangeTarget ();
				}
			}
		}

		void OnTriggerEnter (Collider col) {
			if(col.gameObject.tag == "Player"){
				gameManager.acceptQuest(index, atCityId);
				// waypointController.ChangeTarget ();
			}
		}

		#if UNITY_EDITOR
		void OnDrawGizmosSelected(){
			if (waypointController != null) waypointController.OnDrawGizmosSelected (radius);
		}
		#endif
	}
}