using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ObjectPlacer : MonoBehaviour {
    public GameObject objectToBePlaced = null;
    public float sizeVariation = 1;
    public int frequency = 1;
    public int brushSize = 2;
    public bool editing = false;
    public bool randRot = false;
    public GameObject baseGameObject = null;

    TerrainCollider terrainCollider;
    MeshCollider meshCollider;
    Vector3 worldPosition = Vector3.zero;
    Ray ray;

    /*private void OnDrawGizmosSelected()
    {
        ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;

        try
        {
            meshCollider = transform.GetComponent<MeshCollider>();
            if (meshCollider.Raycast(ray, out hit, 1000))
            {
                worldPosition = hit.point;
            }
        }
        catch
        {
            terrainCollider = Terrain.activeTerrain.GetComponent<TerrainCollider>();
            if (terrainCollider.Raycast(ray, out hit, 1000))
            {
                worldPosition = hit.point;
            }            
        }

        if (editing) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(worldPosition, brushSize);
        }
    }*/
}
