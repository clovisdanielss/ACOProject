using Godot;
using System.Linq;
public partial class IncreaseAlpha : Button
{
	public override void _Pressed()
	{
		var parent = GetParent();
		var antAgents = parent.GetChildren().Where(c => c is AntAgent).Select(c => c as AntAgent);
		foreach (var ant in antAgents)
		{
			ant.Alpha++;
		}
		this.EditText("Alpha", v=>++v);
		var antFactory = parent.GetNode<AntFactory>("AntFactory");
		antFactory.Alpha++;
		base._Pressed();
	}

}
