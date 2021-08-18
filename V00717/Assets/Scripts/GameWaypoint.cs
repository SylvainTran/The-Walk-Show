using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWaypoint : Vertex
{
    public WaypointEvent waypointEvent;
    public WaypointItem WaypointItem;

    /// <summary>
    /// The owner of this waypoint at start
    /// </summary>
    public CharacterModel owner;

    public override Vertex Validate(Vertex p)
    {
        if (!(p.GetType() != this.GetType()))
        {
            Debug.Log("Wrong vertex type!");
            return null;
        }
        GameWaypoint vertex = (GameWaypoint)p;
        return vertex;
    }
}
