using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MenuAction_TechCenter : MenuActionItem {

	private GameObject prefab;
	
	public MenuAction_TechCenter(bool active) : base("Tech Center", active) {
		prefab = (GameObject)Resources.Load("Tech Center");
	}

	public override void Action() {
		GameManager.main.PlaceBuilding(prefab);
	}

	

}
