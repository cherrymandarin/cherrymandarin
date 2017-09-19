using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxAnimator : MonoBehaviour {

    private float size = 0.9f;
    private GameObject box;
    

	// Use this for initialization
	void Start () {
		
	}

    private void Awake()
    {
        box = this.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update () {
        float ts = this.transform.localScale.x- (this.transform.localScale.x - size) / 7f;
        this.transform.localScale = new Vector3(ts, ts, ts);
	}
    public void animateDefault()
    {
        size = 0.9f;
    }
    public void animateLarge()
    {
        size = 1.3f;
    }
    public void animateSmall()
    {
        size = 0.8f;
    }
}
