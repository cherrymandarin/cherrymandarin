using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeSame : MonoBehaviour {
    
    private ThreeSameLogic logic;

	// Use this for initialization
	void Start ()
    {
        this.logic = this.GetComponent<ThreeSameLogic>();
        foreach(ThreeSameLogic.Node node in logic.nodes)
        {
            Instantiate(this.logic.symbols[node.type], new Vector3(node.x, node.y, 0f), Quaternion.identity, this.transform);
        }
	}
    
	// Update is called once per frame
	void Update () {
		
	}
}
