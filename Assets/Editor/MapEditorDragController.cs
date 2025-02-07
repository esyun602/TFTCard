using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class MapEditorDragController
{
    static MapEditorDragController()
    {
        Debug.Log("MapEditor Initialized");
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;
        
        if (e.type != EventType.DragUpdated && e.type != EventType.DragPerform)
            return;
        
        Object[] draggedObjects = DragAndDrop.objectReferences;
        if (draggedObjects == null || draggedObjects.Length == 0)
            return;

        var draggedObj = draggedObjects[0];
        if (draggedObj is MapData md)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Link;

            if (e.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();

                GameObject go = md.InstantiateMapForMapEditor();
                
                Undo.RegisterCreatedObjectUndo(go, "Create object from ScriptableObject");
            }
            
            e.Use();
        }
    }
}