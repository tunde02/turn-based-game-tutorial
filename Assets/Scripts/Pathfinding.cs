using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private LayerMask obstaclesLayerMask;

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    public static Pathfinding Instance { get; private set; }

    private int width;
    private int height;
    private float cellSize;
    private GridSystem<PathNode> gridSystem;


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"There's more than one Pathfinding! {transform} - {Instance}");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Setup(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridSystem = new GridSystem<PathNode>(width, height, cellSize,
            (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));
        gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

        // Setup obstacles
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(new GridPosition(x, z));
                float raycastOffsetDistance = 5f;
                bool isObstacle = Physics.Raycast(
                    origin: worldPosition + Vector3.down * raycastOffsetDistance,
                    direction: Vector3.up,
                    maxDistance: raycastOffsetDistance * 2,
                    layerMask: obstaclesLayerMask
                );

                if (isObstacle)
                {
                    GetNodeAt(x, z).SetIsWalkable(false);
                }
            }
        }
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
    {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        PathNode startNode = gridSystem.GetGridObject(startGridPosition);
        PathNode endNode = gridSystem.GetGridObject(endGridPosition);
        openList.Add(startNode);

        // Initialize every PathNodes
        for (int x = 0; x < gridSystem.GetWidth(); x++)
        {
            for (int z = 0; z < gridSystem.GetHeight(); z++)
            {
                PathNode pathNode = gridSystem.GetGridObject(new GridPosition(x, z));

                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.CaculateFCost();
                pathNode.ResetCameFromPathNode();
            }
        }

        // Setup startNode's cost
        startNode.SetGCost(0);
        startNode.SetHCost(CaculateDistance(startGridPosition, endGridPosition));
        startNode.CaculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostPathNode(openList);

            if (currentNode == endNode)
            {
                pathLength = endNode.GetFCost();
                return CaculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode))
                    continue;

                if (!neighbourNode.IsWalkable())
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tempGCost =
                    currentNode.GetGCost() + CaculateDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());

                if (tempGCost < neighbourNode.GetGCost())
                {
                    neighbourNode.SetCameFromPathNode(currentNode);
                    neighbourNode.SetGCost(tempGCost);
                    neighbourNode.SetHCost(CaculateDistance(neighbourNode.GetGridPosition(), endGridPosition));
                    neighbourNode.CaculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        // No path found
        pathLength = 0;

        return null;
    }

    public int CaculateDistance(GridPosition gridPositionA, GridPosition gridPositionB)
    {
        GridPosition distanceGridPosition = gridPositionA - gridPositionB;
        int xDistance = Mathf.Abs(distanceGridPosition.x);
        int zDistance = Mathf.Abs(distanceGridPosition.z);
        int remaining = Mathf.Abs(xDistance - zDistance);

        // 대각선 거리로 먼저 계산하고, 나머지는 직선 거리로 계산
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostPathNode = pathNodeList[0];

        for (int i = 0; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost())
            {
                lowestFCostPathNode = pathNodeList[i];
            }
        }

        return lowestFCostPathNode;
    }

    private List<GridPosition> CaculatePath(PathNode endNode)
    {
        List<PathNode> pathNodeList = new List<PathNode>();
        PathNode currentNode = endNode;

        pathNodeList.Add(endNode);

        while (currentNode.GetCameFromPathNode() != null)
        {
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }

        pathNodeList.Reverse();

        List<GridPosition> gridPositionList = new List<GridPosition>();
        foreach (PathNode pathNode in pathNodeList)
        {
            gridPositionList.Add(pathNode.GetGridPosition());
        }

        return gridPositionList;
    }

    private List<PathNode> GetNeighbourList(PathNode pathNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();
        GridPosition gridPosition = pathNode.GetGridPosition();
        int[] dx = new int[] { -1, 0, 1, -1, 1, -1, 0, 1 };
        int[] dz = new int[] { -1, -1, -1, 0, 0, 1, 1, 1 };

        for (int i = 0; i < 8; i++)
        {
            int x = gridPosition.x + dx[i];
            int z = gridPosition.z + dz[i];

            if (0 <= x && x < gridSystem.GetWidth() && 0 <= z && z < gridSystem.GetHeight())
            {
                neighbourList.Add(GetNodeAt(x, z));
            }
        }

        return neighbourList;
    }

    private PathNode GetNodeAt(int x, int z)
    {
        return gridSystem.GetGridObject(new GridPosition(x, z));
    }

    public bool IsWalkableGridPosition(GridPosition gridPosition)
    {
        return gridSystem.GetGridObject(gridPosition).IsWalkable();
    }

    public void SetIsWalkableGridPosition(GridPosition gridPosition, bool isWalkable)
    {
        gridSystem.GetGridObject(gridPosition).SetIsWalkable(isWalkable);
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        return FindPath(startGridPosition, endGridPosition, out int pathLength) != null;
    }

    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }
}
