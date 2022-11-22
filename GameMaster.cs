using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMaster : MonoBehaviour {

    // an enum to hold the type of objects we can have in the maze
    public enum CellType
    {
        space, wall, blueKey, blueDoor, redKey, redDoor,
        yellowKey, yellowDoor, greenKey, greenDoor, purpleKey, purpleDoor
    };

    // A Point struct to be stored in linked lists by Prim's algorithm.
    public struct Point {
        public int x, y;
        public Point(int vx, int vy) : this() { this.x = vx; this.y = vy; }
    };

    public CellType[,] maze;
    public GameObject[,] bricks;

    public int mazeWidth, mazeHeight;
    static int offsetX=2, offsetY=2;
    static float tileSize = 0.6f;
    int doorsLeft;
    public static int level = 1;

    public GameObject brickRef;

    // Create a maze and the 3D objects composing it
    void MakeMaze(int cols, int rows)
    {
        mazeWidth = cols;
        mazeHeight = rows;

        // set the offsets so that (0, 0) is in the center of the maze
        offsetX = mazeWidth / 2;
        offsetY = mazeHeight / 2;
        
        // allocate the array
        maze = new CellType[mazeWidth, mazeHeight];
        
        // create the CellType maze
        Prim();
        // create the maze of wall 3D objects
        BuildMaze();
    }

    // Initialize all the cells in the maze with the same value
    void InitMaze(CellType val)
    {
        for (int i = 0; i < mazeWidth; i++)
            for (int j = 0; j < mazeHeight; j++)
                maze[i, j] = val;
    }

    // returns the cells neighboring the given one in a list
    // considers a 4-cell neighborhood.
    // returns less than 4 nodes if close to a border
    LinkedList<Point> Neighbors(int cx, int cy)
    {
        LinkedList<Point> neighbs = new LinkedList<Point>();
        if (cx > 1)
        {
            neighbs.AddFirst(new Point(cx - 1, cy));
        }
        if (cx < mazeWidth - 2)
        {
            neighbs.AddLast(new Point(cx + 1, cy));
        }
        if (cy > 1)
        {
            neighbs.AddLast(new Point(cx, cy - 1));
        }
        if (cy < mazeHeight - 2)
        {
            neighbs.AddLast(new Point(cx, cy + 1));
        }
        return neighbs;
    }

    // converts a column number into a position on the stage
    static public float Col2X(int c)
    {
        return c * tileSize - offsetX * tileSize;
    }

    // converts a column number into a position on the stage
    static public float Row2Y(int r)
    {
        return r * tileSize - offsetY * tileSize;
    }

    // converts an x position to a column number
    static public int X2col(float xVal)
    {
        return offsetX + (int)(xVal / tileSize);
    }

    // converts a y position to a row number
    static public int Y2row(float yVal)
    {
        return offsetY + (int)(yVal / tileSize);
    }

    // builds the physical maze out of wall objects, 
    // assuming that we have generated it beforehand in the maze array.
    void BuildMaze()
    {
        int i, j;
        float brickWidth, brickY;
        brickY = brickRef.transform.position.y;
        brickWidth = brickRef.transform.localScale.x;
        tileSize = brickWidth+0.001f; // a small gap between the bricks to be able to see them

        bricks = new GameObject[mazeWidth, mazeHeight];
        for (i = 0; i < mazeWidth; i++)
            for (j = 0; j < mazeHeight; j++)
            {
                if (maze[i, j] == CellType.wall) // place a brick where we have a wall in the maze
                {
                    bricks[i, j] = Instantiate(brickRef) as GameObject;
                    bricks[i, j].transform.position = new Vector3(Col2X(i), brickY, Row2Y(j));
                }
                else
                {
                    bricks[i,j] = null;
                }
            }
    }

    // implementation of Prim's algorithm to generate a maze
    void Prim()
    {
        LinkedListNode<Point> node;
        int cx, cy, dx, dy, r;
        LinkedList<Point> neighbs;

        // start by filling up the maze with walls
        InitMaze(CellType.wall);

        // start from the center of the maze with a space
        // that's where the player should be
        int startx = mazeWidth / 2, starty = mazeHeight / 2;

        // the frontier contains all the neighbors of the starting position
        LinkedList<Point> frontier = Neighbors(startx, starty);
        maze[startx, starty] = CellType.space;

        while (frontier.Count > 0)               // while we still have nodes in the frontier
        {
            r = Random.Range(0, frontier.Count); // choose a random one
            node = NodeAtIndex(frontier, r);
            cx = node.Value.x;
            cy = node.Value.y;
            frontier.Remove(node);               // remove it from the frontier
            neighbs = Neighbors(cx, cy);
            if (CountSpaces(neighbs) == 1)       // make sure it doesn't close a cycle
            {
                maze[cx, cy] = CellType.space;   // make it a space

                for (node = neighbs.First; node != null; node = node.Next) // process its neighbors
                {
                    dx = node.Value.x;
                    dy = node.Value.y;
                    // if the neighbor is not a space
                    if (maze[dx, dy] != CellType.space)
                    {
                        LinkedList<Point> n = Neighbors(dx, dy);
                        // if it has exactly one space among its neighbors and is not already in the frontier
                        if (CountSpaces(n) == 1 && !ContainsNode(frontier, dx, dy))
                        {
                            //add this neighbor to the frontier;
                            frontier.AddLast(new Point(dx, dy));
                        }
                    }
                }
            }
        }	
    }

    // returns the node at a given index from the list
    LinkedListNode<Point> NodeAtIndex(LinkedList<Point> list, int index)
    {
        LinkedListNode<Point> n = list.First;
        int i = 0;
        while (n!= null && i<index)
        {
            n = n.Next;
            ++i;
        }
        return n;
    }

    // counts the spaces in the list
    int CountSpaces(LinkedList<Point> neighb)
    {
        int count = 0;
        for (LinkedListNode<Point> n = neighb.First; n != null; n = n.Next)
        {
            if (maze[n.Value.x, n.Value.y] == CellType.space)
            {
                count++;
            }
        }
        return count;
    }

    // checks if the list contains a node given with x and y
    bool ContainsNode(LinkedList<Point> list, int cx, int cy)
    {
        LinkedListNode<Point> p = list.First;
        while (p != null)
        {
            if (p.Value.x == cx && p.Value.y == cy)
            {
                return true;
            }
            p = p.Next;
        }
        return false;
    }

    // Use this for initialization
    void Start () {
        MakeMaze(10, 10);
	}
	
	// Update is called once per frame
	//void Update () {
	//}
}
