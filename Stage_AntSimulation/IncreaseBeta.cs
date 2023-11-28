using Godot;
using System.Linq;

public partial class IncreaseBeta : Button
{
	public override void _Pressed()
	{
		var parent = GetParent();
		var antAgents = parent.GetChildren().Where(c => c is AntAgent).Select(c => c as AntAgent);
		foreach(var ant in antAgents){
			ant.QualityImportance --;
		}
		this.EditText("Beta", v=>++v);
		var antFactory = parent.GetNode<NewAnt>("AntFactory");
        antFactory.Beta++;
		base._Pressed();
	}
}
