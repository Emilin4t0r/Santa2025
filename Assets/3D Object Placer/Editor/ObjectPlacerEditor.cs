using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static ObjectPlacer;

[CustomEditor(typeof(ObjectPlacer))]
public class ObjectPlacerEditor : Editor
{
    SerializedProperty objectToBePlaced;
    SerializedProperty sizeVariation;
    SerializedProperty frequency;
    SerializedProperty brushSize;
    SerializedProperty editing;
    SerializedProperty randRot;
    SerializedProperty baseGameObject;

    public void OnEnable()
    {
        objectToBePlaced = serializedObject.FindProperty("objectToBePlaced");
        sizeVariation = serializedObject.FindProperty("sizeVariation");
        frequency = serializedObject.FindProperty("frequency");
        brushSize = serializedObject.FindProperty("brushSize");
        editing = serializedObject.FindProperty("editing");
        randRot = serializedObject.FindProperty("randRot");
        baseGameObject = serializedObject.FindProperty("baseGameObject");
    }

    public void OnSceneGUI()
    {
        if (editing.boolValue)
        {
            //Ensuring the selection doesn't escape the paintable terrain object
            int controlId = GUIUtility.GetControlID(FocusType.Passive);
            if (Event.current.button == 0)
            {
                switch (Event.current.type)
                {
                    case EventType.MouseDown:
                        GUIUtility.hotControl = controlId;
                        Paint(false);
                        Event.current.Use();
                        break;
                    case EventType.MouseDrag:
                        GUIUtility.hotControl = controlId;
                        //Paint new trees when dragging with mouse
                        Paint(true);
                        Event.current.Use();
                        break;
                }
            }
        }
    }

    Vector3 lastPaintPos;
    bool hasParentObj = false;
    GameObject objs = null;
    void Paint(bool hold)
    {
        bool mouseInTerrain = false;
        if (editing.boolValue)
        {
            GameObject baseGO = baseGameObject.objectReferenceValue as GameObject;

            if (baseGO.TryGetComponent<MeshCollider>(out MeshCollider meshCollider))
            {
                //Get mouse position in world 
                Vector3 mworldPosition = Vector3.zero;
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hit;
                if (meshCollider.Raycast(ray, out hit, 1000))
                {
                    mworldPosition = hit.point;
                    mouseInTerrain = true;
                }
                else
                {
                    mouseInTerrain = false;
                }

                if (!hasParentObj)
                {
                    if (!GameObject.Find("ObjParent"))
                    {
                        objs = new GameObject();
                        objs.transform.parent = meshCollider.gameObject.transform;
                        objs.name = "ObjParent";
                        hasParentObj = true;
                    }
                    else
                    {
                        objs = GameObject.Find("ObjParent");
                        hasParentObj = true;
                    }
                }
                if (mouseInTerrain)
                {
                    if (hold)
                    {
                        if (Vector3.Distance(mworldPosition, lastPaintPos) >= 20 - frequency.intValue)
                        {
                            //Instantiate a new tree object on the mouse position
                            SpawnTrees(mworldPosition, meshCollider.gameObject, 1);
                        }
                    }
                    else
                    {
                        SpawnTrees(mworldPosition, meshCollider.gameObject, frequency.intValue / 2);
                    }
                }                
            }            
            else
            {
                //Get mouse position in world 
                TerrainCollider terrainCollider = Terrain.activeTerrain.GetComponent<TerrainCollider>();
                Vector3 mworldPosition = Vector3.zero;
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hit;
                if (terrainCollider.Raycast(ray, out hit, 1000))
                {
                    mworldPosition = hit.point;
                    mouseInTerrain = true;
                }
                else
                {
                    mouseInTerrain = false;
                }

                if (!hasParentObj)
                {
                    if (!GameObject.Find("ObjParent"))
                    {
                        objs = new GameObject();
                        objs.transform.parent = terrainCollider.gameObject.transform;
                        objs.name = "ObjParent";
                        hasParentObj = true;
                    }
                    else
                    {
                        objs = GameObject.Find("ObjParent");
                        hasParentObj = true;
                    }
                }
                if (mouseInTerrain)
                {
                    if (hold)
                    {
                        if (Vector3.Distance(mworldPosition, lastPaintPos) >= 20 - frequency.intValue)
                        {
                            //Instantiate a new tree object on the mouse position
                            SpawnTrees(mworldPosition, terrainCollider.gameObject, 1);
                        }
                    }
                    else
                    {
                        SpawnTrees(mworldPosition, terrainCollider.gameObject, frequency.intValue / 2);
                    }
                }
            }
        }
    }

    void SpawnTrees(Vector3 mousePos, GameObject paintableObj, int amt)
    {
        for (int i = 0; i < amt; i++)
        {
            //Get a random position inside a circle
            Vector2 randPos = Random.insideUnitCircle * brushSize.intValue;
            //Get position of mouse and add randpos coordinates to it
            Vector3 raySpawnPos = new Vector3(mousePos.x + randPos.x, mousePos.y + 5, mousePos.z + randPos.y);
            RaycastHit hit;
            //Spawn a tree on the new random position
            if (Physics.Raycast(raySpawnPos, Vector3.down, out hit, 100f) && hit.transform.gameObject == paintableObj)
            {
                if (objectToBePlaced.objectReferenceValue != null)
                {
                    GameObject tree = Instantiate(objectToBePlaced.objectReferenceValue as GameObject, hit.point, Quaternion.identity, objs.gameObject.transform);
                    float randScale = Random.Range(tree.transform.localScale.x, tree.transform.localScale.x * sizeVariation.floatValue);
                    tree.transform.localScale = new Vector3(randScale, randScale, randScale);
                    if (randRot.boolValue)
                    {
                        tree.transform.localEulerAngles = new Vector3(0, Random.Range(0, 360f), 0);
                    }
                    lastPaintPos = tree.transform.position;
                }
                else
                {
                    Debug.LogWarning("You have not set the 'Object To Be Placed' -attribute!");
                }
            }
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(objectToBePlaced, new GUIContent("Object To Be Placed"));
        EditorGUILayout.PropertyField(baseGameObject, new GUIContent("Base Game Object"));
        EditorGUILayout.PropertyField(sizeVariation, new GUIContent("Object Size Variation"));
        EditorGUILayout.PropertyField(randRot, new GUIContent("Randomize Rotation"));
        EditorGUILayout.IntSlider(frequency, 1, 50, new GUIContent("Frequency"));
        EditorGUILayout.IntSlider(brushSize, 1, 100, new GUIContent("Brush Size"));
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("TOGGLE THIS TO START PAINTING");
        EditorGUILayout.PropertyField(editing, new GUIContent("Editing"));
        serializedObject.ApplyModifiedProperties();
    }
}
