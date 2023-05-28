using UnityEditor;
using UnityEngine;

public abstract class FolderCreator
{
    [MenuItem("GameObject/Create Folder", false, -1)]
    private static void CreateEmptyFolder()
    {
        GameObject obj = new GameObject();
        Undo.RegisterCreatedObjectUndo(obj, obj.name);
        if (Selection.activeTransform != null)
        {
            Object[] selectedObjects = Selection.objects;
            GameObject selectedGameObject = selectedObjects[0] as GameObject;

            if (selectedGameObject != null) selectedGameObject.transform.SetParent(obj.transform);
        }
        
        obj.name = "New Folder";
        EditorUtility.SetDirty(obj);
        Selection.activeGameObject = obj;
        AddTag(obj, "Folder");
    }

    private static void AddTag(GameObject obj, string tag)
    {
        Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if (asset is {Length: > 0})
        {
            SerializedObject so = new SerializedObject(asset[0]);
            SerializedProperty tags = so.FindProperty("tags");

            for (int i = 0; i < tags.arraySize; ++i)
            {
                if (tags.GetArrayElementAtIndex(i).stringValue == tag)
                {
                    obj.tag = tag;
                    return;
                }
            }

            tags.InsertArrayElementAtIndex(0);
            tags.GetArrayElementAtIndex(0).stringValue = tag;
            so.ApplyModifiedProperties();
            so.Update();

            obj.tag = tag;
        }
    }
}