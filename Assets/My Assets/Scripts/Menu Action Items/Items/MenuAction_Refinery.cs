using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MenuAction_Refinery : MenuActionItem {

	private GameObject prefab;
	
	public MenuAction_Refinery(bool active) : base("Refinery", active) {
		prefab = (GameObject)Resources.Load("Refinery");
	}

	public override void Action() {
		GameManager.main.PlaceBuilding(prefab);
	}

	

}
