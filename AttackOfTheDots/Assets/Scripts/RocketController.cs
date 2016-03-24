using UnityEngine;
using System.Collections;

public class RocketController : MonoBehaviour {

	float rocket_flight_duration = 1.0f;

	Vector2 origin = new Vector2(0,-4);
	Vector2 target;

	//Return rocket to beyond the screen
	public void Hide(){
		gameObject.transform.position = new Vector3 (-5, 2, 0);
	}

	//Rocket orientation and target
	public void SetTarget(Vector2 point){
		target = point;

		float diff_x = target.x - origin.x;
		float diff_y = target.y - origin.y;

		float multiplier = Mathf.Sqrt (diff_x * diff_x + diff_y * diff_y);

		float angle = - Mathf.Rad2Deg * Mathf.Asin (diff_x/multiplier);

		this.gameObject.transform.eulerAngles = new Vector3(0,0,angle);
	}

	//Rocket flight progress
	public void Progress(float rocket_flight){
		if (rocket_flight < rocket_flight_duration) {
			gameObject.transform.position = Vector2.Lerp(origin, target, rocket_flight/rocket_flight_duration);

		}
	}

}
