using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MenuAction_WarFactory : MenuActionItem {

	private GameObject prefab;
	
	public MenuAction_WarFactory(bool active) : base("War Factory", active) {
		prefab = (GameObject)Resources.Load("War Factory");
	}

	public override void Action() {
		GameManager.main.PlaceBuilding(prefab);
	}

	

}
