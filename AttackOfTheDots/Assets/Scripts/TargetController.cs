using UnityEngine;
using System.Collections;

public class TargetController : MonoBehaviour {

	float firing_cooldown_duration = 2.0f;

	//Return target to beyond the screen
	public void Hide(){
		gameObject.transform.position = new Vector3 (-5, 0, 0);
	}

	//Target scale progress
	public void Progress(float firing_cooldown){
		if (firing_cooldown < firing_cooldown_duration) {
			gameObject.transform.localScale = new Vector3(firing_cooldown/firing_cooldown_duration,firing_cooldown/firing_cooldown_duration,0);

		}
	}

}
