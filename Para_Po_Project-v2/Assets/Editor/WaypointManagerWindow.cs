using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WaypointManagerWindow : EditorWindow
{
    [MenuItem("Tools/Waypoint Editor")]
    public static void Open()
    {
        GetWindow<WaypointManagerWindow>();
    }

    public Transform waypointRoot;  // Root for general waypoints
    public GameObject waypointPrefab;  // Prefab or template for pedestrian waypoints

    void OnGUI()
    {
        // Create a serialized object to handle the UI for waypoint roots
        SerializedObject obj = new SerializedObject(this);

        // Draw fields for both waypoint roots
        EditorGUILayout.PropertyField(obj.FindProperty("waypointRoot"));
        EditorGUILayout.PropertyField(obj.FindProperty("waypointPrefab"));

        // Handle cases where the roots are null
        if (waypointRoot == null)
        {
            EditorGUILayout.HelpBox("Waypoint Root must be assigned. Please assign a root transform.", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.BeginVertical("box");
            DrawButtons();  // Draw the buttons for creating, removing, and branching waypoints
            EditorGUILayout.EndVertical();
        }

        // Apply changes to serialized properties
        obj.ApplyModifiedProperties();
    }

    void DrawButtons()
    {
        // Buttons for various waypoint operations
        if (GUILayout.Button("Add Branch Waypoint"))
        {
            CreateBranch();
        }
        if (GUILayout.Button("Create Waypoint"))
        {
            CreateWaypoint();
        }

        // Only show these options if a Waypoint is selected in the hierarchy
        if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Waypoint>())
        {
            if (GUILayout.Button("Create Waypoint Before"))
            {
                CreateWaypointBefore();
            }
            if (GUILayout.Button("Create Waypoint After"))
            {
                CreateWaypointAfter();
            }
            if (GUILayout.Button("Remove Waypoint"))
            {
                RemoveWaypoint();
            }
        }
    }

    void CreateWaypoint()
    {
        // Check if pedestrianWaypointRoot is assigned
        if (waypointPrefab == null)
        {
            Debug.LogError("Pedestrian Waypoint Root is not assigned!");
            return;
        }

        // Instantiate a new waypoint based on the pedestrianWaypointRoot prefab
        GameObject waypointObject = Instantiate(waypointPrefab);
        waypointObject.name = "Waypoint " + waypointRoot.childCount;
        waypointObject.transform.SetParent(waypointRoot, false); // Set the waypoint under the waypointRoot

        Waypoint waypoint = waypointObject.GetComponent<Waypoint>();

        if (waypointRoot.childCount > 1)
        {
            waypoint.previousWaypoint = waypointRoot.GetChild(waypointRoot.childCount - 2).GetComponent<Waypoint>();
            waypoint.previousWaypoint.nextWaypoint = waypoint;

            waypoint.transform.position = waypoint.previousWaypoint.transform.position;
            waypoint.transform.forward = waypoint.previousWaypoint.transform.forward;
        }

        Selection.activeGameObject = waypoint.gameObject;
    }

    void CreateWaypointBefore()
    {
        // Create a waypoint before the currently selected waypoint
        if (waypointPrefab == null)
        {
            Debug.LogError("Pedestrian Waypoint Root is not assigned!");
            return;
        }

        GameObject waypointObject = Instantiate(waypointPrefab);
        waypointObject.name = "Waypoint " + waypointRoot.childCount;
        waypointObject.transform.SetParent(waypointRoot, false);

        Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();
        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        waypointObject.transform.position = selectedWaypoint.transform.position;
        waypointObject.transform.forward = selectedWaypoint.transform.forward;

        if (selectedWaypoint.previousWaypoint != null)
        {
            newWaypoint.previousWaypoint = selectedWaypoint.previousWaypoint;
            selectedWaypoint.previousWaypoint.nextWaypoint = newWaypoint;
        }

        newWaypoint.nextWaypoint = selectedWaypoint;
        selectedWaypoint.previousWaypoint = newWaypoint;

        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());

        Selection.activeGameObject = newWaypoint.gameObject;
    }

    void CreateWaypointAfter()
    {
        // Create a waypoint after the currently selected waypoint
        if (waypointPrefab == null)
        {
            Debug.LogError("Pedestrian Waypoint Root is not assigned!");
            return;
        }

        GameObject waypointObject = Instantiate(waypointPrefab);
        waypointObject.name = "Waypoint " + waypointRoot.childCount;
        waypointObject.transform.SetParent(waypointRoot, false);

        Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();
        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        waypointObject.transform.position = selectedWaypoint.transform.position;
        waypointObject.transform.forward = selectedWaypoint.transform.forward;

        if (selectedWaypoint.nextWaypoint != null)
        {
            selectedWaypoint.nextWaypoint.previousWaypoint = newWaypoint;
            newWaypoint.nextWaypoint = selectedWaypoint.nextWaypoint;
        }

        selectedWaypoint.nextWaypoint = newWaypoint;

        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());

        Selection.activeGameObject = newWaypoint.gameObject;
    }

    void RemoveWaypoint()
    {
        // Remove the currently selected waypoint
        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        if (selectedWaypoint.nextWaypoint != null)
        {
            selectedWaypoint.nextWaypoint.previousWaypoint = selectedWaypoint.previousWaypoint;
        }
        if (selectedWaypoint.previousWaypoint != null)
        {
            selectedWaypoint.previousWaypoint.nextWaypoint = selectedWaypoint.nextWaypoint;
            Selection.activeGameObject = selectedWaypoint.previousWaypoint.gameObject;
        }

        DestroyImmediate(selectedWaypoint.gameObject);
    }

    void CreateBranch()
    {
        // Create a branching waypoint from the currently selected waypoint
        if (waypointPrefab == null)
        {
            Debug.LogError("Pedestrian Waypoint Root is not assigned!");
            return;
        }

        GameObject waypointObject = Instantiate(waypointPrefab);
        waypointObject.name = "Waypoint " + waypointRoot.childCount;
        waypointObject.transform.SetParent(waypointRoot, false);

        Waypoint waypoint = waypointObject.GetComponent<Waypoint>();
        Waypoint branchedFrom = Selection.activeGameObject.GetComponent<Waypoint>();

        branchedFrom.branches.Add(waypoint);

        waypoint.transform.position = branchedFrom.transform.position;
        waypoint.transform.forward = branchedFrom.transform.forward;

        Selection.activeGameObject = waypoint.gameObject;
    }
}
