using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MenuAction_Radar : MenuActionItem {

	private GameObject prefab;
	
	public MenuAction_Radar(bool active) : base("Radar", active) {
		prefab = (GameObject)Resources.Load("Radar");
	}

	public override void Action() {
		GameManager.main.PlaceBuilding(prefab);
	}

	

}
