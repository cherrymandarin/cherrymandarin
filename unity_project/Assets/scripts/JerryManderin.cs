using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JerryManderin : MonoBehaviour {

    public List<GameObject> love_group;
    public List<GameObject> hate_group;
    public List<GameObject> neutral_group;

    public List<GameObject> winMarkers;

    private GameObject dragging;
    private Vector2 dragstartpoint;

    public LayerMask includeLayers;
    public LayerMask draggingLayer;

    private Vector3 mouseDownBegin;
    private GameObject currentPiece;
    private float currentYOff;

    private enum Phase { START, END, CANCEL, UPDATE };

    private Dictionary<GameObject, JerryLogic.JerryNode> mapGoToNode;

    

    private JerryLogic logic;

    private bool hadTouch = false;

    // Use this for initialization
    void Start () {
        this.mapGoToNode = new Dictionary<GameObject, JerryLogic.JerryNode>();
        this.logic = this.GetComponent<JerryLogic>();
        
        for(int i = 0; i < logic.columns.Count; i++)
        {
            var column = logic.columns[i];
            var heightOff = 0;
            for(int j = 0; j < column.nodes.Count; j++)
            {
                var n = column.nodes[j];
                GameObject prefab = null;
                if (n.type == 0)
                    prefab = love_group[n.height - 1];
                else if (n.type == 1)
                    prefab = hate_group[n.height - 1];
                else 
                    prefab = neutral_group[n.height - 1];

                float x = i-1f;
                float y = heightOff + n.height / 2f-4.5f;
                float z = 0;
                Debug.Log(x+","+ y+","+ z);
               // n.gameobject.transform.GetChild(0).transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                n.gameobject = Instantiate(prefab, new Vector3(x,y,z), Quaternion.identity, this.transform);
                heightOff += n.height;
                mapGoToNode.Add(n.gameobject, n);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Update winning rows
        /*
        for(int i = 0; i < logic.columns.Count; i++)
        {
            var c = logic.columns[i];
            this.winMarkers[i].transform.GetChild(0).gameObject.GetComponent<Renderer>().enabled = true;
            this.winMarkers[i].transform.GetChild(1).gameObject.GetComponent<Renderer>().enabled = true;
            this.winMarkers[i].transform.GetChild(2).gameObject.GetComponent<Renderer>().enabled = true;
            this.winMarkers[i].transform.GetChild(c.winner).gameObject.GetComponent<Renderer>().enabled = true;
        }
        */
        //INTERACTIONS BELOW
        if(hadTouch && Input.touchCount == 0)
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
            for (int i = 0; i < logic.nodes.Count; i++)
            {
                var go = logic.nodes[i].gameobject;
                go.GetComponent<BoxAnimator>().animateDefault();
                go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, -0f);
            }
            this.currentPiece.transform.position = dragstartpoint;
            this.currentPiece = null;
        }
        else if (phase == Phase.START)
        {
            //Fetch the piece being hit.
            mouseDownBegin = position;
            var hit = checkHit(ray);
            
            if(hit != null)
            {
                this.currentPiece = checkHit(ray).transform.parent.gameObject;
                SetLayerRecursively(this.currentPiece, (int)Mathf.Log(draggingLayer.value, 2));
                dragstartpoint = currentPiece.transform.position;
                currentYOff = this.currentPiece.transform.position.y;
                currentPiece.transform.position = new Vector3(dragstartpoint.x, dragstartpoint.y, -1);
            }
        }
        else if (phase == Phase.END && this.currentPiece != null)
        {
            //swap pieces if possible.
            var over = checkHit(ray);
            if (over != null)
            {
                over = over.transform.parent.gameObject;
                var n = mapGoToNode[over];
                var cur = mapGoToNode[currentPiece];

                if (n.height == cur.height)
                {
                    //Swap gameobjects
                    var t = over.transform.position;
                    over.transform.position = dragstartpoint;
                    cur.gameobject.transform.position = t;

                    //Swap node types
                    var tt = cur.type;
                    cur.type = n.type;
                    n.type = tt;

                    over.transform.position = new Vector3(over.transform.position.x, over.transform.position.y, -0.5f);
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

            for (int i = 0; i < logic.nodes.Count; i++)
            {
                var go = logic.nodes[i].gameobject;
                SetLayerRecursively(go, (int)Mathf.Log(includeLayers.value,2));
                go.GetComponent<BoxAnimator>().animateDefault();
                go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, -0f);
            }
            
            this.currentPiece = null;
        }
        else if (phase == Phase.UPDATE && this.currentPiece != null)
        {
            this.currentPiece.GetComponent<BoxAnimator>().animateLarge();
            for (int i = 0; i < logic.nodes.Count; i++)
            {
                var go = logic.nodes[i].gameobject;
                go.GetComponent<BoxAnimator>().animateSmall();
                go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, -0f);
            }
            var over = checkHit(ray);
            if(over != null)
            {
                over =over.transform.parent.gameObject;
                var n = mapGoToNode[over];
                if(n.height == mapGoToNode[currentPiece].height)
                {
                    over.GetComponent<BoxAnimator>().animateLarge();
                    over.transform.position = new Vector3(over.transform.position.x, over.transform.position.y, -0.5f);
                }
            }
            //Update positions and other such when touch/mouse is down.
            currentPiece.GetComponent<BoxAnimator>().animateDefault();

            var pos = this.currentPiece.transform.parent.InverseTransformPoint(worldPos.x, worldPos.y, -1);
            //pos.x -= 1.4f;
            //pos.y += 1.0f;
            this.currentPiece.transform.position = pos;
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

    private void SetLayerRecursively(GameObject obj ,int newLayer  )
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
