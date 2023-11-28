using Godot;
using System;
using System.Linq;
public partial class DecreaseAlpha : Button
{
	public override void _Pressed()
	{
		var parent = GetParent();
		var antAgents = parent.GetChildren().Where(c => c is AntAgent).Select(c => c as AntAgent);
		foreach(var ant in antAgents){
			if(ant.PheromoneImportance - 1 >= 0){
				ant.PheromoneImportance --;
			}
		}
		this.EditText("Alpha", v=>{--v; return v < 0 ? 0 : v;});
		var antFactory = parent.GetNode<NewAnt>("AntFactory");
		antFactory.Alpha--;
		base._Pressed();
	}
}
