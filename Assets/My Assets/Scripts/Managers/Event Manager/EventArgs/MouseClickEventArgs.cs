using System;
using UnityEngine;
using RTSEngine;

public class MouseClickEventArgs : EventArgs {
	
	public ButtonType buttonType {
		get;
		private set;
	}
	public ButtonEventType buttonEventType {
		get;
		private set;
	}
	public Vector2 screenPosition {
		get;
		private set;
	}
	public Vector3 worldPosition {
		get;
		private set;
	}
	public RTSGameObject gameObject {
		get;
		private set;
	}

	
	public MouseClickEventArgs(ButtonType buttonType, ButtonEventType buttonEventType, Vector2 screenPosition, Vector3 worldPosition, RTSGameObject gameObject = null) {
		this.buttonType = buttonType;
		this.buttonEventType = buttonEventType;
		this.screenPosition = screenPosition;
		this.worldPosition = worldPosition;
		this.gameObject = gameObject;
	}

}

public enum ButtonEventType {
	Down,
	Holding,
	Up
}

public enum ButtonType {
	LeftButton,
	MiddleButton,
	RightButton
}
