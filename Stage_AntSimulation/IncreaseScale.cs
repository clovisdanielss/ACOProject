using Godot;
using System.Linq;

public partial class IncreaseScale : Button
{
	public override void _Pressed()
	{
		var parent = GetParent();
		var antAgents = parent.GetChildren().Where(c => c is AntAgent).Select(c => c as AntAgent);
		foreach (var ant in antAgents)
		{
			ant.Scale = new Vector2(ant.Scale.X + 0.1f,ant.Scale.Y + 0.1f);
		}
		this.EditText("Scale", v=>{v += 0.1f; return v;});
		var antFactory = parent.GetNode<AntFactory>("AntFactory");
		antFactory.MapScale += 0.1f;
		base._Pressed();
	}

}
