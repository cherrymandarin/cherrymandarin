using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeSameLogic : MonoBehaviour {

    public class Node
    {
        public int x;
        public int y;
        public int type;
        public GameObject go;
    }
    public List<List<Node>> grid;
    public List<Node> nodes;

    public int WIDTH = 8;
    public int HEIGHT = 8;

    public List<GameObject> symbols;

    public int moves = 20;
    public int movesLeft = 20;

    private int[] checklist = new int[] {
            0, 1, 0, 2,
            0, 1, 0, 3,
            0, 2, 0, 3,
            0, 1, 1, 2,
            0, 1, -1, 2,
            1, 1, 0, 2,
            -1, 1, 0, 2,
            1, 1, 1, 2,
            -1, 1, -1, 2,
            0, -1, 0, -2,
            0, -1, 0, -3,
            0, -2, 0, -3,
            0, -1, 1, -2,
            0, -1, -1, -2,
            1, -1, 0, -2,
            -1, -1, 0, -2,
            1, -1, 1, -2,
            -1, -1, -1, -2,
            0, -1, 0, 1,
            0, -2, 0, 1,
            0, -1, 0, 2,
            1, -1, 0, 1,
            -1, -1, 0, 1,
            0, -1, -1, 1,
            0, -1, 1, 1,
            1, -1, 1, 1,
            -1, -1, -1, 1,
            1, 0, 2, 0,
            2, 0, 3, 0,
            1, 0, 3, 0,
            1, -1, 2, -1,
            1, 1, 2, 1,
            1, -1, 2, 0,
            1, 1, 2, 0,
            1, 0, 2, -1,
            1, 0, 2, 1,
            -1, 0, -2, 0,
            -2, 0, -3, 0,
            -1, 0, -3, 0,
            -1, -1, -2, -1,
            -1, 1, -2, 1,
            -1, -1, -2, 0,
            -1, 1, -2, 0,
            -1, 0, -2, -1,
            -1, 0, -2, 1,
            -1, 0, 1, 0,
            -2, 0, 1, 0,
            -1, 0, 2, 0,
            -1, 0, 1, 1,
            -1, 0, 1, -1,
            -1, 1, 1, 0,
            -1, -1, 1, 0,
            -1, 1, 1, 1,
            -1, -1, 1, -1
    };
    
	// Use this for initialization
	void Start () {
        this.buildGrid();
        this.debugPrint();
        this.buildInitialConnections();
        this.debugPrint();
        this.randomizeGrid();
        this.debugPrint();
    }

    private void randomizeNoMove(Node node )
	{
        List<int> possible = new List<int>();
        for (int i = 0; i < symbols.Count; i++) possible.Add(i);
        if(node.x > 0)
        {
            if(possible.Contains(grid[node.x-1][node.y].type))
                possible.Remove(grid[node.x - 1][node.y].type);
        }
        if (node.x < WIDTH-1)
        {
            if (possible.Contains(grid[node.x + 1][node.y].type))
                possible.Remove(grid[node.x + 1][node.y].type);
        }
        if (node.y > 0)
        {
            if (possible.Contains(grid[node.x][node.y-1].type))
                possible.Remove(grid[node.x][node.y-1].type);
        }
        if (node.y <HEIGHT-1)
        {
            if (possible.Contains(grid[node.x ][node.y+1].type))
                possible.Remove(grid[node.x][node.y+1].type);
        }

        node.type = possible[Random.Range(0, possible.Count)];
    }
    
    private void randomizeGrid()
	{
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                var node = grid[x][y];
                if (node.type >= 0) continue;
                randomizeNoMove(node);
            }
        }
	}

    private void randomValue(Node node )
    {
        node.type = Random.Range(0, symbols.Count);
    }

    private void buildGrid()
    {
        this.grid = new List<List<Node>>();
        this.nodes = new List<Node>();
        for(int i = 0; i < WIDTH;i++)
        {
            this.grid.Add(new List<Node>(HEIGHT));
            for(int j = 0; j < HEIGHT;j++)
            {
                var node = new Node();
                node.type = -1;
                node.x = i;
                node.y = j;
                this.grid[i].Add(node);
                nodes.Add(node);
            }
        }
    }

    private void buildInitialConnections()
    {
        //Populate the grid with values that are found from the start
        int count = 5;
        List<int> possible = new List<int>();

        for (int i = 0; i < WIDTH * (HEIGHT - 1); i++)
        {
            if (i % WIDTH < WIDTH - 1)
            {
                possible.Add(i);
            }
        }

        //Remove rightmost corner as it's too close to edge on both sides.
        possible.Remove((WIDTH - 2) + (HEIGHT - 2) * WIDTH);
        
        List<List<int>> clearOnSpawn = new List<List<int>>();
        List<int> l1 = new List<int>();
        l1.Add(-1);
        l1.Add(0);
        clearOnSpawn.Add(l1);
        l1 = new List<int>();
        l1.Add(1);
        l1.Add(0);
        clearOnSpawn.Add(l1);
        l1 = new List<int>();
        l1.Add(0);
        l1.Add(1);
        clearOnSpawn.Add(l1);
        l1 = new List<int>();
        l1.Add(0);
        l1.Add(-1);
        clearOnSpawn.Add(l1);
		
		for (int i=0; i < count; i++)
		{
			if (possible.Count == 0){
				continue;
			}

            int pos = possible[Random.Range(0, possible.Count)];
			int xp = pos % WIDTH;
			int yp = (int)Mathf.Floor(pos / WIDTH);
			
			possible.Remove(pos);
			foreach (List<int> off in clearOnSpawn)
			{
				if (xp + off[0] < 0 || xp+off[0] >= WIDTH || yp+off[1] < 0 || yp+off[1] >= HEIGHT)
					continue;
				int rp = xp + off[0] + (yp + off[1]) * WIDTH;
                possible.Remove(rp);
			}
            bool vertical = Random.value < 0.5f;
			if (xp >= WIDTH - 2) vertical = true;
			if (yp >= HEIGHT - 2) vertical = false;
			
			int xd = vertical? 0 : 1;
			int yd = vertical? 1 : 0;
			int offset = vertical? 1 : WIDTH;

            List<int> nextPositions = new List<int>();
            nextPositions.Add(pos);
            nextPositions.Add(offset + xp + xd * 1 + (yp + yd * 1) * WIDTH);
            nextPositions.Add(offset + xp + xd * 2 + (yp + yd * 2) * WIDTH);
			
			//Test if chosen orientation is possible
			if (!(this.nodes[nextPositions[0]].type == -1 && this.nodes[nextPositions[1]].type == -1 && this.nodes[nextPositions[2]].type == -1))
				continue;

            //Set the values
            List<int> possibleValues = new List<int>();
            for (int j= 0; j < symbols.Count;j++)
            {
                possibleValues.Add(i);
            }
			
			//Remove values that area near the spawning ones
			foreach (int p in nextPositions)
            {
                int xsp = p % WIDTH;
                int ysp = (int)Mathf.Floor(p / WIDTH);
                foreach (List<int> off in clearOnSpawn)
                {
                    if (xsp + off[0] < 0 || xsp + off[0] >= WIDTH || ysp + off[1] < 0 || ysp + off[1] >= HEIGHT)
                    continue;
                    int rp = xsp + off[0] + (ysp + off[1]) * WIDTH;
                    if (nodes[rp].type >= 0)
                        possibleValues.Remove(nodes[rp].type);
                }
            }

            //Set the values
            int val = possibleValues [Random.Range(0, possibleValues.Count)];
            foreach (int p in nextPositions)
            {
                nodes[p].type = val;
                possible.Remove(p);
            }
        }
    }

    public List<Node> swap(Node node1, Node node2)
	{
		//Swap values
		int t = node1.type;
        node1.type = node2.type;
        node2.type = t;

        List<Node> search = new List<Node>();
        search.Add(node1);
        search.Add(node2);
	    List<Node> all = findCombined(search);
		
		//Reset the swap if it wasnt acceptable
		if (all.Count == 0)
		{
			node2.type = node1.type;
			node1.type = t;
		}
		else
		{
			movesLeft--;
		}
		
		return all;
	}

    private List<Node> findCombined(List<Node> nodes)
	{
		//Check if nodes create lines
		List<Node> all = new List<Node>();
		foreach (Node node in nodes)
		{
			//Dont calculate duplicates
			if (all.Contains(node)) continue;
			var horizontalRemove = hasLineHor(node);
            var verticalRemove = hasLineVer(node);

            foreach(Node n in horizontalRemove)
            {
                if (!all.Contains(n)) all.Add(n);
            }
            foreach (Node n in verticalRemove)
            {
                if (!all.Contains(n)) all.Add(n);
            }
        }
        return all;
	}

    private List<Node> hasLineHor(Node node)
	{
		//Check horizontal
		List<Node> found = new List<Node>();
		int xo = node.x;
		int yo = node.y;
        int search = node.type;
		while (xo<WIDTH && grid[xo][yo].type == search)
		{
			found.Add(grid[xo][yo]);
			xo++;
		}
        xo = node.x-1;
		while (xo >= 0 && grid[xo][yo].type == search)
		{
			found.Add(grid[xo][yo]);
			xo--;
		}
		if (found.Count< 3) found.Clear();
		return found;
	}
	
	private List<Node> hasLineVer(Node node)
	{
		//Check vertical
		List<Node> found = new List<Node>();
		int yo = node.y;
		int x = node.x;
        int search = node.type;
		while (yo<HEIGHT && grid[x][yo].type == search)
		{
			found.Add(grid[x][yo]);
			yo++;
		}
		yo = node.y-1;
		while (yo >= 0 && grid[x][yo].type == search)
		{
			found.Add(grid[x][yo]);
			yo--;
		}
		if (found.Count< 3) found.Clear();
		return found;
	}

    public void remove(List<Node> removed)
	{
		foreach (Node node in removed)
		{
            node.type = -1;
		}
        //Bubble the empty values up
        removed.Sort((a, b) => { return a.x - b.y; });
		foreach (Node node in removed)
		{
			while (true)
			{
				if (node.y == 0) break;
				
				Node next = this.grid[node.x][node.y - 1];
                int t = node.type;
                node.type = next.type;
                next.type = t;
                next = node;
            }
		}
	}
	
    public void resetGrid()
    {
        this.buildInitialConnections();
        this.randomizeGrid();
    }

    public List<Node> spawn()
    {
        //Find out removed.
        List<Node> removed = new List<Node>();
        nodes.ForEach(n => { if (n.type == -1) removed.Add(n); });

        //Find out all moves
        List<List<Node>> possible = this.findPossibleMoves();

        foreach (Node node in removed)
        {
            randomValue(node);
        }

        //Test if theres removals
        List<Node> toRemove = findCombined(nodes);
        
        return toRemove;
    }

    public List<List<Node>> findPossibleMoves()
	{
        List<List<Node>> found = new List<List<Node>>();
		for ( int x = 0; x <WIDTH; x++)
		{
			for (int y = 0; y < HEIGHT; y++)
			{
				var node = grid[x][y];
                List < Node > moves = findMoves(node);
				if (moves.Count > 0) found.Add(moves);
			}
		}
		return found;
	}

    private List<Node> findMoves(Node node)
	{
		//Check if movement to left would trigger line
		int x = node.x;
	    int y = node.y;
        List<Node> found = new List<Node>();
		int value = node.type;
        if (x > 0)
        {
            if (checkForLineHor(x - 1, y, new int[] { -1 }, value) || checkForLineVer(x - 1, y, new int[]{ -1, 1}, value))
			{
				found.Add(node);
				found.Add(grid[x - 1][y]);
			}
		}
		//Check if movement to right would trigger line
		if (x<WIDTH-1)
		{
			if (checkForLineHor(x+1, y, new int[] { 1 }, value) || checkForLineVer(x+1, y, new int[] { -1, 1 }, value))
			{
				found.Add(node);
				found.Add(grid[x + 1][y]);
			}
		}
		
		//Check if movement to up would trigger line
		if (y > 0)
		{
			if (checkForLineVer(x, y-1, new int[] { -1 }, value) || checkForLineHor(x, y-1,new int[] { -1, 1 }, value))
			{
				found.Add(node);
				found.Add(grid[x][y - 1]);
			}
		}
		//Check if movement to down would trigger line
		if (y<HEIGHT-1)
		{
			if (checkForLineVer(x, y+1, new int[] { 1 }, value) || checkForLineHor(x, y+1, new int[] { -1, 1 }, value))
			{
				found.Add(node);
				found.Add(grid[x][y + 1]);
			}
		}
		return found;
	}
    private bool checkForLineHor(int x, int y, int[] offset, int value)
	{
		int c = 1;
		
        for(int i = 0; i < offset.Length; i++)
        {
            int off = offset[i];
		    var xo = x + off;
		    while (xo >= 0 && xo<WIDTH )
		    {
			    if (grid[xo][y].type != value)
			    {
				    break;
			    }
			    c++;
			    xo += off;
		    }
        }
        return c >= 3;
	}
	
	private bool checkForLineVer(int x, int y,int[] offset, int value)
	{
		int c = 1;

        for(int i = 0; i < offset.Length; i++)
        {
            int off = offset[i];
            int yo = y + off;
		    while (yo >= 0 && yo<HEIGHT)
		    {
			    if ( grid[x][yo].type!= value)
				    break;
			    c++;
			    yo += off;
		    }
        }

        return c >= 3;
	}

    private void debugPrint()
    {
        string s = "";
        for(int i = 0; i < HEIGHT; i++)
        {
            for(int j = 0; j < WIDTH; j++)
            {
                s += grid[j][i].type;
            }
            s += "\n";
        }
        Debug.Log("---state---");
        Debug.Log(s);
    }

	// Update is called once per frame
	void Update () {
		
	}
}
