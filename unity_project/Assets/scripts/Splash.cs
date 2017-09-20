using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour {

	void Awake () {
		
	}

	void Update () {
		if (Input.GetMouseButtonUp(0)) {
			SceneManager.LoadScene ("dev_henri");
		}
	}
}
