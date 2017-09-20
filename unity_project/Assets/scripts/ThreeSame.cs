using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeSame : MonoBehaviour {

    public GameObject main;

    private ThreeSameLogic logic;

    private Vector2 dragstartpoint;
    public LayerMask includeLayers;
    public LayerMask draggingLayer;
    private Vector2 dragStartScreen;

    public GameObject mainNode;

    private Vector3 mouseDownBegin;
    private GameObject currentPiece;
    private enum Phase { START, END, CANCEL, UPDATE };
    private Dictionary<GameObject, ThreeSameLogic.Node> mapGoToNode;

    private bool hadTouch = false;

    private List<List<ThreeSameLogic.Node>> removing;

    // Use this for initialization
    void Start ()
    {
        removing = new List<List<ThreeSameLogic.Node>>();
        this.mapGoToNode = new Dictionary<GameObject, ThreeSameLogic.Node>();
        this.logic = this.GetComponent<ThreeSameLogic>();
        foreach(ThreeSameLogic.Node node in logic.nodes)
        {
            var go = Instantiate(mainNode, new Vector3(node.x*1.1f,7f*1.1f- node.y*1.1f, 0f), Quaternion.identity, this.transform);
            for(int i = 0; i < go.transform.childCount; i++)
            {
                var rndr = go.transform.GetChild(i).GetComponentsInChildren<Renderer>();
                foreach (Renderer r in rndr) r.enabled = false;
            }
            node.go = go;
            foreach(Renderer r in go.transform.GetChild(node.type).GetComponentsInChildren<Renderer>())r.enabled = true;
            mapGoToNode.Add(go, node);
        }
        this.transform.position = new Vector3(-17f, -4f, 0f);
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
        float dx = position.x-this.mouseDownBegin.x;
        float dy = position.y-this.mouseDownBegin.y;
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
                Debug.Log("currnet: " + currentPiece);
             //   SetLayerRecursively(this.currentPiece, (int)Mathf.Log(draggingLayer.value, 2));
                dragstartpoint = currentPiece.transform.position;
                currentPiece.transform.position = new Vector3(dragstartpoint.x, dragstartpoint.y, -1);
            }
        }
        else if (phase == Phase.END && this.currentPiece != null)
        {
            //swap pieces if possible.
            GameObject over = null;// checkHit(ray);
            var cur = mapGoToNode[currentPiece];
            ThreeSameLogic.Node n = null;
            Debug.Log("DELTAS " + dx + "," + dy);
            if (Mathf.Abs(dx) > 10 || Mathf.Abs(dy) >10)
            {

                //   over = over.transform.parent.parent.gameObject;
                if (Mathf.Abs(dx) > Mathf.Abs(dy))
                {
                    if (dx < 0 && cur.x > 0)
                    {
                        n = logic.grid[cur.x - 1][cur.y];
                        over = n.go;
                    }
                    else if (dx > 0 && cur.x < logic.WIDTH - 1)
                    {
                        n = logic.grid[cur.x + 1][cur.y];
                        over = n.go;
                    }
                }
                else
                {
                    if (dy > 0 && cur.y > 0)
                    {
                        n = logic.grid[cur.x][cur.y - 1];
                        over = n.go;
                    }
                    else if (dy < 0 && cur.y < logic.HEIGHT - 1)
                    {
                        n = logic.grid[cur.x][cur.y + 1];
                        over = n.go;
                    }
                }

            }
            Debug.Log("hit found: " + over);
            if (over != null)
            {
                
                int distance = (int)(Mathf.Abs(n.x - cur.x) + Mathf.Abs(n.y - cur.y));
                if (distance == 1)
                {
                    var remove = logic.swap(n, cur);
                    if (remove.Count > 0)
                    {
                        removing.Add(remove);
                        //Ok
                        n.go.GetComponent<TSBox>().animateOutIn(0f);
                        cur.go.GetComponent<TSBox>().animateOutIn(0f);
                        updatenode(n);
                        updatenode(cur);

                        logic.remove(remove);

                        Invoke("popRemove", 0.2f);
                    }
                    else
                    {
                        //Animate error
                    }
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

    private void popRemove()
    {
        Debug.Log("pop remove");
        var remove = removing[0];
        removing.RemoveAt(0);
        
        var nextRemove = logic.spawn();
        logic.remove(nextRemove);
        foreach (ThreeSameLogic.Node nod in remove)
        {
            updatenode(nod);
            nod.go.GetComponent<TSBox>().animateOutIn(0f);
        }
        if (nextRemove.Count > 0)
        {
            removing.Add(nextRemove);
            Invoke("popRemove", 0.2f);
        }
    }

    private void updatenode(ThreeSameLogic.Node node)
    {
        var go = node.go;
        if (node.type == -1) return;
        foreach (Renderer r in go.transform.GetChild(0).GetComponentsInChildren<Renderer>()) r.enabled = false;
        foreach (Renderer r in go.transform.GetChild(1).GetComponentsInChildren<Renderer>()) r.enabled = false;
        foreach (Renderer r in go.transform.GetChild(2).GetComponentsInChildren<Renderer>()) r.enabled = false;
        foreach (Renderer r in go.transform.GetChild(3).GetComponentsInChildren<Renderer>()) r.enabled = false;
        foreach (Renderer r in go.transform.GetChild(4).GetComponentsInChildren<Renderer>()) r.enabled = false;
        foreach (Renderer r in go.transform.GetChild(5).GetComponentsInChildren<Renderer>()) r.enabled = false;
        foreach (Renderer r in go.transform.GetChild(node.type).GetComponentsInChildren<Renderer>()) r.enabled = true;

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
