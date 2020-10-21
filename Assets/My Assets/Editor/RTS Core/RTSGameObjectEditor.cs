using UnityEditor;
using UnityEngine;
using System.Collections;
using RTSEngine;

[CustomEditor(typeof(RTSGameObject), true)]
public class RTSGameObjectEditor : Editor {

	public override void OnInspectorGUI() {
		RTSGameObject myTarget = (RTSGameObject)target;

		//base.OnInspectorGUI();

		GUILayout.Space(5);
		EditorGUILayout.LabelField("RTSGameObject", EditorStyles.boldLabel);
		GUILayout.Space(5);

		myTarget.teamController = (TeamController)EditorGUILayout.ObjectField("Team: ", myTarget.teamController, typeof(TeamController), true);
		myTarget.unitType = (UnitType)EditorGUILayout.EnumPopup("RTS Type", myTarget.unitType);
		myTarget.team = (Team)EditorGUILayout.EnumPopup("Team Color", myTarget.team);
		myTarget.maxHealth = EditorGUILayout.IntField("Max Health", myTarget.maxHealth);
		myTarget.health = EditorGUILayout.IntField("Health", myTarget.health);
		myTarget.attackRadius = EditorGUILayout.FloatField("Attack Radius", myTarget.attackRadius);
		myTarget.nearRadius = EditorGUILayout.FloatField("Near Radius", myTarget.nearRadius);
		myTarget.canAttack = EditorGUILayout.Toggle("Can Attack", myTarget.canAttack);
		myTarget.turret = (GameObject)EditorGUILayout.ObjectField("Prefab: Turret", myTarget.turret, typeof(GameObject), true);
		myTarget.bullet = (GameObject)EditorGUILayout.ObjectField("Prefab: Bullet", myTarget.bullet, typeof(GameObject), true);
		myTarget.bulletSpawn = (GameObject)EditorGUILayout.ObjectField("Prefab: Bullet Spawn", myTarget.bulletSpawn, typeof(GameObject), true);
		myTarget.attackTime = EditorGUILayout.FloatField("Attack Time", myTarget.attackTime);
		GUILayout.Space(5);

		EditorGUILayout.LabelField("Is Selected: " + myTarget.isSelected);
		EditorGUILayout.LabelField("Is Attacking: " + myTarget.isAttacking);
		EditorGUILayout.LabelField("Target Lock: " + myTarget.targetLock);
		EditorGUILayout.LabelField("Attack Timer: " + myTarget.attackTimer);

		GUILayout.Space(20);

		if(GUI.changed) {
			EditorUtility.SetDirty(myTarget);
		}
	}

}
