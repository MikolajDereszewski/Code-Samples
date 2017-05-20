using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class DialogueEditor : EditorWindow
{
    Dialogue DialogueTree;
    DialogueNPC EditedNPC;
    
    public Vector2 Connection;
    private Color SentenceConnectionColor = Color.black, AnswerConnectionColor = new Color(1F, 0.5F, 0F, 1F);
    private Vector2 Scrolling;
    private Vector3 GridOffset;

    [MenuItem("Window/Dialogue Editor")]
    static void ShowEditor()
    {
        DialogueEditor editor = EditorWindow.GetWindow<DialogueEditor>();
        editor.DialogueTree = new Dialogue();
        editor.Scrolling = new Vector2(0, 0);
    }

    void CreateNode(NodeType Type)
    {
        if(Type == NodeType.Sentence)
            DialogueTree.Nodes.Add(new Node("Sample Text", new Rect(Scrolling.x+10, Scrolling.y+120, 200, 150), Type, DialogueTree.Nodes.Count));
        if (Type == NodeType.Choice)
            DialogueTree.Nodes.Add(new Node("Sample Answer", new Rect(Scrolling.x + 120, Scrolling.y + 120, 200, 100), Type, DialogueTree.Nodes.Count));
    }

    void DrawSingleNode(int id)
    {
        if (id >= DialogueTree.Nodes.Count)
            return;
        if (DialogueTree.Nodes[id] == null)
            return;
        string name = (DialogueTree.Nodes[id].Type == NodeType.Choice) ? "Choice" :
                      (id == 0) ? "Starting Sentence" : "Sentence";
        GUI.backgroundColor = (name == "Starting Sentence") ? Color.green :
                              (name == "Choice") ? new Color(1F, 0.5F, 0F, 1F) : 
                              (DialogueTree.Nodes[id].isEnding) ? Color.red : Color.gray;
        DialogueTree.Nodes[id].Position = GUI.Window(id, DialogueTree.Nodes[id].Position, DrawNodeWindow, name);
        foreach (int i in DialogueTree.Nodes[id].ConnectedID)
            DrawNodeCurve(DialogueTree.Nodes[id].Bottom(), DialogueTree.Nodes[i].Top(), DialogueTree.Nodes[i].Type);
        CheckNodeProperity(id);
        CheckConnection(id);
        GUI.backgroundColor = new Color(0.7F, 0.7F, 0.7F, 1);
    }

    void ResetConnection()
    {
        Connection = new Vector2(-1, -1);
    }

    void ResetConnectedAnswers(int id)
    {
        DialogueTree.Nodes[id].ConnectedID = new List<int>();
    }

    void ResetAllConnections(int id)
    {
        foreach (Node N in DialogueTree.Nodes)
        {
            foreach (int i in N.ConnectedID)
            {
                if (i == id)
                {
                    ResetConnectedAnswers(N.id);
                    break;
                }
            }
        }
        ResetConnectedAnswers(id);
    }

    void CheckNodeProperity(int id)
    {
        if (DialogueTree.Nodes[id].ConnectedID.Count <= 1)
            return;
        foreach(int i in DialogueTree.Nodes[id].ConnectedID)
        {
            if (DialogueTree.Nodes[i].Type == NodeType.Sentence)
            {
                ResetConnectedAnswers(id);
                break;
            } 
        }
    }

    void CheckConnection(int id)
    {
        GUI.backgroundColor = new Color(0.7F, 0.7F, 0.7F, 1);
        if (GUI.Button(DialogueTree.Nodes[id].Top(), "Origin"))
        {
            if (Connection.x == id)
                ResetConnection();
            else if (Connection.x == -1)
                Connection.x = id;
            if(Connection.y != -1)
            {
                if(DialogueTree.Nodes[(int)Connection.y].Type == NodeType.Sentence)
                {
                    if (DialogueTree.Nodes[id].Type == NodeType.Choice)
                    {
                        if (DialogueTree.Nodes[(int)Connection.y].ConnectedID.Count > 0)
                            if (DialogueTree.Nodes[DialogueTree.Nodes[(int)Connection.y].ConnectedID[0]].Type == NodeType.Sentence)
                                ResetConnectedAnswers((int)Connection.y);
                        DialogueTree.Nodes[(int)Connection.y].ConnectedID.Add(id);
                    }
                        
                    else
                    {
                        ResetConnectedAnswers((int)Connection.y);
                        DialogueTree.Nodes[(int)Connection.y].ConnectedID.Add(id);
                    }
                    ResetConnection();
                }
                else
                {
                    if (DialogueTree.Nodes[id].Type == NodeType.Sentence)
                    {
                        ResetConnectedAnswers((int)Connection.y);
                        DialogueTree.Nodes[(int)Connection.y].ConnectedID.Add(id);
                    }
                    ResetConnection();
                }
            }
        }
        if (GUI.Button(DialogueTree.Nodes[id].Bottom(), "Next Node"))
        {
            if (Connection.y == id)
                ResetConnection();
            else if (Connection.y == -1)
                Connection.y = id;
            if (Connection.x != -1)
            {
                if (DialogueTree.Nodes[id].Type == NodeType.Sentence)
                {
                    if (DialogueTree.Nodes[(int)Connection.x].Type == NodeType.Choice)
                    {
                        if (DialogueTree.Nodes[id].ConnectedID.Count > 0)
                            if (DialogueTree.Nodes[DialogueTree.Nodes[id].ConnectedID[0]].Type == NodeType.Sentence)
                                ResetConnectedAnswers(id);
                        DialogueTree.Nodes[id].ConnectedID.Add((int)Connection.x);
                    }
                    else
                    {
                        ResetConnectedAnswers(id);
                        DialogueTree.Nodes[id].ConnectedID.Add((int)Connection.x);
                    }
                    ResetConnection();
                }
                else
                {
                    if (DialogueTree.Nodes[(int)Connection.x].Type == NodeType.Sentence)
                    {
                        ResetConnectedAnswers(id);
                        DialogueTree.Nodes[id].ConnectedID.Add((int)Connection.x);
                    }
                    ResetConnection();
                }
            }
        }
    }

    void DrawNodes()
    {
        BeginWindows();
        for (int i = 0; i < DialogueTree.Nodes.Count; i++)
            DrawSingleNode(i);
        EndWindows();
    }

    int DrawInterface()
    {
        GUI.backgroundColor = new Color(0.7F, 0.7F, 0.7F, 1);
        GUILayout.Box(GUIContent.none, GUILayout.Height(100), GUILayout.Width(600));
        Rect AddButton1 = new Rect(10, 10, 150, 50);
        if (GUI.Button(AddButton1, "New Sentence Node"))
            return 1;

        Rect AddButton2 = new Rect(170, 10, 150, 50);
        if (GUI.Button(AddButton2, "New Answer Node"))
            return 2;

        Rect DeleteAll = new Rect(330, 10, 100, 50);
        if (GUI.Button(DeleteAll, "Delete All"))
            return 3;
        return 0;
    }

    void DrawGrid()
    {
        int spacing = 20;
        int rows = (int)position.height / spacing;
        int cols = (int)position.width / spacing;
        Handles.BeginGUI();
        Handles.color = Color.gray;
        for(int i = -2; i < rows+2; i++)
        {
            Handles.DrawLine(new Vector3(GridOffset.x-20, GridOffset.y + i * spacing), new Vector3(GridOffset.x+position.width+20, GridOffset.y + i * spacing));
        }
        for (int i = -2; i < cols+2; i++)
        {
            Handles.DrawLine(new Vector3(GridOffset.x + i * spacing, GridOffset.y-20), new Vector3(GridOffset.x + i * spacing, GridOffset.y+position.height+20));
        }
    }

    void DrawConnectingLine()
    {
        GUI.backgroundColor = new Color(0.7F, 0.7F, 0.7F, 1);
        if (Connection.x == -1 && Connection.y == -1)
            return;
        if (Connection.x != -1)
            DrawNodeCurve(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0), DialogueTree.Nodes[(int)Connection.x].Top(), NodeType.Sentence);
        if (Connection.y != -1)
            DrawNodeCurve(DialogueTree.Nodes[(int)Connection.y].Bottom(), new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0), NodeType.Sentence);
        Repaint();
    }

    void OnGUI()
    {
        DrawGrid();
        ProcessScrolling(Event.current);
        if (DialogueTree == null)
            DialogueTree = new Dialogue();
        if (DialogueTree.Nodes.Count > 0)
        {
            DrawConnectingLine();
            DrawNodes();
        }
        switch (DrawInterface())
        {
            case 0:
                break;
            case 1:
                CreateNode(NodeType.Sentence);
                break;
            case 2:
                CreateNode(NodeType.Choice);
                break;
            case 3:
                DialogueTree = new Dialogue();
                break;
        }
        if (EditedNPC != null)
            EditedNPC.SaveChanges(DialogueTree);
        if (GUI.changed) Repaint();
    }

    private void ProcessScrolling(Event e)
    {
        Scrolling = Vector2.zero;
        if (e.type == EventType.MouseDrag)
        {
            if (e.button == 2)
            {
                Scrolling = e.delta;
                for (int i = 0; i < DialogueTree.Nodes.Count; i++)
                {
                    if(DialogueTree.Nodes[i].id != -1)
                        DialogueTree.Nodes[i].Scroll(Scrolling);
                }
                GridOffset += new Vector3(e.delta.x, e.delta.y, 0);
                if (Mathf.Abs(GridOffset.x) >= 20)
                    GridOffset.x = 0;
                if (Mathf.Abs(GridOffset.y) >= 20)
                    GridOffset.y = 0;
                GUI.changed = true;
            }
        }
    }

    void ChangeNodeType(int id)
    {
        NodeType before = DialogueTree.Nodes[id].Type;
        DialogueTree.Nodes[id].Type = (NodeType)EditorGUILayout.EnumPopup(DialogueTree.Nodes[id].Type);
        if (before == DialogueTree.Nodes[id].Type)
            return;
        if (DialogueTree.Nodes[id].Type == NodeType.Sentence)
            DialogueTree.Nodes[id].Position = new Rect(DialogueTree.Nodes[id].Position.x, DialogueTree.Nodes[id].Position.y, 200, 150);
        else
            DialogueTree.Nodes[id].Position = new Rect(DialogueTree.Nodes[id].Position.x, DialogueTree.Nodes[id].Position.y, 200, 100);
    }
    
    void DrawNodeWindow(int id)
    {
        if (DialogueTree.Nodes.Count == 0)
            return;
        if (id != 0)
            ChangeNodeType(id);
        DialogueTree.Nodes[id].text = EditorGUILayout.TextField(DialogueTree.Nodes[id].text);
        if (EditorGUILayout.BeginFadeGroup(DialogueTree.Nodes[id].Type.GetHashCode()))
        {
            DialogueTree.Nodes[id].isEnding = EditorGUILayout.ToggleLeft("Is ending?", DialogueTree.Nodes[id].isEnding);
            if (id != 0)
            {
                if (GUILayout.Button("Set as starting sentence"))
                {
                    ResetAllConnections(id);
                    ResetAllConnections(0);
                    Node Temp = DialogueTree.Nodes[id];
                    DialogueTree.Nodes[id] = DialogueTree.Nodes[0];
                    DialogueTree.Nodes[0] = Temp;
                }
            }
        }
        else
            DialogueTree.Nodes[id].isEnding = false;
        if (GUILayout.Button("Reset Node"))
        {
            ResetAllConnections(id);
        }
        if (GUILayout.Button("Delete Node"))
        {
            ResetAllConnections(id);
            DialogueTree.Nodes[id] = new Node("", new Rect(-20, -20, 0, 0), NodeType.Sentence, -1);
        }
        GUI.DragWindow();
    }
    
    void DrawNodeCurve(Rect start, Rect end, NodeType Type)
    {
        Vector3 startPos = new Vector3(start.x + start.width / 2, start.y + start.height / 2, 0);
        Vector3 endPos = new Vector3(end.x + end.width / 2, end.y + end.height / 2, 0);
        Vector3 startTan = startPos + Vector3.down * -90;
        Vector3 endTan = endPos + Vector3.down * 90;
        Color colorUsed = (Type == NodeType.Choice) ? AnswerConnectionColor : SentenceConnectionColor;
        GUI.backgroundColor = new Color(1F, 1F, 1F, 1);
        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.gray, null, 15);
        Handles.DrawBezier(startPos, endPos, startTan, endTan, new Color(colorUsed.r, colorUsed.g, colorUsed.b, 1), null, 5);
        GUI.backgroundColor = new Color(0.7F, 0.7F, 0.7F, 1);
    }

    public void LoadDialogueTree(DialogueNPC NPC)
    {
        EditedNPC = NPC;
        DialogueTree = EditedNPC.NPC_Dialogue;
    }
}