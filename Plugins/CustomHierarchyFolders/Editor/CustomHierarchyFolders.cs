using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class CustomHierarchyFolders
{
    private static GameObject _selectObj;
    
    static CustomHierarchyFolders()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
    }

    private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectRect)
    {
        GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        if (obj == null)
        {
            _selectObj = null;
            return;
        }
        
        if (obj.transform == Selection.activeTransform)
        {
            _selectObj = obj;
        }

        if (obj.CompareTag("Folder"))
        {
            obj.transform.hideFlags = HideFlags.NotEditable | HideFlags.HideInInspector;
            obj.hideFlags = HideFlags.HideInInspector;

            var sv = SceneVisibilityManager.instance;
            sv.DisablePicking(obj, false);
            
            Color hoverColor = new Color(0.2666667f, 0.2666667f, 0.2666667f, 1f);
            Color normalColor = new Color(0.22f, 0.22f, 0.22f, 1f);
            Color selectionColor = new Color(0.17f, 0.36f, 0.52f, 1f);

            GUI.color = new Color(1, 1, 1, 1);
            var selectionRect = selectRect;
            selectionRect.xMin = selectRect.x + 1;
            selectionRect.xMax = selectionRect.xMin + 15f;
            
             if (obj.transform == Selection.activeTransform)
            {
                EditorGUI.DrawRect(selectionRect, selectionColor);
            }
            else if (selectRect.Contains(Event.current.mousePosition))
            {
                EditorGUI.DrawRect(selectionRect, hoverColor);
            }
            else
            {
                EditorGUI.DrawRect(selectionRect, normalColor);
            }

            selectionRect = selectRect;
            selectionRect.x = selectRect.xMin + 25;
            ChangeFolderIcon(obj, selectRect);
        }
    }
    
    private static void ChangeFolderIcon(GameObject obj, Rect selectRect)
    {
        string iconPath = obj.transform.childCount > 0 ? (ExpandedStatus(obj) ? "d_FolderOpened Icon" : "d_Folder Icon") : "d_FolderEmpty Icon";
        EditorGUI.LabelField(selectRect, EditorGUIUtility.IconContent(iconPath));
    }
    
    private static bool ExpandedStatus(GameObject obj)
    {
        var sceneHierarchyWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
        var getExpandedIDs = sceneHierarchyWindowType.GetMethod("GetExpandedIDs", BindingFlags.NonPublic | BindingFlags.Instance);
        var lastInteractedHierarchyWindow = sceneHierarchyWindowType.GetProperty("lastInteractedHierarchyWindow", BindingFlags.Public | BindingFlags.Static);
        if (lastInteractedHierarchyWindow == null)
            return false;

        if (getExpandedIDs == null) return false;
        
        var expandedIDs = getExpandedIDs.Invoke(lastInteractedHierarchyWindow.GetValue(null), null) as int[];
        return expandedIDs != null && expandedIDs.Contains(obj.GetInstanceID());

    }
}