using UnityEngine;
using System.Collections.Generic;

public enum NodeType
{
    Choice = 0,
    Sentence = 1
}

[System.Serializable]
public class Node
{
    public string text;
    public Rect Position;
    public int id;
    public NodeType Type;
    public bool isEnding;

    public List<int> ConnectedID;

    public Node(string text, Rect Position, NodeType Type, int id)
    {
        this.text = text;
        this.Position = Position;
        this.id = id;
        this.Type = Type;
        ConnectedID = new List<int>();
    }

    public Rect Top()
    {
        return new Rect(Position.x + Position.width / 2 - 40, Position.y - 25, 80, 30);
    }

    public Rect Bottom()
    {
        return new Rect(Position.x + Position.width / 2 - 40, Position.y + Position.height-5, 80, 30);
    }

    public void Scroll(Vector2 delta)
    {
        Position.position += delta;
    }
}

[System.Serializable]
public class Dialogue
{
    public List<Node> Nodes;
    public Dialogue()
    {
        Nodes = new List<Node>();
    }
}