using UnityEditor;
using UnityEngine;
using System.Collections;
using RTSEngine;

[CustomEditor(typeof(Building), true)]
public class BuildingEditor : RTSGameObjectEditor {

	BuildingMode buidlingMode;

	public void OnEnable() {
		//buidlingMode = BuildingMode.Construction;
	}

	public override void OnInspectorGUI() {
		Building myTarget = (Building)target;

		base.OnInspectorGUI();

		EditorGUILayout.LabelField("Building", EditorStyles.boldLabel);
		GUILayout.Space(5);

		myTarget.buildingMode = (BuildingMode)EditorGUILayout.EnumPopup("Building Mode", myTarget.buildingMode);
		myTarget.constructionPointsMax = EditorGUILayout.IntField("Max Construction points", myTarget.constructionPointsMax);
		myTarget.constructionPoints = EditorGUILayout.IntField("Construction points", myTarget.constructionPoints);
		myTarget.objectGhost = (GameObject)EditorGUILayout.ObjectField("Prefab: Ghost", myTarget.objectGhost, typeof(GameObject), true);
		myTarget.objectBase = (GameObject)EditorGUILayout.ObjectField("Prefab: Base", myTarget.objectBase, typeof(GameObject), true);
		myTarget.objectConstruction = (GameObject)EditorGUILayout.ObjectField("Prefab: Construction", myTarget.objectConstruction, typeof(GameObject), true);
		GUILayout.Space(5);

		EditorGUILayout.LabelField("Is Constructed: " + myTarget.IsConstructed);

		GUILayout.Space(20);

		if(GUI.changed) {
			EditorUtility.SetDirty(myTarget);
		}
	}

}
