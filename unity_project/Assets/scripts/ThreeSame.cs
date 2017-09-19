using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeSame : MonoBehaviour {
    
    private ThreeSameLogic logic;

    private Vector2 dragstartpoint;
    public LayerMask includeLayers;
    public LayerMask draggingLayer;

    public GameObject mainNode;

    private Vector3 mouseDownBegin;
    private GameObject currentPiece;
    private enum Phase { START, END, CANCEL, UPDATE };
    private Dictionary<GameObject, ThreeSameLogic.Node> mapGoToNode;

    private bool hadTouch = false;

    // Use this for initialization
    void Start ()
    {
        this.mapGoToNode = new Dictionary<GameObject, ThreeSameLogic.Node>();
        this.logic = this.GetComponent<ThreeSameLogic>();
        foreach(ThreeSameLogic.Node node in logic.nodes)
        {
            var go = Instantiate(mainNode, new Vector3(node.x*1.1f, node.y*1.1f, 0f), Quaternion.identity, this.transform);
            for(int i = 0; i < go.transform.childCount; i++)
            {
                var rndr = go.transform.GetChild(i).GetComponentsInChildren<Renderer>();
                foreach (Renderer r in rndr) r.enabled = false;
            }
            node.go = go;
            foreach(Renderer r in go.transform.GetChild(node.type).GetComponentsInChildren<Renderer>())r.enabled = true;
            mapGoToNode.Add(go, node);
        }
        this.transform.position = new Vector3(-7f, -4f, 0f);
	}
    
	// Update is called once per frame
	void Update () {
        //INTERACTIONS BELOW
        if (hadTouch && Input.touchCount == 0)
        {
            this.handleInteraction(Phase.END, new Vector2());
        }
        for (var i = 0; i < Input.touchCount; i++)
        {
            hadTouch = true;
            //TODO - this could be updated to support multiple touches. Currently only one is supported.
            var touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Ended)
            {
                hadTouch = false;
                this.handleInteraction(Phase.END, touch.position);
            }
            else if (touch.phase == TouchPhase.Began)
            {
                this.handleInteraction(Phase.START, touch.position);
            }/*
            else if (touch.phase != TouchPhase.Canceled)
            {
                this.handleInteraction(Phase.CANCEL, touch.position);
            }*/
            else
            {
                this.handleInteraction(Phase.UPDATE, touch.position);
            }

            return;
        }

        //Handle mouse only if no touch had been handled. Mouse is mainly for development purposes.

        if (Input.GetMouseButtonDown(0))
        {
            this.handleInteraction(Phase.START, new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        }
        else if (Input.GetMouseButtonUp(0))
        {
            this.handleInteraction(Phase.END, new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        }
        else if (Input.GetMouseButton(0))
        {
            this.handleInteraction(Phase.UPDATE, new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        }
    }

    void handleInteraction(Phase phase, Vector2 position)
    {
        float dx = this.mouseDownBegin.x - position.x;
        float dy = this.mouseDownBegin.y - position.y;
        var ray = Camera.main.ScreenPointToRay(new Vector3(position.x, position.y));
        var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, 0));
        //TODO - should this be calculated based on screen resolution instead of absolute?
        if (phase == Phase.CANCEL)
        {
            this.currentPiece.transform.position = dragstartpoint;
            this.currentPiece = null;
        }
        else if (phase == Phase.START)
        {
            //Fetch the piece being hit.
            mouseDownBegin = position;
            var hit = checkHit(ray);

            if (hit != null)
            {
                this.currentPiece = checkHit(ray).transform.parent.parent.gameObject;
                SetLayerRecursively(this.currentPiece, (int)Mathf.Log(draggingLayer.value, 2));
                dragstartpoint = currentPiece.transform.position;
                currentPiece.transform.position = new Vector3(dragstartpoint.x, dragstartpoint.y, -1);
            }
        }
        else if (phase == Phase.END && this.currentPiece != null)
        {
            //swap pieces if possible.
            var over = checkHit(ray);
            if (over != null)
            {
                over = over.transform.parent.parent.gameObject;
                var n = mapGoToNode[over];
                var cur = mapGoToNode[currentPiece];

                int distance = (int)(Mathf.Abs(n.x - cur.x) + Mathf.Abs(n.y-cur.y));
                if (distance == 1)
                {
                    //Swap node types
                    var tt = cur.type;
                    cur.type = n.type;
                    n.type = tt;

                    //update gos
                    GameObject go = n.go;
                    foreach (Renderer r in go.transform.GetChild(0).GetComponentsInChildren<Renderer>()) r.enabled = false;
                    foreach (Renderer r in go.transform.GetChild(1).GetComponentsInChildren<Renderer>()) r.enabled = false;
                    foreach (Renderer r in go.transform.GetChild(2).GetComponentsInChildren<Renderer>()) r.enabled = false;
                    foreach (Renderer r in go.transform.GetChild(3).GetComponentsInChildren<Renderer>()) r.enabled = false;
                    foreach (Renderer r in go.transform.GetChild(4).GetComponentsInChildren<Renderer>()) r.enabled = false;
                    foreach (Renderer r in go.transform.GetChild(5).GetComponentsInChildren<Renderer>()) r.enabled = false;
                    foreach (Renderer r in go.transform.GetChild(n.type).GetComponentsInChildren<Renderer>()) r.enabled = true;
                    //TODO
                    go = cur.go;
                    foreach (Renderer r in go.transform.GetChild(0).GetComponentsInChildren<Renderer>()) r.enabled = false;
                    foreach (Renderer r in go.transform.GetChild(1).GetComponentsInChildren<Renderer>()) r.enabled = false;
                    foreach (Renderer r in go.transform.GetChild(2).GetComponentsInChildren<Renderer>()) r.enabled = false;
                    foreach (Renderer r in go.transform.GetChild(3).GetComponentsInChildren<Renderer>()) r.enabled = false;
                    foreach (Renderer r in go.transform.GetChild(4).GetComponentsInChildren<Renderer>()) r.enabled = false;
                    foreach (Renderer r in go.transform.GetChild(5).GetComponentsInChildren<Renderer>()) r.enabled = false;
                    foreach (Renderer r in go.transform.GetChild(cur.type).GetComponentsInChildren<Renderer>()) r.enabled = true;

                }
                else
                {
                    this.currentPiece.transform.position = dragstartpoint;
                }
            }
            else
            {
                this.currentPiece.transform.position = dragstartpoint;
            }
            this.currentPiece = null;
        }
        else if (phase == Phase.UPDATE && this.currentPiece != null)
        {
            //todo suggest move soon to happen
        }
    }

    GameObject checkHit(Ray ray)
    {
        RaycastHit info = new RaycastHit();
        bool hits = Physics.Raycast(ray, out info, 300f, includeLayers);
        if (hits)
        {
            return info.collider.gameObject.gameObject;
        }
        else
        {
            return null;
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
