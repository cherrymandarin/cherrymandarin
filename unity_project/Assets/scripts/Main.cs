﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

    public GameObject threesame;
    public GameObject jerrymandarin;

    public AnimationCurve cameraMove;

    private float cameratarget=-10f;
    private float cameraMoveStartTime = 0f;
    private bool cameraMoving = false;
    private bool cameraReverse = false;

    private int gamesplayed = 0;

    public Material active;
    // Use this for initialization
    void Start () {
		
	}

    private void Awake()
    {
        var tsui = threesame.transform.Find("ui").transform;
        threesame.transform.Find("ui").transform.localPosition = new Vector3(tsui.localPosition.x, 10f, tsui.localPosition.z);
    }
    // Update is called once per frame
    void Update () {
		if(cameraMoving)
        {
            float phase = Time.time - cameraMoveStartTime;
            phase *= 3f;
            if (phase > 1f)
            {
                phase = 1f;
                cameraMoving = false;
            }
            float pos = cameraMove.Evaluate(phase) * cameratarget;
            
            if (cameraReverse)
                pos = cameratarget - pos;
            Camera.main.transform.position = new Vector3(pos, 0f, -10f);
            float op = pos;
            jerrymandarin.transform.Find("ui").transform.position = new Vector3(0f, op, 0f);
            Debug.Log(10f - op);
            var tsui = threesame.transform.Find("ui").transform;
            threesame.transform.Find("ui").transform.localPosition = new Vector3(tsui.localPosition.x, 10f+op, tsui.localPosition.z);
        }
        if(gamesplayed < 3)
            this.transform.GetChild(gamesplayed).GetComponent<Renderer>().material = active;
    }

    public void moveToThreeSame(int lovePoints, int hatePoints, int neutralPoints, int draws)
    {
        threesame.GetComponent<ThreeSame>().reset();
        cameraMoving = true;
        cameraMoveStartTime = Time.time;
        cameraReverse = false;
    }

    public static int score0;
    public static int score1;
    public static int score2;

    private void toend()
    {
        Application.LoadLevel("End");
    }

    public void moveToJerryManderin()
    {
        gamesplayed++;
        if(gamesplayed == 3)
        {
            Invoke("toend", 1f);
        }
        else
        {
            jerrymandarin.GetComponent<JerryManderin>().reset();
            cameraReverse = true;
            cameraMoving = true;
            cameraMoveStartTime = Time.time;
        }
    }
}
