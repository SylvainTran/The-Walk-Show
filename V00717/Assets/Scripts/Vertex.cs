using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex: MonoBehaviour
{
    /// <summary>
    /// The integer index for this vertex in the adjacency structures.
    /// </summary>
    public int intKey;

    private Dictionary<Vertex, EdgeObject> outgoing = null;
    public Dictionary<Vertex, EdgeObject> Outgoing { get { return outgoing; } }
    private Dictionary<Vertex, EdgeObject> ingoing = null;
    public Dictionary<Vertex, EdgeObject> Ingoing { get { return ingoing; } }

    public Vertex[] vertices;
    public EdgeObject[] edges;

    /// <summary>
    /// Whether the graph is directed or not => may be useful later
    /// </summary>
    public bool isDirected = false;

    private void Start()
    {
        outgoing = new Dictionary<Vertex, EdgeObject>();

        for(int i = 0; i < outgoing.Count; i++)
        {
            outgoing.Add(vertices[i], edges[i]);
        }
        if(!isDirected)
        {
            ingoing = outgoing;
        } else
        {
            ingoing = new Dictionary<Vertex, EdgeObject>();
        }
    }

    public virtual Vertex Validate(Vertex p)
    {
        if (!(p.GetType() != this.GetType() ))
        {
            Debug.Log("Wrong vertex type!");
            return null;
        }
        Vertex vertex = (Vertex)p;
        return vertex;
    }
}
