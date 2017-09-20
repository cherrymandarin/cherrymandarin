using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour {

	public Transform _dirLight;

	private bool _hasTouched;

	private AudioManager _audioManager;
	private AudioSource _audio;

	private float _flashSec = 1f;

	void Awake () {
		if (null != _dirLight) {
			_dirLight.eulerAngles = new Vector3 (_dirLight.eulerAngles.x, -100f, _dirLight.eulerAngles.z);
		}

		_audioManager = FindObjectOfType<AudioManager> ();
		_audio = _audioManager.GetComponent<AudioSource> ();
	}

	void Update () {
		if (null != _dirLight && Mathf.Abs(_dirLight.eulerAngles.y) > 1f) {
			_dirLight.eulerAngles = new Vector3 (_dirLight.eulerAngles.x, _dirLight.eulerAngles.y + Time.deltaTime * 50f, _dirLight.eulerAngles.z);
		}

		if (!_hasTouched) {
			if (Input.GetMouseButtonUp (0)) {
				//SceneManager.LoadScene ("dev_henri");
				_hasTouched = true;
				_audio.PlayOneShot (_audioManager._audioClips [3]);
			}
		} else {
			_flashSec -= Time.deltaTime;
			_dirLight.GetComponent<Light> ().intensity += Time.deltaTime * 20f;
		}

		if (_flashSec < 0f) {
			SceneManager.LoadScene ("dev_henri");
		}
	}
}
