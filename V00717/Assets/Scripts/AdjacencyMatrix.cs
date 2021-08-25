public class AdjacencyMatrix
{
    private EdgeObject[] edgeObjects;
    private EdgeObject[,] adjacencyMatrix;

    public AdjacencyMatrix(EdgeObject[] edgeObjects)
    {
        this.edgeObjects = edgeObjects;
        // Initialize and make the adjacency matrix
        adjacencyMatrix = new EdgeObject[4,4];
    }

    public Vertex[] EndVertices(EdgeObject e)
    {
        return new Vertex[2] { e.startPoint, e.endPoint };
    }

    public GameWaypoint Opposite(Vertex v, EdgeObject e)
    {
        return e.startPoint == v ? e.endPoint : e.startPoint;
    }

    public void AreAdjacent(Vertex v, Vertex w)
    {
        GameWaypoint gwp = (GameWaypoint)v;

        
    }

    public void Replace(Vertex v, Vertex x)
    {

    }

    public void Replace(EdgeObject e, EdgeObject x)
    {

    }

    public void InsertVertex(Vertex v)
    {

    }

    public void InsertEdge(Vertex v, Vertex w, EdgeObjectElement e)
    {

    }

    public void RemoveVertex(Vertex v)
    {

    }

    public void RemoveEdge(EdgeObject e)
    {

    }

    /// <summary>
    /// Iterable collection methods
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    //public EdgeObject[] IncidentEdges(Vertex v)
    //{

    //}

    //public GameWaypoint[] Vertices()
    //{

    //}

    //public EdgeObject[] Edges()
    //{

    //}
}
