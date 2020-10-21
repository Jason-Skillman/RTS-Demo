using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MenuAction_PowerPlant : MenuActionItem {

	private GameObject prefab;
	
	public MenuAction_PowerPlant(bool active) : base("Power Plant", active) {
		prefab = (GameObject)Resources.Load("Power Plant");
	}

	public override void Action() {
		//Debug.Log("Power Plant Action Clicked");
		//GameManager.main.gameMode = GameMode.PlaceBuilding;

		//GameObject newBuilding = UnityEngine.Object.Instantiate(prefab, HoverManager.main.hoverPosision, Quaternion.identity);

		GameManager.main.PlaceBuilding(prefab);
	}

	

}
