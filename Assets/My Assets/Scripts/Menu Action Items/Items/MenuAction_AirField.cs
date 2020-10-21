using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MenuAction_Airfield : MenuActionItem {

	private GameObject prefab;
	
	public MenuAction_Airfield(bool active) : base("Airfield", active) {
		prefab = (GameObject)Resources.Load("Airfield");
	}

	public override void Action() {
		GameManager.main.PlaceBuilding(prefab);
	}

	

}
