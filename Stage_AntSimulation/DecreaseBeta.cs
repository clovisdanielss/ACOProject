using Godot;
using System.Linq;

public partial class DecreaseBeta : Button
{
    public override void _Pressed()
	{
		var parent = GetParent();
		var antAgents = parent.GetChildren().Where(c => c is AntAgent).Select(c => c as AntAgent);
		foreach(var ant in antAgents){
			if(ant.QualityImportance - 1 >= 0)
				ant.QualityImportance --;
		}
		this.EditText("Beta", v=>{--v; return v < 0 ? 0 : v;});
		var antFactory = parent.GetNode<AntFactory>("AntFactory");
        antFactory.Beta--;
		base._Pressed();
	}
}
