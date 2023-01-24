using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph
{
    private static List<Vector2Int> neighbours4directions = new List<Vector2Int>
    {
        new Vector2Int(0,1), new Vector2Int(1,0), new Vector2Int(0,-1), new Vector2Int(-1,0)
    };

    private static List<Vector2Int> neighbours8directions = new List<Vector2Int>
    {
        new Vector2Int(0,1),  new Vector2Int(1,0), new Vector2Int(0,-1), new Vector2Int(-1,0),
        new Vector2Int(1,1),  new Vector2Int(1,-1), new Vector2Int(-1,1), new Vector2Int(-1,-1)
    };

    List<Vector2Int> graph;
    public Graph(IEnumerable<Vector2Int> verticles)
    {
        graph = new List<Vector2Int>(verticles); 
    }

    public List<Vector2Int> GetNeighbours4Directions(Vector2Int startPosition)
    {
        return GetNeighbours(startPosition, neighbours4directions);
    }

    public List<Vector2Int> GetNeighbours8Directions(Vector2Int startPosition)
    {
        return GetNeighbours(startPosition, neighbours8directions);
    }

    private List<Vector2Int> GetNeighbours(Vector2Int startPosition, 
        List<Vector2Int> neighboursOffsetList)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();
        foreach (var direction in neighboursOffsetList)
        {
            Vector2Int potentialNeighbour = startPosition + direction;
            if (graph.Contains(potentialNeighbour))
                neighbours.Add(potentialNeighbour);
        }
        return neighbours;
    }
}
