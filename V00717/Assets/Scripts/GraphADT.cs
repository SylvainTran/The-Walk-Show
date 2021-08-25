public interface GraphADT {
    public Vertex[] EndVertices(EdgeObject e);

    public Vertex Opposite(Vertex v, EdgeObject e);

    public void AreAdjacent(Vertex v, Vertex w);

    public void Replace(Vertex v, Vertex x);

    public void Replace(EdgeObject e, EdgeObject x);

    public void InsertVertex(Vertex v);

    public void InsertEdge(Vertex v, Vertex w, EdgeObjectElement e);

    public void RemoveVertex(Vertex v);

    public void RemoveEdge(EdgeObject e);

    public EdgeObject[] IncidentEdges(Vertex v);

    public Vertex[] Vertices();

    public EdgeObject[] Edges();
}
