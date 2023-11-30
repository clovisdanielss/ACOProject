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
			if(ant.Alpha - 1 >= 0){
				ant.Alpha --;
			}
		}
		this.EditText("Alpha", v=>{--v; return v < 0 ? 0 : v;});
		var antFactory = parent.GetNode<AntFactory>("AntFactory");
		antFactory.Alpha--;
		base._Pressed();
	}
}
