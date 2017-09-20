using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxAnimator : MonoBehaviour {

    public AnimationCurve inOut;

    private float size = 0.9f;
    private GameObject box;
    public Color origCol;
    public int type=0;
    private bool animatingInOut = false;
    private float ioStart = 0;

	// Use this for initialization
	void Start () {
		
	}

    private void Awake()
    {
        box = this.transform.GetChild(0).gameObject;
        origCol = this.transform.Find("Cube").GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update () {
        float ts = this.transform.localScale.x- (this.transform.localScale.x - size) / 3f;
        if (animatingInOut)
        {
            float phase = (Time.time - ioStart)*3.5f;
            if (phase >= 1)
            {
                phase = 1;
                animatingInOut = false;
            }
            ts = inOut.Evaluate(phase);
        }
        this.transform.localScale = new Vector3(ts, ts, ts);

        //   this.GetComponentInChildren<Renderer>().material.SetFloat("Metallic", (size + 0.18f));
        var mp =Mathf.Min(1, (size + 0.1f));
        mp *= mp;

        this.transform.Find("Cube").GetComponent<Renderer>().material.color = new Color(origCol.r, origCol.g , origCol.b , 1f*mp*mp*mp);
        float dance = Mathf.Max(0f,size - 0.9f);
        this.transform.localRotation = Quaternion.Euler(Mathf.Sin(Time.time * 5f) * dance * 75f, Mathf.Sin(Time.time * 20f) * dance * 145f,Mathf.Sin(Time.time*15f) * dance * 105f);
 	}
    public void animateDefault()
    {
        size = 0.9f;
    }
    public void animateLarge()
    {
        size = 0.93f;
    }
    public void animateSmall()
    {
        size = 0.8f;
    }
    public void animateExtraLarge()
    {
        size = 1.0f;
    }

    public void animateOutIn(float delay)
    {
        ioStart = Time.time+delay;
        animatingInOut = true;
    }
}
