using UnityEditor;
using UnityEngine;
using System.Collections;
using RTSEngine;

[CustomEditor(typeof(Unit), true)]
public class UnitEditor : RTSGameObjectEditor {

	public override void OnInspectorGUI() {
		Unit myTarget = (Unit)target;

		base.OnInspectorGUI();

		EditorGUILayout.LabelField("Unit", EditorStyles.boldLabel);
		GUILayout.Space(5);

		myTarget.canMove = EditorGUILayout.Toggle("Can Move", myTarget.canMove);
		myTarget.canInteract = EditorGUILayout.Toggle("Can Interact", myTarget.canInteract);
		GUILayout.Space(5);

		EditorGUILayout.LabelField("Target To Pursue: "+myTarget.targetToPursue);

		GUILayout.Space(20);

		if(GUI.changed) {
			EditorUtility.SetDirty(myTarget);
		}
	}

}
