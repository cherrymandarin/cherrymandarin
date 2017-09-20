using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoDestroyPls : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		DontDestroyOnLoad (transform.gameObject);
	}
}
