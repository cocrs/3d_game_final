using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MTriggerType{Front,Back}
public class TriggerFB : MonoBehaviour
{
    public MParkingManager manager;

	// Is front trigger or back?
	public MTriggerType triggerType;

	// On parking triggers enter
	void OnTriggerEnter (Collider col)
	{
		
		// Is front trigger
		// if (triggerType == MTriggerType.Front) {
		// 	if (col.tag == "Front") {
		// 		manager.tFront = true;
		// 	}
		// } else {// Or back trigger?
		// 	if (col.tag == "Back") {
		// 		manager.tBack = true;
		// 	}

		// }
		
		
	}
	// On parking triggers exit
	void OnTriggerExit (Collider col)
	{
		
		// if (triggerType == MTriggerType.Front) {
		// 	if (col.tag == "Front") {
		// 		manager.tFront = false;
		// 	}
		
		// } else {
		// 	if (col.tag == "Back") {
		// 		manager.tBack = false;
		// 	}

		// }
	}
}
