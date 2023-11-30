using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// Pheromone will work like Edges. 
/// Since there is no specified graph, we will have diferents values of pheromone for different directions. 
/// It will work simmilarly as the ant sensors, N, NE, E, SE, S, SW, W, NW.
/// </summary>
public partial class Edge
{
	public Vertex V { get; set; }
	public Vertex U { get; set; }
	private double Rho = .7;
	private double Tau { get; set; }
	private double Eta { get; set; }
	public double Quality => Eta;
	public double Pheromone => Tau;

	public Edge(Vertex u, Vertex v, double ph)
	{
		SetEdge(u,v);
		Tau = ph;
		Eta = 1/V.Position.DistanceTo(U.Position);
	}

	public void SetEdge(Vertex u, Vertex v)
	{
		if(u == v) throw new InvalidOperationException("Do not autolink any vertex");
		this.U = u;
		this.V = v;
		this.U.Edges.Add(this);
		this.V.Edges.Add(this);
	}
	public void Update(double deltaTau)
	{
		Tau = (1 - Rho) * Tau + Rho * deltaTau;
	}

	public Vertex GetOtherVertex(Vertex v){
		if(V == v) return U;
		if(U == v) return V;
		return null;
	}

	public bool IsThisEdge(Vertex v, Vertex u){
		return (V == v && U == u) || (V == u && U == v);
	}

	public Vertex GetVertexAt(Vector2 pos){
		return pos == V.Position ? V : pos == U.Position ? U : null;
	}

    public override bool Equals(object obj)
    {
        if(obj is Edge edge){
			return IsThisEdge(edge.U, edge.V);
		}
		return false;
    }
}
