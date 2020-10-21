using UnityEditor;
using UnityEngine;
using System.Collections;
using RTSEngine;

[CustomEditor(typeof(Worker), true)]
public class WorkerEditor : UnitEditor {

	public override void OnInspectorGUI() {
		Worker myTarget = (Worker)target;

		base.OnInspectorGUI();

		EditorGUILayout.LabelField("Worker", EditorStyles.boldLabel);
		GUILayout.Space(5);

		myTarget.canBuild = EditorGUILayout.Toggle("Can Build", myTarget.canBuild);
		myTarget.buildPointAmount = EditorGUILayout.IntField("Build Point Amount", myTarget.buildPointAmount);
		GUILayout.Space(5);

		EditorGUILayout.LabelField("Is Building: " + myTarget.isBuilding);
		EditorGUILayout.LabelField("Building To Build: " + myTarget.buildingToBuild);
		EditorGUILayout.LabelField("Is Going To Build: " + myTarget.isGoingToBuild);

		GUILayout.Space(20);

		if(GUI.changed) {
			EditorUtility.SetDirty(myTarget);
		}
	}

}
