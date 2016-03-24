using UnityEngine;
using System.Collections;

public class GrinController : MonoBehaviour {

	Animator anim;

	void Start () {
		anim = GetComponent<Animator> ();
	}

	//Start grin animation
	public void Appear(){
		anim.SetTrigger ("Appear");
	}

	//Reset grin
	public void Disappear(){
		anim.SetTrigger ("Disappear");
	}

	//Return grin to beyond the screen
	public void Hide(){
		gameObject.transform.position = new Vector3 (-5, 3.5f, 0);
	}

}
