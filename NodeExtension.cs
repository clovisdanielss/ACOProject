using Godot;
using System;
using System.Linq;

public static class NodeExtension
{
    public static void EditText(this Node n, string path, Func<float, float> cb)
    {
        if (!n.GetTree().Root.GetChild(0).HasNode(path)) return;
        var textEditor = n.GetTree().Root.GetChild(0).GetNode<TextEdit>(path);
        var texts = textEditor.Text.Split(" ");
        if (float.TryParse(texts[^1], out float val))
        {
            texts[^1] = (cb(val)).ToString();
        }
        textEditor.Text = string.Join(" ", texts);
    }

    public static string GetParameterFromText(this Node n, string path)
    {
        if (!n.GetTree().Root.GetChild(0).HasNode(path)) return string.Empty;
        var textEditor = n.GetTree().Root.GetChild(0).GetNode<TextEdit>(path);
        var texts = textEditor.Text.Split(" ");
        return texts[^1];
    }

    public static void AppendText(this Node n, string path, string value)
    {
        if (!n.GetTree().Root.GetChild(0).HasNode(path)) return;
        var textEditor = n.GetTree().Root.GetChild(0).GetNode<TextEdit>(path);
        textEditor.Text = textEditor.Text + "\n" + value;
    }
}
