
public class AdjacencyMap
{
    private bool isDirected = false;

    public AdjacencyMap(bool isDirected)
    {
        this.isDirected = isDirected;
    }

    /// <summary>
    /// Returns the edge object from vertex v to u using the map inside vertex.
    /// </summary>
    /// <param name="v">from</param>
    /// <param name="u">to</param>
    /// <returns></returns>
    public EdgeObject GetEdge(Vertex v, Vertex u)
    {
        GameWaypoint _v = (GameWaypoint)v.Validate(v);
        return v.Outgoing[u];
    }

    public EdgeObject[] Edges()
    {
        throw new System.NotImplementedException();
    }

    public Vertex[] EndVertices(EdgeObject e)
    {
        return new Vertex[] { e.startPoint, e.endPoint };
    }

    public EdgeObject[] IncidentEdges(Vertex v)
    {
        return v.edges;
    }

    public void InsertEdge(Vertex v, Vertex w, EdgeObjectElement e)
    {
        throw new System.NotImplementedException();
    }

    public void InsertVertex(Vertex v)
    {
        throw new System.NotImplementedException();
    }

    public Vertex Opposite(Vertex v, EdgeObject e)
    {
        return e.startPoint == v ? e.endPoint : e.startPoint;
    }

    public void RemoveEdge(EdgeObject e)
    {
        // This version only makes it not traversable as we never really remove edges
        e.element.traversable = false;
    }

    public void EnableEdge(EdgeObject e)
    {
        e.element.traversable = true;
    }

    public void RemoveVertex(Vertex v)
    {
        throw new System.NotImplementedException();
    }
        
    public void Replace(Vertex v, Vertex x)
    {
        throw new System.NotImplementedException();
    }

    public void Replace(EdgeObject e, EdgeObject x)
    {
        throw new System.NotImplementedException();
    }

    public Vertex[] Vertices()
    {
        throw new System.NotImplementedException();
    }
}
