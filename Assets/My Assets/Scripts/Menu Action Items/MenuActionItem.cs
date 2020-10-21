using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class MenuActionItem {

	public string name;
	public bool active;


	public MenuActionItem(string name, bool active) {
		this.name = name;
		this.active = active;
	}

	public abstract void Action();

}