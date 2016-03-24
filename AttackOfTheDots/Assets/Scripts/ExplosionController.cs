using UnityEngine;
using System.Collections;

public class ExplosionController : MonoBehaviour {

	Animator anim;
	AudioSource player;
	bool audio_enabled;

	//On Load
	void Start () {
		anim = gameObject.GetComponent<Animator>();
		player = gameObject.GetComponent<AudioSource>();
		audio_enabled = true;
	}

	//Triggered from the menu
	public void ToggleAudio(){
		audio_enabled = !audio_enabled;
	}

	//Return explosion to beyond the screen
	public void Hide(){
		gameObject.transform.position = new Vector3 (-5, -2, 0);
	}

	//Activate explosion animation & SFX
	public void Detonate(){
		anim.SetTrigger ("Detonate");
		if (audio_enabled) {
			player.Play ();
		}
	}

}
