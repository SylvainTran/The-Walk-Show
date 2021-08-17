using System;
using UnityEngine;

/// <summary>
/// The edge object implementation in Unity.
/// Used to connect way points in an adjacency list or matrix structure.
/// With Monobehaviour, could also be visualized.
/// </summary>
public class EdgeObject : MonoBehaviour
{
    /// <summary>
    /// The element at this edge.
    /// </summary>
    public EdgeObjectElement element;
    /// <summary>
    /// Start point vertex.
    /// </summary>
    public GameWaypoint startPoint;
    /// <summary>
    /// End point vertex.
    /// </summary>
    public GameWaypoint endPoint;
}
