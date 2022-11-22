using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private GridPosition gridPosition;
    private int gCost;
    private int hCost;
    private int fCost;
    private PathNode cameFromPathNode;
    private bool isWalkable = true;


    public PathNode(GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
    }

    public override string ToString()
    {
        return gridPosition.ToString();
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public int GetGCost()
    {
        return gCost;
    }

    public void SetGCost(int gCost)
    {
        this.gCost = gCost;
    }

    public int GetHCost()
    {
        return hCost;
    }

    public void SetHCost(int hCost)
    {
        this.hCost = hCost;
    }

    public int GetFCost()
    {
        return fCost;
    }

    public void CaculateFCost()
    {
        fCost = gCost + hCost;
    }

    public PathNode GetCameFromPathNode()
    {
        return cameFromPathNode;
    }

    public void SetCameFromPathNode(PathNode pathNode)
    {
        cameFromPathNode = pathNode;
    }

    public void ResetCameFromPathNode()
    {
        cameFromPathNode = null;
    }

    public bool IsWalkable()
    {
        return isWalkable;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
    }
}
