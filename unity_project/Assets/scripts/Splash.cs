using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour {

	public Transform _dirLight;

	void Awake () {
		if (null != _dirLight) {
			_dirLight.eulerAngles = new Vector3 (_dirLight.eulerAngles.x, -100f, _dirLight.eulerAngles.z);
		}
	}

	void Update () {
		if (null != _dirLight && Mathf.Abs(_dirLight.eulerAngles.y) > 1f) {
			_dirLight.eulerAngles = new Vector3 (_dirLight.eulerAngles.x, _dirLight.eulerAngles.y + Time.deltaTime * 50f, _dirLight.eulerAngles.z);
		}

		if (Input.GetMouseButtonUp(0)) {
			SceneManager.LoadScene ("dev_henri");
		}
	}
}
