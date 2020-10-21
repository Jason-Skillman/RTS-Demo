using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MenuAction_Barracks : MenuActionItem {

	private GameObject prefab;
	
	public MenuAction_Barracks(bool active) : base("Barracks", active) {
		prefab = (GameObject)Resources.Load("Barracks");
	}

	public override void Action() {
		GameManager.main.PlaceBuilding(prefab);
	}

	

}
