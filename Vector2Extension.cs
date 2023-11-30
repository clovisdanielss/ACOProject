using System;
using Godot;

public static class Vector2Extension{
    public static bool IsOffscreen(this Vector2 position, float width, float height)
	{
		return position.X < 0 || position.X > width || position.Y > height || position.Y < 0;
	}
    public static bool IntersectWith(this Vector2 position, Vertex v, double radius)
	{
		return Math.Pow(v.Position.X - position.X, 2) + Math.Pow(v.Position.Y - position.Y, 2) < Math.Pow(radius, 2);
	}

	public static bool IntersectWith(this Vector2 position, Vector2 v, double radius)
	{
		return Math.Pow(v.X - position.X, 2) + Math.Pow(v.Y - position.Y, 2) < Math.Pow(radius, 2);
	}
    public static Vector2 North(this Vector2 p, double dist)
	{
		return p + new Vector2(0, (float)-dist);
	}
	public static Vector2 NorthEast(this Vector2 p, double dist)
	{
		return p + new Vector2((float)dist, (float)-dist);
	}
	public static Vector2 East(this Vector2 p, double dist)
	{
		return p + new Vector2((float)dist, 0);
	}
	public static Vector2 SouthEast(this Vector2 p, double dist)
	{
		return p + new Vector2((float)dist, (float)dist);
	}
	public static Vector2 South(this Vector2 p, double dist)
	{
		return p + new Vector2(0, (float)dist);
	}
	public static Vector2 SouthWest(this Vector2 p, double dist)
	{
		return p + new Vector2((float)-dist, (float)dist);
	}
	public static Vector2 West(this Vector2 p, double dist)
	{
		return p + new Vector2((float)-dist, 0);
	}
	public static Vector2 NorthWest(this Vector2 p, double dist)
	{
		return p + new Vector2((float)-dist, (float)-dist);
	}

	public static bool IsGeometricAdjacent(this Vector2 p1, Vector2 p2, double dist){
		return p2 == p1.North(dist) || p2 == p1.NorthEast(dist) || p2 == p1.East(dist)
			|| p2 == p1.SouthEast(dist) || p2 == p1.South(dist) || p2 == p1.SouthWest(dist)
			|| p2 == p1.West(dist) || p2 == p1.NorthWest(dist);
	}
}