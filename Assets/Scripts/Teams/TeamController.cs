using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using RTSEngine;

public class TeamController : MonoBehaviour {
	
	public Team team;
	public PlayerControl control = PlayerControl.Nothing;
	public Material matColor;

	public List<RTSGameObject> allAllies;
	public List<RTSGameObject> allEnemies;


	public void Start() {
		TeamManager.main.AddTeam(this);

		allAllies = new List<RTSGameObject>();
		allEnemies = new List<RTSGameObject>();
	}

	public void Update() {
		foreach(RTSGameObject rtsGameObject in RTSGameObject.allRTSGameObjects) {
			if(!allAllies.Contains(rtsGameObject) && rtsGameObject.team == team) {
				rtsGameObject.teamController = this;
				allAllies.Add(rtsGameObject);
			}
			else if(!allEnemies.Contains(rtsGameObject) && rtsGameObject.team != team) {
				allEnemies.Add(rtsGameObject);
			}
		}
	}


	public bool IsRTSAlly(RTSGameObject rtsGameObject) {
		if(allAllies.Contains(rtsGameObject)) {
			return true;
		}
		return false;
	}

	public bool IsRTSEnemy(RTSGameObject rtsGameObject) {
		if(allEnemies.Contains(rtsGameObject)) {
			return true;
		}
		return false;
	}

}

public enum Team {
	Netrual,
	Red,
	Blue,
	Yellow,
	Green
}

public enum PlayerControl {
	Player,
	Computer,
	Nothing
}
