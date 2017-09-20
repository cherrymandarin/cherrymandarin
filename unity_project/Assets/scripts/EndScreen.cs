using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class EndScreen : MonoBehaviour {
    public GameObject s1;
    public GameObject s2;
    public GameObject s3;
	// Use this for initialization
	void Start () {
		
	}
    private void Awake()
    {
        s1.GetComponent<TextMesh>().text = Main.score0 + "";
        s2.GetComponent<TextMesh>().text = Main.score1 + "";
        s3.GetComponent<TextMesh>().text = Main.score2 + "";
    }
    
    
    private bool _hasTouched;



    float _flashSec =0.5f;
    void Update()
    {
      
        if (!_hasTouched)
        {
            if (Input.GetMouseButtonUp(0))
            {
                //SceneManager.LoadScene ("dev_henri");
                _hasTouched = true;
            }
        }
        else
        {
            _flashSec -= Time.deltaTime;
           
        }

        if (_flashSec < 0f)
        {
            
            SceneManager.LoadScene("dev_henri");
        }
    }
}
