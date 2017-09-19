using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JerryLogic : MonoBehaviour {

    public int WIDTH = 9;
    public int HEIGHT = 9;

    public class JerryNode {
        public int type;
        public int height;
        public GameObject gameobject;
        public int column;
    };
    
    public class JerryColumn
    {
        public int position;
        public List<JerryNode> nodes;
        public int winner;
    }

    public List<JerryColumn> columns;
    public List<JerryNode> nodes;
    
	// Use this for initialization
	void Start () {
        nodes = new List<JerryNode>();
        columns = new List<JerryColumn>();
        var nc = 0;
        for (int i = 0; i < WIDTH; i++)
        {
            var column = new JerryColumn();
            column.position = i;
            column.nodes = new List<JerryNode>();

            columns.Add(column);

            var height = 0;
            while(height < 9)
            {
                //Create initial nodes.
                var node = new JerryNode();
                node.height = Random.Range(1, Mathf.Min(4,9-height));
                node.column = i;
                height += node.height;
                node.type = nc % 3;
                nc++;
                column.nodes.Add(node);
                nodes.Add(node);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		foreach(JerryColumn c in columns)
        {
            int lovecount = 0;
            int hatecount = 0;
            int neutralcount = 0;
            foreach(JerryNode n in c.nodes)
            {
                if (n.type == 0)
                    lovecount += n.height;
                else if (n.type == 1)
                    hatecount += n.height;
                else
                    neutralcount += n.height;
            }

            if(lovecount > hatecount && lovecount > neutralcount)
            {
                c.winner = 0;
            }
            else if(hatecount > lovecount && hatecount > neutralcount)
            {
                c.winner = 1;
            }
            else if(neutralcount > lovecount && neutralcount > hatecount)
            {
                c.winner = 2;
            }
            else
            {
                //Draw
                c.winner = -1;
            }
        }
    }
}
