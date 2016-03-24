using UnityEngine;
using System.Collections;

public class DotController : MonoBehaviour {

	//Speed variables
	static float SLOW = 0.5f;
	static float NORMAL = 0.65f;
	static float FAST = 0.8f;

	//Sprites depending on dot type
	public Sprite sprite_yellow;
	public Sprite sprite_blue;
	public Sprite sprite_green;
	public Sprite sprite_brown;
	public Sprite sprite_purple;

	//Type variables
	float speed;
	bool predictable;
	bool armored;

	float direction_angle;

	bool paused;

	//Variables for unpredictable switching of direction
	float unpredictable_cooldown;
	float unpredictable_switch_count;

	//Position and set off
	void Activate(){

		transform.position = new Vector2 (Random.Range (-2.0f, 2.0f), 4.0f);
		direction_angle = Random.Range (-150.0f, -30.0f);

		if (!predictable) {
			unpredictable_cooldown = 0;
			unpredictable_switch_count = 0;
		}

	}

	void Update () {
		
		if (!paused) {
			
			if (!predictable) {
				unpredictable_cooldown += Time.deltaTime;
				if (unpredictable_cooldown > unpredictable_switch_count) {
					unpredictable_switch_count++;
					direction_angle = Random.Range (-150.0f, -30.0f);
				}
			}

			Vector3 diff = new Vector3 (Mathf.Cos (Mathf.Deg2Rad * direction_angle), Mathf.Sin (Mathf.Deg2Rad * direction_angle), 0);
			diff = Vector3.Lerp (Vector3.zero, diff, Time.deltaTime * speed);

			Vector3 pos = transform.position + diff;
			if (pos.x < -2.4f) {
				pos.x += Mathf.Abs (pos.x + 2.4f) * 2;
				direction_angle = -180.0f - direction_angle;
			} else if (pos.x > 2.4f) {
				pos.x -= Mathf.Abs (pos.x - 2.4f) * 2;
				direction_angle = -180.0f - direction_angle;
			}

			transform.position = pos;

		}

	}

	void Pause(){
		paused = true;
	}

	void Unpause(){
		paused = false;
	}

	//Hit by explosion
	public void Hit(){
		if (armored) {
			Yellow (false);
		} else {
			transform.position = new Vector3 (-10, -10, 0);
			gameObject.SetActive (false);
		}

	}

	//Init as Yellow dot
	public void Yellow(bool activate){
		gameObject.name = "Yellow";
		speed = SLOW;
		predictable = true;
		armored = false;
		GetComponent<SpriteRenderer> ().sprite = sprite_yellow;
		if (activate) {
			Activate ();
		}
	}

	//Init as Blue dot
	public void Blue(bool activate){
		gameObject.name = "Blue";
		speed = NORMAL;
		predictable = true;
		armored = false;
		GetComponent<SpriteRenderer> ().sprite = sprite_blue;
		if (activate) {
			Activate ();
		}
	}

	//Init as Green dot
	public void Green(bool activate){
		gameObject.name = "Green";
		speed = FAST;
		predictable = true;
		armored = false;
		GetComponent<SpriteRenderer> ().sprite = sprite_green;
		if (activate) {
			Activate ();
		}
	}

	//Init as Brown dot
	public void Brown(bool activate){
		gameObject.name = "Brown";
		speed = SLOW;
		predictable = true;
		armored = true;
		GetComponent<SpriteRenderer> ().sprite = sprite_brown;
		if (activate) {
			Activate ();
		}
	}

	//Init as Purple dot
	public void Purple(bool activate){
		gameObject.name = "Purple";
		speed = NORMAL;
		predictable = false;
		armored = false;
		GetComponent<SpriteRenderer> ().sprite = sprite_purple;
		if (activate) {
			Activate ();
		}
	}


}
