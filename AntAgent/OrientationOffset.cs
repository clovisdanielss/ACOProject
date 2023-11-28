using Godot;

public static class OrientationOffset
{
	public static Vector2 North(double dist)
	{
		return new Vector2(0, (float)-dist);
	}
	public static Vector2 NorthEast(double dist)
	{
		return new Vector2((float)dist, (float)-dist);
	}
	public static Vector2 East(double dist)
	{
		return new Vector2((float)dist, 0);
	}
	public static Vector2 SouthEast(double dist)
	{
		return new Vector2((float)dist, (float)dist);
	}
	public static Vector2 South(double dist)
	{
		return new Vector2(0, (float)dist);
	}
	public static Vector2 SouthWest(double dist)
	{
		return new Vector2((float)-dist, (float)dist);
	}
	public static Vector2 West(double dist)
	{
		return new Vector2((float)-dist, 0);
	}
	public static Vector2 NorthWest(double dist)
	{
		return new Vector2((float)-dist, (float)-dist);
	}
}
