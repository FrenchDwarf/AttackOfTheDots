using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	//Audio elements
	public AudioClip music_jingle;
	public AudioClip music_gamewon;
	public AudioClip music_gameover;
	private AudioSource player;
	private bool sfx = true;
	private bool music = true;

	//Spawn weights
	private int spawn_weight_yellow = 4;
	private int spawn_weight_blue = 5;
	private int spawn_weight_green = 2;
	private int spawn_weight_brown = 2;
	private int spawn_weight_purple = 1;

	//Interacting GameObjects
	public GameObject enemy_object;
	public GameObject camera_object;
	public GameObject grin_object;
	public GameObject cannon_object;
	public GameObject target_object;
	public GameObject rocket_object;
	public GameObject explosion_object;
	private Camera camera_component;

	//Camera Animation variables (GameOver)
	private Vector3 focus_target;
	private bool focus_animation_active;
	private float focus_animation_timer;

	//Menu variables
	private float menu_position; //1 : shown, 0 : hidden
	private bool menu_active;
	private bool menu_transitioning;
	private float menu_transition_duration = 0.5f;

	//Game state variables
	private bool paused;
	private bool game_over;
	private bool game_won;

	//Reference variables
	private List<GameObject> enemy_objects;
	private Vector2 target;

	//Cannon-Target-Rocket state variables
	private bool is_firing = false;
	private bool rocket_fired = false;
	private float firing_cooldown_period = 3.0f;
	private float target_cooldown_period = 2.0f;
	private float firing_cooldown = 0;

	//Level variables
	private float level;
	private float score;
	private float timer;
	private float dots_released;
	private float dots_to_be_released;

	//GUI variables
	public Texture2D[] gui_textures;
	private GUIStyle guistyle_banner_text;

	//On Load
	void Start () {

		Screen.SetResolution (480, 800, true);

		player = GetComponent<AudioSource> ();
		player.PlayOneShot (music_jingle);

		camera_component = camera_object.GetComponent<Camera> ();
		focus_animation_active = false;

		level = 1;
		score = 0;
		timer = 0;
		dots_released = 0;
		dots_to_be_released = 10 + level * 2;

		enemy_objects = new List<GameObject> ();

		menu_position = 0;
		menu_active = false;

		ConstructGUIStyles ();

		paused = false;

	}

	//Create text styles
	void ConstructGUIStyles(){
		guistyle_banner_text = new GUIStyle();
		guistyle_banner_text.fontSize = Screen.height / 24;
		guistyle_banner_text.normal.textColor = Color.white;
		guistyle_banner_text.alignment = TextAnchor.MiddleCenter;
	}

	//Once per frame
	void Update () {
		if (paused) {

			if (game_over) {
				if (focus_animation_active) {
					focus_animation_timer += Time.deltaTime;

					if (focus_animation_timer < 1) {
						camera_object.transform.position = Vector3.Lerp (new Vector3 (0, 0, -10), focus_target, focus_animation_timer);
						camera_component.orthographicSize = Mathf.Lerp (4, 1, focus_animation_timer);
					} else {
						camera_object.transform.position = focus_target;
						camera_component.orthographicSize = 1;
						grin_object.transform.position = new Vector3 (focus_target.x, focus_target.y, 0);
						grin_object.SendMessage ("Appear");
						focus_animation_active = false;
					}
				}
			}

			if (menu_active) {
				if (menu_transitioning) {
					menu_position += Time.deltaTime / menu_transition_duration;

					if (menu_position >= 1) {
						menu_transitioning = false;
						menu_position = 1;
					}
				}
			} else {
				if (menu_transitioning) {
					menu_position -= Time.deltaTime / menu_transition_duration;

					if (menu_position <= 0) {
						menu_transitioning = false;
						menu_position = 0;
						if (!game_over && !game_won) {
							Unpause ();
						}
					}
				}
			}

		} else {
			timer += Time.deltaTime;

			if (dots_released < dots_to_be_released && timer > dots_released * 60 / dots_to_be_released) {
				ReleaseDot ();
			}

			if (is_firing) {
				ProgressCooldown ();
			}

			if (Input.GetButton ("Fire1")) {

				if (!is_firing) {

					Vector2 mousePos = camera_object.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);

					if (mousePos.y > -3.0f && mousePos.y < 3.5f && mousePos.x < 2.4f && mousePos.x > -2.4f) {
						target = mousePos;
						FireCannon();
					}

				}

			}

			if (score == dots_to_be_released) {
				Win ();
			}

		}

	}

	//State change (Pause)
	void Pause(){
		if (music) {
			player.Pause ();
		}

		paused = true;
		for (int i = 0; i < enemy_objects.Count; i++) {
			if (enemy_objects [i].activeSelf) {
				enemy_objects[i].SendMessage("Pause");
			}
		}
	}

	//State change (Unpause)
	void Unpause(){
		if (music) {
			if (player.time != 0) {
				player.PlayOneShot (music_jingle);
			} else {
				player.UnPause ();
			}
		}

		paused = false;

		for (int i = 0; i < enemy_objects.Count; i++) {
			if (enemy_objects [i].activeSelf) {
				enemy_objects[i].SendMessage("Unpause");
			}
		}
	}

	//Spawn a new dot
	void ReleaseDot(){

		dots_released++;
		enemy_objects.Add ((GameObject) Instantiate(enemy_object));

		string color;
		int range;
		if (level < 3) {
			range = spawn_weight_yellow;
		}else if (level < 5) {
			range = spawn_weight_yellow + spawn_weight_blue;
		}else if (level < 7) {
			range = spawn_weight_yellow + spawn_weight_blue + spawn_weight_green;
		}else if (level < 9) {
			range = spawn_weight_yellow + spawn_weight_blue + spawn_weight_green + spawn_weight_brown;
		}else{
			range = spawn_weight_yellow + spawn_weight_blue + spawn_weight_green + spawn_weight_brown + spawn_weight_purple;
		}

		int index = Random.Range (0, range-1);

		if (index < spawn_weight_yellow) {
			color = "Yellow";
		}else if (index < (spawn_weight_yellow + spawn_weight_blue)) {
			color = "Blue";
		}else if (index < (spawn_weight_yellow + spawn_weight_blue + spawn_weight_green)) {
			color = "Green";
		}else if (index < (spawn_weight_yellow + spawn_weight_blue + spawn_weight_green + spawn_weight_brown)) {
			color = "Brown";
		}else{
			color = "Purple";
		}

		enemy_objects [(int)dots_released - 1].SendMessage (color, true);

	}

	//Cannon-Target-Rocket animation
	void ProgressCooldown(){
		firing_cooldown += Time.deltaTime;

		if (!rocket_fired && firing_cooldown > target_cooldown_period) {
			rocket_fired = true;
		}

		if (firing_cooldown > firing_cooldown_period) {
			is_firing = false;
		}
		target_object.SendMessage ("Progress", firing_cooldown);
		if (rocket_fired) {
			rocket_object.SendMessage ("Progress", firing_cooldown - target_cooldown_period);
		}

		if (!is_firing) {
			target_object.SendMessage ("Hide");
			rocket_object.SendMessage ("Hide");
			Detonate ();
		}
	}

	//On click (if not already in use)
	void FireCannon(){
		is_firing = true;
		rocket_fired = false;
		firing_cooldown = 0;
		rocket_object.SendMessage ("SetTarget", target);
		target_object.SendMessage ("Progress", firing_cooldown);
		cannon_object.transform.eulerAngles = new Vector3 (0, 0, GetCannonAngle(target));
		target_object.transform.position = target;
	}

	//Cannon should face the right direction
	float GetCannonAngle(Vector2 point){
		float cannon_x = cannon_object.transform.position.x;
		float cannon_y = cannon_object.transform.position.y;

		float diff_x = point.x - cannon_x;
		float diff_y = point.y - cannon_y;

		float multiplier = Mathf.Sqrt (diff_x * diff_x + diff_y * diff_y);

		float angle = - Mathf.Rad2Deg * Mathf.Asin (diff_x/multiplier);

		return angle;
	}

	//Explosion effect - Effect on dots
	void Detonate(){
		explosion_object.transform.position = target;
		explosion_object.SendMessage ("Detonate");

		for (int i = 0; i < enemy_objects.Count; i++) {
			if (Vector2.Distance(enemy_objects[i].transform.position, target) < 1){

				if (enemy_objects [i].name != "Brown") {
					score++;
				}
				enemy_objects[i].SendMessage("Hit");

			}
		}

	}

	//When all dots have been destroyed
	void Win(){
		rocket_object.SendMessage ("Hide");
		target_object.SendMessage ("Hide");
		explosion_object.SendMessage ("Hide");
		game_won = true;
		Pause ();
		if (music) {
			player.Stop ();
			player.PlayOneShot (music_gamewon);
		}

	}

	//Game Over when a dot reaches the bottom
	void OnTriggerEnter2D(Collider2D other){
		rocket_object.SendMessage ("Hide");
		target_object.SendMessage ("Hide");
		explosion_object.SendMessage ("Hide");
		FocusCamera (other.gameObject.transform.position);
		if (music) {
			player.Stop ();
			player.PlayOneShot (music_gameover);
		}


	}

	//Setup Camera animation
	void FocusCamera (Vector2 point){

		focus_target = new Vector3 (point.x, point.y, -10);
		focus_animation_active = true;
		focus_animation_timer = 0;
		game_over = true;
		Pause ();

	}

	//GUI
	void OnGUI(){

		GUI_Constant ();

		if (game_over & !focus_animation_active) {
			GUI_GameOver ();
		}

		if (game_won & !focus_animation_active) {
			GUI_LevelCleared ();
		}

		if (paused) {
			GUI_Menu ();
		}

	}

	//Scoreboard
	void GUI_Constant(){

		GUI.backgroundColor = Color.clear;

		GUI.BeginGroup(new Rect(0,0,Screen.width,Screen.height/16));

		GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height/16), gui_textures[0]);
		GUI.DrawTexture(new Rect(Screen.width * 11 / 24,Screen.height / 160,Screen.width / 12,Screen.height / 20), gui_textures[1]);

		GUI.Label(new Rect(0,0,Screen.width / 2,Screen.height / 16), "LEVEL "+level.ToString("00"), guistyle_banner_text);
		GUI.Label(new Rect(Screen.width / 2,0,Screen.width / 2,Screen.height / 16), "SCORE "+score.ToString("00"), guistyle_banner_text);

		GUI.EndGroup();

		if (!menu_transitioning) {
			if (GUI.Button(new Rect(Screen.width * 11 / 24,Screen.height / 160,Screen.width / 12,Screen.height / 20), "")){
				menu_active = !menu_active;
				menu_transitioning = true;
				if (menu_active && !game_over && !game_won) {
					Pause();
				}

			}
		}

	}

	//GUI on GameOver
	void GUI_GameOver(){

		GUI.backgroundColor = Color.clear;

		GUI.BeginGroup(new Rect(0,0,Screen.width,Screen.height));

		GUI.DrawTexture(new Rect(Screen.width * 1 / 8,Screen.height * 5 / 16,Screen.width * 3 / 4,Screen.height * 3 / 8), gui_textures[8]);

		GUI.DrawTexture(new Rect(Screen.width * 7 / 24,Screen.height * 12 / 16,Screen.width * 5 / 12,Screen.height * 6 / 80), gui_textures[9]);

		GUI.EndGroup();

		//Reset
		if (!menu_active && !menu_transitioning) {
			if (GUI.Button(new Rect(Screen.width * 7 / 24,Screen.height * 12 / 16,Screen.width * 5 / 12,Screen.height * 6 / 80), "")){
				Reset();
			}
		}
	}

	//GUI on GameWon
	void GUI_LevelCleared(){

		GUI.backgroundColor = Color.clear;

		GUI.BeginGroup(new Rect(0,0,Screen.width,Screen.height));

		GUI.DrawTexture(new Rect(Screen.width * 1 / 8,Screen.height * 5 / 16,Screen.width * 3 / 4,Screen.height * 3 / 8), gui_textures[10]);

		if (level < 10) {
			GUI.DrawTexture(new Rect(Screen.width * 7 / 24,Screen.height * 12 / 16,Screen.width * 5 / 12,Screen.height * 6 / 80), gui_textures[11]);
		}

		GUI.EndGroup();

		//Reset
		if (!menu_active && !menu_transitioning && level < 10) {
			if (GUI.Button(new Rect(Screen.width * 7 / 24,Screen.height * 12 / 16,Screen.width * 5 / 12,Screen.height * 6 / 80), "")){
				level++;
				Reset();
			}
		}
	}

	//GUI for pull-down menu
	void GUI_Menu(){

		GUI.backgroundColor = Color.clear;

		GUI.BeginGroup(new Rect(0,0,Screen.width,Screen.height));

		GUI.DrawTexture(new Rect(Screen.width * 5 / 24,menu_position * Screen.height / 8,Screen.width * 7 / 12,menu_position * Screen.height * 3 / 4), gui_textures[2]);

		if (sfx) {
			GUI.DrawTexture (new Rect (Screen.width * 7 / 24, menu_position * Screen.height * 2 / 12, Screen.width / 6, menu_position * Screen.height / 10), gui_textures [4]);
		} else {
			GUI.DrawTexture (new Rect (Screen.width * 7 / 24, menu_position * Screen.height * 2 / 12, Screen.width / 6, menu_position * Screen.height / 10), gui_textures [5]);
		}

		if (music) {
			GUI.DrawTexture(new Rect(Screen.width * 13 / 24,menu_position * Screen.height * 2 / 12,Screen.width / 6,menu_position * Screen.height / 10), gui_textures[6]);
		} else {
			GUI.DrawTexture(new Rect(Screen.width * 13 / 24,menu_position * Screen.height * 2 / 12,Screen.width / 6,menu_position * Screen.height / 10), gui_textures[7]);
		}

		GUI.DrawTexture(new Rect(Screen.width * 7 / 24,menu_position * Screen.height * 6 / 8,Screen.width * 5 / 12,menu_position * Screen.height * 6 / 80), gui_textures[3]);

		GUI.EndGroup();

		if (menu_active && !menu_transitioning) {
			//Enable the buttons
			if (GUI.Button(new Rect (Screen.width * 7 / 24, menu_position * Screen.height * 2 / 12, Screen.width / 6, menu_position * Screen.height / 10), "")){
				sfx = !sfx;
				explosion_object.SendMessage ("ToggleAudio");
			}
			if (GUI.Button(new Rect(Screen.width * 13 / 24,menu_position * Screen.height * 2 / 12,Screen.width / 6,menu_position * Screen.height / 10), "")){
				music = !music;
			}
			if (GUI.Button(new Rect(Screen.width * 7 / 24,menu_position * Screen.height * 6 / 8,Screen.width * 5 / 12,menu_position * Screen.height * 6 / 80), "")){
				SceneManager.LoadScene(0);
			}
		}
	}

	//Retry & Next Level
	void Reset(){
		score = 0;
		timer = 0;
		dots_released = 0;
		dots_to_be_released = 10 + level * 2;
		GameObject[] to_destroy = GameObject.FindGameObjectsWithTag("dot");
		Debug.Log (to_destroy.Length);

		foreach (GameObject to_die in to_destroy) {
			GameObject.Destroy (to_die);
		}
		enemy_objects = new List<GameObject> ();

		camera_object.transform.position = new Vector3 (0, 0, -10);
		camera_component.orthographicSize = 4;

		focus_animation_active = false;
		focus_animation_timer = 0;
		menu_active = false;
		menu_position = 0;
		menu_transitioning = false;

		game_over = false;
		game_won = false;
		cannon_object.transform.eulerAngles = Vector3.zero;
		grin_object.SendMessage ("Hide");
		grin_object.SendMessage ("Disappear");

		is_firing = false;
		rocket_fired = false;
		firing_cooldown = 0;

		if (music) {
			player.Stop ();
			player.PlayOneShot (music_jingle);
		}

		paused = false;

	}

}
