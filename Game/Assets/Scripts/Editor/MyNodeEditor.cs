using Staple;
using Staple.Editor;
using System;

public class MyNodeEditor : EditorWindow, INodeUIObserver
{
    public NodeUI nodeUI;

    [MenuItem("Editor/My Node Editor")]
    public static void Present()
    {
        GetWindow<MyNodeEditor>();
    }

    public MyNodeEditor()
    {
        nodeUI = new(this)
        {
            showMinimap = true
        };
    }

    public override void OnGUI()
    {
        nodeUI?.DoLayout();
    }

    public bool ValidateConnection(NodeUI nodeUI, NodeUI.NodeSocket from, NodeUI.NodeSocket to)
    {
        if (to.Name != "Input 2")
        {
            return false;
        }

        return true;
    }

    public (bool, Action) OnNodeClick(NodeUI nodeUI, NodeUI.Node node, MouseButton button)
    {
        switch(button)
        {
            case MouseButton.Right:

                return (true, () =>
                {
                    EditorGUI.MenuItem("Delete", "Delete.Node", () =>
                    {
                        nodeUI.DeleteNode(node);
                    });
                });
        }

        return (false, null);
    }

    public (bool, Action) OnLinkClick(NodeUI nodeUI, NodeUI.NodeSocket from, NodeUI.NodeSocket to, MouseButton button)
    {
        switch(button)
        {
            case MouseButton.Right:

                return (true, () =>
                {
                    EditorGUI.MenuItem("Delete", "Delete.Link", () =>
                    {
                        nodeUI.DeleteLink(from, to);
                    });
                });
        }

        return (false, null);
    }

    public (bool, Action) OnWorkspaceClick(NodeUI nodeUI, MouseButton button)
    {
        switch(button)
        {
            case MouseButton.Right:
                return (true, () =>
                {
                    EditorGUI.MenuItem("Create Node", "Create.Node", () =>
                    {
                        nodeUI.CreateNode("New Node",
                            [
                                new NodeUI.Connector("Input 1", NodeUI.PinShape.Circle),
                        new NodeUI.Connector("Input 2", NodeUI.PinShape.Quad)
                            ],
                            [new NodeUI.Connector("Output", NodeUI.PinShape.CircleFilled)],
                            (node) =>
                            {
                                EditorGUI.Label("This is my node from My Node Editor");
                            });
                    });
                });
        }

        return (false, null);
    }
}
