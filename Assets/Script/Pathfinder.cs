using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinder : MonoBehaviour
{
    private const string OBSTACLES_TILEMAP_NAME = "Obstacles";
    private const string WALLS_TILEMAP_NAME = "Walls";
    private const int LATERAL_COST = 10;
    private const int DIAGONAL_COST = 14;

    private GridLayout grid;
    private Tilemap obstacles;
    private Tilemap walls;
    [SerializeField]
    private LayerMask enemyLayers = 64;
    private int maxX;
    private int minX;
    private int maxY;
    private int minY;

    private class GraphNode
    {
        public Vector2Int Position;
        public bool IsWalkable;
        public List<GraphEdge> Neighbours;
    }

    private class GraphEdge
    {
        public int Cost;
        public GraphNode Destination;
    }

    private class PathNode
    {
        public GraphNode Node;
        public int FValue;
        public int GValue;
        public int HValue;
        public PathNode Parent;

        public PathNode(GraphNode node)
        {
            Node = node;
        }
    }

    private GraphNode[,] graph;
    private List<GraphNode> unobstructedTiles;

    // Create graphs and register Pathfinder for Enemies in Room
    void Start()
    {
        GridLayout[] grids = GetComponentsInChildren<GridLayout>();
        foreach (GridLayout gridCandidate in grids)
        {
            if (gridCandidate.gameObject.name == OBSTACLES_TILEMAP_NAME)
            {
                grid = gridCandidate;
            }
        }

        Tilemap[] tilemaps = GetComponentsInChildren<Tilemap>();
        foreach (Tilemap tilemapCandidate in tilemaps)
        {
            if (tilemapCandidate.gameObject.name == OBSTACLES_TILEMAP_NAME)
            {
                obstacles = tilemapCandidate;
            }
            if (tilemapCandidate.gameObject.name == WALLS_TILEMAP_NAME)
            {
                walls = tilemapCandidate;
            }
        }

        // Find corners of walls
        bool found = false;
        maxX = int.MaxValue;
        maxY = int.MaxValue;
        minX = int.MinValue;
        minY = int.MinValue;

        for (int i = 0; i < walls.size.x && !found; i++)
        {
            for (int j = 0; j < walls.size.y && !found; j++)
            {
                TileBase tile = walls.GetTile(new Vector3Int(i + walls.origin.x, j + walls.origin.y, walls.origin.z));
                if (tile != null)
                {
                    minX = i;
                    minY = j;
                    found = true;
                }
            }
        }

        found = false;

        for (int i = walls.size.x - 1; i >= 0 && !found; i--)
        {
            for (int j = walls.size.y - 1; j >= 0 && !found; j--)
            {
                TileBase tile = walls.GetTile(new Vector3Int(i + walls.origin.x, j + walls.origin.y, walls.origin.z));
                if (tile != null)
                {
                    maxX = i;
                    maxY = j;
                    found = true;
                }
            }
        }

        // Build room tile graph
        graph = new GraphNode[walls.size.x, walls.size.y];

        for (int i = 0; i < walls.size.x; i++)
        {
            for (int j = 0; j < walls.size.y; j++)
            {
                TileBase tile = walls.GetTile(new Vector3Int(i + walls.origin.x, j + walls.origin.y, walls.origin.z));
                if (i <= minX || i >= maxX || j <= minY || j >= maxY || tile != null)
                {
                    // Obstacle
                    graph[i, j] = new GraphNode() {
                        Position = new Vector2Int(i, j),
                        IsWalkable = false,
                        Neighbours = new List<GraphEdge>()
                    };
                }
                else
                {
                    // Walkable
                    graph[i, j] = new GraphNode() {
                        Position = new Vector2Int(i, j),
                        IsWalkable = true,
                        Neighbours = new List<GraphEdge>()
                    };
                }
            }
        }

        for (int i = obstacles.origin.x - walls.origin.x; i < obstacles.size.x + (obstacles.origin.x - walls.origin.x); i++)
        {
            for (int j = obstacles.origin.y - walls.origin.y; j < obstacles.size.y + (obstacles.origin.y - walls.origin.y); j++)
            {
                TileBase tile = obstacles.GetTile(new Vector3Int(i + walls.origin.x, j + walls.origin.y, walls.origin.z));
                if (tile != null)
                {
                    // Obstacle
                    graph[i, j] = new GraphNode()
                    {
                        Position = new Vector2Int(i, j),
                        IsWalkable = false,
                        Neighbours = new List<GraphEdge>()
                    };
                }
                else if (graph[i, j] != null)
                {
                    // Do nothing
                }
                else
                {
                    // Walkable
                    graph[i, j] = new GraphNode()
                    {
                        Position = new Vector2Int(i, j),
                        IsWalkable = true,
                        Neighbours = new List<GraphEdge>()
                    };
                }
            }
        }

        // Populate neighbour lists
        foreach (GraphNode node in graph)
        {
            // Up
            if (node.Position.y + 1 < graph.GetLength(1))
            {
                GraphNode destination = graph[node.Position.x, node.Position.y + 1];
                if (destination.IsWalkable)
                {
                    node.Neighbours.Add(new GraphEdge()
                    {
                        Cost = LATERAL_COST,
                        Destination = destination
                    });
                }
            }
            // Down
            if (node.Position.y - 1 >= graph.GetLowerBound(1))
            {
                GraphNode destination = graph[node.Position.x, node.Position.y - 1];
                if (destination.IsWalkable)
                {
                    node.Neighbours.Add(new GraphEdge()
                    {
                        Cost = LATERAL_COST,
                        Destination = destination
                    });
                }
            }
            // Left
            if (node.Position.x - 1 >= graph.GetLowerBound(0))
            {
                GraphNode destination = graph[node.Position.x - 1, node.Position.y];
                if (destination.IsWalkable)
                {
                    node.Neighbours.Add(new GraphEdge()
                    {
                        Cost = LATERAL_COST,
                        Destination = destination
                    });
                }
            }
            // Right
            if (node.Position.x + 1 < graph.GetLength(0))
            {
                GraphNode destination = graph[node.Position.x + 1, node.Position.y];
                if (destination.IsWalkable)
                {
                    node.Neighbours.Add(new GraphEdge()
                    {
                        Cost = LATERAL_COST,
                        Destination = destination
                    });
                }
            }
            // Upper-left
            if (node.Position.x - 1 >= graph.GetLowerBound(0)
                && node.Position.y + 1 < graph.GetLength(1))
            {
                GraphNode destination = graph[node.Position.x - 1, node.Position.y + 1];
                if (destination.IsWalkable
                    && graph[node.Position.x - 1, node.Position.y].IsWalkable
                    && graph[node.Position.x, node.Position.y + 1].IsWalkable)
                {
                    node.Neighbours.Add(new GraphEdge()
                    {
                        Cost = DIAGONAL_COST,
                        Destination = destination
                    });
                }
            }
            // Upper-right
            if (node.Position.x + 1 < graph.GetLength(0)
                && node.Position.y + 1 < graph.GetLength(1))
            {
                GraphNode destination = graph[node.Position.x + 1, node.Position.y + 1];
                if (destination.IsWalkable
                    && graph[node.Position.x + 1, node.Position.y].IsWalkable
                    && graph[node.Position.x, node.Position.y + 1].IsWalkable)
                {
                    node.Neighbours.Add(new GraphEdge()
                    {
                        Cost = DIAGONAL_COST,
                        Destination = destination
                    });
                }
            }
            // Lower-left
            if (node.Position.x - 1 >= graph.GetLowerBound(0)
                && node.Position.y - 1 >= graph.GetLowerBound(1))
            {
                GraphNode destination = graph[node.Position.x - 1, node.Position.y - 1];
                if (destination.IsWalkable
                    && graph[node.Position.x - 1, node.Position.y].IsWalkable
                    && graph[node.Position.x, node.Position.y - 1].IsWalkable)
                {
                    node.Neighbours.Add(new GraphEdge()
                    {
                        Cost = DIAGONAL_COST,
                        Destination = destination
                    });
                }
            }
            // Lower-right
            if (node.Position.x + 1 < graph.GetLength(0)
                && node.Position.y - 1 >= graph.GetLowerBound(1))
            {
                GraphNode destination = graph[node.Position.x + 1, node.Position.y - 1];
                if (destination.IsWalkable
                    && graph[node.Position.x + 1, node.Position.y].IsWalkable
                    && graph[node.Position.x, node.Position.y - 1].IsWalkable)
                {
                    node.Neighbours.Add(new GraphEdge()
                    {
                        Cost = DIAGONAL_COST,
                        Destination = destination
                    });
                }
            }
        }

        // Get list of unobstructed tiles
        unobstructedTiles = new List<GraphNode>();
        foreach (GraphNode node in graph)
        {
            if (node.IsWalkable)
            {
                unobstructedTiles.Add(node);
            }
        }

        // Register Pathfinder with Enemies
        float gridSizeX = grid.CellToWorld(walls.size + walls.origin).x - grid.CellToWorld(walls.origin).x;
        float gridSizeY = grid.CellToWorld(walls.size + walls.origin).y - grid.CellToWorld(walls.origin).y;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(walls.transform.position, new Vector2(gridSizeX, gridSizeY), 0, enemyLayers);
        for (int i = 0; i < colliders.Length; i++)
        {
            Enemy enemy = colliders[i].transform.parent.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.RegisterPathfinder(this);
            }
        }
    }

    public List<Vector2> FindPath(Vector2 startingPoint, Vector2 endingPoint)
    {
        List<Vector2> path = new List<Vector2>();
        Vector3Int startingTile = grid.WorldToCell(startingPoint) - walls.origin;
        Vector3Int endingTile = grid.WorldToCell(endingPoint) - walls.origin;

        // Check if destination is reachable, if not, return empty list
        if (endingTile.x >= graph.GetLength(0)
            || endingTile.x < graph.GetLowerBound(0)
            || endingTile.y >= graph.GetLength(1)
            || endingTile.y < graph.GetLowerBound(1)
            || !graph[endingTile.x, endingTile.y].IsWalkable)
        {
            return path;
        }

        PathNode[,] pathGraph = new PathNode[graph.GetLength(0) + graph.GetLowerBound(0), graph.GetLength(1) + graph.GetLowerBound(1)];
        PriorityQueue<PathNode> unvisited = new PriorityQueue<PathNode>();
        List<PathNode> visited = new List<PathNode>();

        // Handle case where player walks out of bounds
        endingTile.x = Mathf.Clamp(endingTile.x, graph.GetLowerBound(0), graph.GetLength(0) - 1);
        endingTile.y = Mathf.Clamp(endingTile.y, graph.GetLowerBound(1), graph.GetLength(1) - 1);

        PathNode endingNode = new PathNode(graph[endingTile.x, endingTile.y]);
        pathGraph[endingNode.Node.Position.x, endingNode.Node.Position.y] = endingNode;

        PathNode startingNode = new PathNode(graph[startingTile.x, startingTile.y]);
        startingNode.Parent = null;
        startingNode.GValue = 0;
        startingNode.HValue = 0;
        startingNode.FValue = startingNode.GValue + startingNode.HValue;
        pathGraph[startingNode.Node.Position.x, startingNode.Node.Position.y] = startingNode;
        unvisited.Enqueue(startingNode, startingNode.FValue);

        bool found = false;

        // Test surrounding nodes until exhausted or path is found
        while (!unvisited.IsEmpty() && !found)
        {
            PathNode currentNode = unvisited.Dequeue();
            foreach (GraphEdge edge in currentNode.Node.Neighbours)
            {
                PathNode neighbourNode = pathGraph[edge.Destination.Position.x, edge.Destination.Position.y];
                if (neighbourNode != null)
                {
                    if (neighbourNode == endingNode)
                    {
                        endingNode.Parent = currentNode;
                        found = true;
                        break;
                    }
                    else
                    {
                        if (neighbourNode.GValue > currentNode.GValue + edge.Cost)
                        {
                            neighbourNode.Parent = currentNode;
                            neighbourNode.GValue = currentNode.GValue + edge.Cost;
                            neighbourNode.FValue = neighbourNode.GValue + neighbourNode.HValue;
                            unvisited.SetPriority(neighbourNode, neighbourNode.FValue);
                        }
                    }
                }
                else
                {
                    PathNode newNode = new PathNode(edge.Destination);
                    newNode.Parent = currentNode;
                    newNode.GValue = currentNode.GValue + edge.Cost;
                    int distanceXFromEndpoint = Mathf.Abs(endingNode.Node.Position.x - newNode.Node.Position.x);
                    int distanceYFromEndpoint = Mathf.Abs(endingNode.Node.Position.y - newNode.Node.Position.y);
                    int distanceDifferentialFromEndpoint = Mathf.Abs(distanceXFromEndpoint - distanceYFromEndpoint);
                    newNode.HValue = Mathf.Min(distanceXFromEndpoint, distanceYFromEndpoint) * DIAGONAL_COST + distanceDifferentialFromEndpoint * LATERAL_COST;
                    newNode.FValue = newNode.GValue + newNode.HValue;
                    pathGraph[newNode.Node.Position.x, newNode.Node.Position.y] = newNode;
                    unvisited.Enqueue(newNode, newNode.FValue);
                }
            }
        }

        // Add nodes to path
        PathNode nodeToAdd = endingNode;
        while (nodeToAdd.Parent != null)
        {
            Vector2 tilePosition = walls.GetCellCenterWorld(new Vector3Int(nodeToAdd.Node.Position.x + walls.origin.x, nodeToAdd.Node.Position.y + walls.origin.y, walls.origin.z));
            path.Add(tilePosition);
            nodeToAdd = nodeToAdd.Parent;
        }

        // Reverse path
        path.Reverse();

        return path;
    }

    public Vector2 GetRandomUnobstructedTilePosition()
    {
        int randomCellIndex = (int)(Random.value * unobstructedTiles.Count - 1);
        Vector2 tilePosition = walls.GetCellCenterWorld(new Vector3Int(unobstructedTiles[randomCellIndex].Position.x + walls.origin.x, unobstructedTiles[randomCellIndex].Position.y + walls.origin.y, walls.origin.z));
        return tilePosition;
    }

    public Vector2 GetUpperLeftCornerPos()
    {
        GraphNode corner = graph[minX, maxY];
        Vector2 tileCenterPosition = walls.GetCellCenterWorld(new Vector3Int(corner.Position.x + walls.origin.x, corner.Position.y + walls.origin.y, walls.origin.z));
        Vector2 roomCornerPosition = tileCenterPosition + new Vector2(grid.cellSize.x/2, -grid.cellSize.y/2);
        return roomCornerPosition;
    }

    public Vector2 GetUpperRightCornerPos()
    {
        GraphNode corner = graph[maxX, maxY];
        Vector2 tileCenterPosition = walls.GetCellCenterWorld(new Vector3Int(corner.Position.x + walls.origin.x, corner.Position.y + walls.origin.y, walls.origin.z));
        Vector2 roomCornerPosition = tileCenterPosition + new Vector2(-grid.cellSize.x / 2, -grid.cellSize.y / 2);
        return roomCornerPosition;
    }
    public Vector2 GetLowerLeftCornerPos()
    {
        GraphNode corner = graph[minX, minY];
        Vector2 tileCenterPosition = walls.GetCellCenterWorld(new Vector3Int(corner.Position.x + walls.origin.x, corner.Position.y + walls.origin.y, walls.origin.z));
        Vector2 roomCornerPosition = tileCenterPosition + new Vector2(grid.cellSize.x / 2, grid.cellSize.y / 2);
        return roomCornerPosition;
    }

    public Vector2 GetLowerRightCornerPos()
    {
        GraphNode corner = graph[maxX, minY];
        Vector2 tileCenterPosition = walls.GetCellCenterWorld(new Vector3Int(corner.Position.x + walls.origin.x, corner.Position.y + walls.origin.y, walls.origin.z));
        Vector2 roomCornerPosition = tileCenterPosition + new Vector2(-grid.cellSize.x / 2, grid.cellSize.y / 2);
        return roomCornerPosition;
    }
}
