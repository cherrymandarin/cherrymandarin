using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JerryManderin : MonoBehaviour {

    public List<GameObject> love_group;
    public List<GameObject> hate_group;
    public List<GameObject> neutral_group;

    private GameObject dragging;
    private Vector2 dragstartpoint;

    public LayerMask includeLayers;

    private Vector3 mouseDownBegin;
    private GameObject currentPiece;

    private enum Phase { START, END, CANCEL, UPDATE };

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
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
            else if (touch.phase != TouchPhase.Canceled)
            {
                this.handleInteraction(Phase.CANCEL, touch.position);
            }
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

        //TODO - should this be calculated based on screen resolution instead of absolute?
        if (phase == Phase.CANCEL)
        {
            this.currentPiece = null;
        }
        else if (phase == Phase.START)
        {
            //Fetch the piece being hit.
            mouseDownBegin = position;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            this.currentPiece = checkHit(ray);
        }
        else if (phase == Phase.END && this.currentPiece != null)
        {
            //TODO swap pieces
            this.currentPiece = null;
        }
        else if (phase == Phase.UPDATE && this.currentPiece != null)
        {
            //Update positions and other such when touch/mouse is down.
           
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
}
