using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JerryManderin : MonoBehaviour
{
    public int moves = 10;

    public GameObject movetext;

    public List<GameObject> love_group;
    public List<GameObject> hate_group;
    public List<GameObject> neutral_group;

    public List<GameObject> winMarkers;


    public LayerMask includeLayers;

    private GameObject currentPiece;

    private Dictionary<GameObject, JerryLogic.JerryNode> mapGoToNode;

    private enum Phase { START, END, CANCEL, UPDATE };

    private GameObject startHit;
    private JerryLogic logic;

    // Use this for initialization
    void Start()
    {
        this.mapGoToNode = new Dictionary<GameObject, JerryLogic.JerryNode>();
        this.logic = this.GetComponent<JerryLogic>();

        for (int i = 0; i < logic.columns.Count; i++)
        {
            var column = logic.columns[i];
            var heightOff = 0;
            for (int j = 0; j < column.nodes.Count; j++)
            {
                var n = column.nodes[j];
                GameObject prefab = null;
                if (n.type == 0)
                    prefab = love_group[n.height - 1];
                else if (n.type == 1)
                    prefab = hate_group[n.height - 1];
                else
                    prefab = neutral_group[n.height - 1];

                float x = i*1.2f - 2.5f;
                float y = heightOff + n.height / 2f - 4.9f;
                float z = 0;
                
                // n.gameobject.transform.GetChild(0).transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                n.gameobject = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity, this.transform);
                heightOff += n.height;
                mapGoToNode.Add(n.gameobject, n);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Update winning rows
        
        for(int i = 0; i < logic.columns.Count; i++)
        {
            var c = logic.columns[i];
            this.winMarkers[i].transform.GetChild(0).gameObject.GetComponent<Renderer>().enabled = false;
            this.winMarkers[i].transform.GetChild(1).gameObject.GetComponent<Renderer>().enabled = false;
            this.winMarkers[i].transform.GetChild(2).gameObject.GetComponent<Renderer>().enabled = false;
            if(c.winner >= 0)
                this.winMarkers[i].transform.GetChild(c.winner).gameObject.GetComponent<Renderer>().enabled = true;
        }
        

        for (var i = 0; i < Input.touchCount; i++)
        {
            //TODO - this could be updated to support multiple touches. Currently only one is supported.
            var touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Ended)
            {
                this.handleInteraction(Phase.END, touch.position);
            }
            else if (touch.phase == TouchPhase.Began)
            {
                this.handleInteraction(Phase.START, touch.position);
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

    }

    void handleInteraction(Phase phase, Vector2 position)
    {
        var ray = Camera.main.ScreenPointToRay(new Vector3(position.x, position.y));
        var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, 0));
        //TODO - should this be calculated based on screen resolution instead of absolute?

        if (phase == Phase.START)
        {
            //Fetch the piece being hit.
            var hit = checkHit(ray);
            
            if (hit != null)
            {
                startHit = hit.transform.parent.gameObject;
                startHit.GetComponent<BoxAnimator>().animateLarge();
            }
        }
        else if (phase == Phase.END)
        {
            //Fetch the piece being hit.
            var hit = checkHit(ray);

            if (hit != null && hit.transform.parent.gameObject == startHit)
            {
                
                if (currentPiece == null)
                {
                    this.currentPiece = hit.transform.parent.gameObject;
                    var n = mapGoToNode[this.currentPiece];
                    Debug.Log("selected: " + n.type);
                    foreach (JerryLogic.JerryNode node in logic.nodes)
                    {
                        if (node.height == n.height && node.type != n.type && node.column != n.column || node == n)
                            node.gameobject.GetComponent<BoxAnimator>().animateLarge();
                        else
                            node.gameobject.GetComponent<BoxAnimator>().animateSmall();
                    }
                }
                else
                {
                    var hitTo = hit.transform.parent.gameObject;
                    var from = mapGoToNode[currentPiece];
                    var to = mapGoToNode[hitTo];
                    Debug.Log("swap: " + from.type + " to " + to.type);
                    if(from.height == to.height && from != to && from.type != to.type && from.column != to.column)
                    {
                        //Swap gameobjects
                        var t = currentPiece.transform.position;
                        currentPiece.transform.position = hitTo.transform.position;
                        hitTo.transform.position = t;

                        //Swap node types
                        var tt = from.type;
                        from.type = to.type;
                        to.type = tt;

                        mapGoToNode[currentPiece] = to;
                        mapGoToNode[hitTo] = from;

                        Debug.Log("after swap " + from.type + " to " + to.type);
                        moves--;
                        
                        //Animate from & to
                        from.gameobject.GetComponent<BoxAnimator>().animateOutIn(0.0f);
                        to.gameobject.GetComponent<BoxAnimator>().animateOutIn(0.0f);
                    }
                    currentPiece = null;
                    foreach (JerryLogic.JerryNode node in logic.nodes)
                    {
                        node.gameobject.GetComponent<BoxAnimator>().animateDefault();
                    }

                   
                }
            }
           
        }
        else if (phase == Phase.UPDATE && this.currentPiece != null)
        {
            
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
