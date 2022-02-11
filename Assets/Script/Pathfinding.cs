using System;
using System.Collections.Generic;
using UnityEngine;

//static class which provides utility functions for pathfinding
public class Pathfinding
{
    public static List<Vector2Int> AStar(Vector2Int start, Vector2Int end, bool allowDiagonalMoves = true)
    {
        PriorityQueue<AStarNode> q = new PriorityQueue<AStarNode>();
        List<AStarNode> visitedNodes = new List<AStarNode>();

        AStarNode curr = new AStarNode(start, end);

        q.Enqueue(curr, curr.combinedHeuristic);
        Dictionary<Vector2Int, Vector2Int> validTileCache = new Dictionary<Vector2Int, Vector2Int>();

        while (!q.IsEmpty())
        {
            curr = q.Dequeue();

            if (curr.position == end)
            {
                //reached destination tile
                break;
            }

            visitedNodes.Add(curr);

            if (IsValidTile(curr.position + Vector2Int.right, validTileCache))
            {
                AStarNode temp = new AStarNode(curr.position + Vector2Int.right, curr, end);

                if (!visitedNodes.Contains(temp))
                {
                    if (!q.Contains(temp))
                    {
                        q.Enqueue(temp, temp.combinedHeuristic);
                    }
                    else if (temp.combinedHeuristic < q.GetPriority(temp))
                    {
                        q.SetPriority(temp, temp.combinedHeuristic);
                    }
                }
            }

            if (IsValidTile(curr.position + Vector2Int.left, validTileCache))
            {
                AStarNode temp = new AStarNode(curr.position + Vector2Int.left, curr, end);

                if (!visitedNodes.Contains(temp))
                {
                    if (!q.Contains(temp))
                    {
                        q.Enqueue(temp, temp.combinedHeuristic);
                    }
                    else if (temp.combinedHeuristic < q.GetPriority(temp))
                    {
                        q.SetPriority(temp, temp.combinedHeuristic);
                    }
                }
            }

            if (IsValidTile(curr.position + Vector2Int.up, validTileCache))
            {
                AStarNode temp = new AStarNode(curr.position + Vector2Int.up, curr, end);

                if (!visitedNodes.Contains(temp))
                {
                    if (!q.Contains(temp))
                    {
                        q.Enqueue(temp, temp.combinedHeuristic);
                    }
                    else if (temp.combinedHeuristic < q.GetPriority(temp))
                    {
                        q.SetPriority(temp, temp.combinedHeuristic);
                    }
                }
            }

            if (IsValidTile(curr.position + Vector2Int.down, validTileCache))
            {
                AStarNode temp = new AStarNode(curr.position + Vector2Int.down, curr, end);

                if (!visitedNodes.Contains(temp))
                {
                    if (!q.Contains(temp))
                    {
                        q.Enqueue(temp, temp.combinedHeuristic);
                    }
                    else if (temp.combinedHeuristic < q.GetPriority(temp))
                    {
                        q.SetPriority(temp, temp.combinedHeuristic);
                    }
                }
            }

            if (allowDiagonalMoves)
            {
                if (IsValidTile(curr.position + Vector2Int.up + Vector2Int.left, validTileCache))
                {
                    AStarNode temp = new AStarNode(curr.position + Vector2Int.up + Vector2Int.left, curr, end);

                    if (!visitedNodes.Contains(temp))
                    {
                        if (!q.Contains(temp))
                        {
                            q.Enqueue(temp, temp.combinedHeuristic);
                        }
                        else if (temp.combinedHeuristic < q.GetPriority(temp))
                        {
                            q.SetPriority(temp, temp.combinedHeuristic);
                        }
                    }
                }

                if (IsValidTile(curr.position + Vector2Int.up + Vector2Int.right, validTileCache))
                {
                    AStarNode temp = new AStarNode(curr.position + Vector2Int.up + Vector2Int.right, curr, end);

                    if (!visitedNodes.Contains(temp))
                    {
                        if (!q.Contains(temp))
                        {
                            q.Enqueue(temp, temp.combinedHeuristic);
                        }
                        else if (temp.combinedHeuristic < q.GetPriority(temp))
                        {
                            q.SetPriority(temp, temp.combinedHeuristic);
                        }
                    }
                }

                if (IsValidTile(curr.position + Vector2Int.down + Vector2Int.left, validTileCache))
                {
                    AStarNode temp = new AStarNode(curr.position + Vector2Int.down + Vector2Int.left, curr, end);

                    if (!visitedNodes.Contains(temp))
                    {
                        if (!q.Contains(temp))
                        {
                            q.Enqueue(temp, temp.combinedHeuristic);
                        }
                        else if (temp.combinedHeuristic < q.GetPriority(temp))
                        {
                            q.SetPriority(temp, temp.combinedHeuristic);
                        }
                    }
                }

                if (IsValidTile(curr.position + Vector2Int.down + Vector2Int.right, validTileCache))
                {
                    AStarNode temp = new AStarNode(curr.position + Vector2Int.down + Vector2Int.right, curr, end);

                    if (!visitedNodes.Contains(temp))
                    {
                        if (!q.Contains(temp))
                        {
                            q.Enqueue(temp, temp.combinedHeuristic);
                        }
                        else if (temp.combinedHeuristic < q.GetPriority(temp))
                        {
                            q.SetPriority(temp, temp.combinedHeuristic);
                        }
                    }
                }
            }
        }

        if (curr.position != end)
        {
            throw new Exception("Unknown Error Occured, destination tile not reached: start: " + start + " dest: " + end);
        }

        return GetPathListFromAStarNode(curr);
    }

    //creates a path ending at the current node and returns a list of the tile positions to travers to 
    //reach the current node the tile position at index 0 is the starting node an the last tile of 
    //the list is the current node
    private static List<Vector2Int> GetPathListFromAStarNode(AStarNode currNode)
    {
        if (currNode.from == null)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            path.Add(currNode.position);
            return path;
        }
        List<Vector2Int> prevPath = GetPathListFromAStarNode(currNode.from);
        prevPath.Add(currNode.position);
        return prevPath;
    }

    private static bool IsValidTile(Vector2Int tilePos, Dictionary<Vector2Int, Vector2Int> validTileCache)
    {
        //temp implementation we will need to be able to check in a room if a tile can be walked on or not
        return true; 
    }

    //object used by the A* algorithm to keep track of a single node's metrics
    private class AStarNode
    {
        private Vector2Int m_pos;
        private AStarNode m_from;
        private int m_distanceHeuristic;
        private int m_stepHeuristic;

        public AStarNode(Vector2Int pos, Vector2Int dest)
        {
            m_pos = pos;
            m_from = null;
            m_stepHeuristic = 0;
            m_distanceHeuristic = (int)(dest - pos).magnitude;
        }

        public AStarNode(Vector2Int pos, AStarNode from, Vector2Int dest, int stepValue = 1)
        {
            m_pos = pos;
            UpdateStep(from, stepValue);
            m_distanceHeuristic = (int)(dest - pos).magnitude;
        }

        public Vector2Int position
        {
            get
            {
                return m_pos;
            }
        }

        public AStarNode from
        {
            get
            {
                return m_from;
            }
        }

        public int stepHeuristic
        {
            get
            {
                return m_stepHeuristic;
            }
        }

        public int distanceHeuristic
        {
            get
            {
                return m_distanceHeuristic;
            }
        }

        public int combinedHeuristic
        {
            get
            {
                return m_stepHeuristic + m_distanceHeuristic;
            }
        }

        public void UpdateStep(AStarNode from, int stepVal = 1)
        {
            if (from == null)
            {
                throw new NullReferenceException();
            }
            m_from = from;
            m_stepHeuristic = from.stepHeuristic + stepVal;
        }

        //compares the positions of the nodes
        public static bool operator ==(AStarNode n1, AStarNode n2)
        {
            if ((object)n1 == null)
            {
                return (object)n2 == null;
            }
            else if ((object)n2 == null)
            {
                return false;
            }
            return n1.position == n2.position;
        }

        public static bool operator !=(AStarNode n1, AStarNode n2)
        {
            return !(n1 == n2);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool Equals(object o)
        {
            if (o == null)
            {
                return false;
            }

            if (o is AStarNode)
            {
                return this == (AStarNode)o;
            }
            return false;
        }

        public override String ToString()
        {
            return position.ToString() + " priority: " + combinedHeuristic;
        }
    }
}
