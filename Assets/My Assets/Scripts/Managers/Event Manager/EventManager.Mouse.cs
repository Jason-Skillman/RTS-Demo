using UnityEngine;
using System.Collections;
using System;
using RTSEngine;

public partial class EventManager {

	//-----------------------Double Click Paramters------------------
	public float doubleClickTime = 300.0f;
	private bool checkForDoubleClick = false;
	private DateTime timeAtFirstClick;


	Vector2 screenPosision = new Vector2();
	Vector3 worldPosision = new Vector3();
	RTSGameObject gameObjectSelected;


	private void CheckMouseClickEvents() {

		screenPosision.Set((int)Input.mousePosition.x, (int)Input.mousePosition.y);
		gameObjectSelected = null;

		//------------------------------Raycast for all events------------------------------
		if(Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, Mathf.Infinity)) {
				worldPosision.Set(hit.point.x, hit.point.y, hit.point.z);
				if(hit.collider.gameObject.GetComponent<RTSGameObject>() != null) {  //If the gameobject is a RTSGameObject
					gameObjectSelected = hit.collider.gameObject.GetComponent<RTSGameObject>();
				}
			}
		}


		//Left mouse button
		if(Input.GetMouseButton(0)) {
			if(Input.GetMouseButtonDown(0)) {
				if(gameObjectSelected != null) {
					MouseClickEvent(new MouseClickEventArgs(ButtonType.LeftButton, ButtonEventType.Down, screenPosision, worldPosision, gameObjectSelected));
				} else {
					MouseClickEvent(new MouseClickEventArgs(ButtonType.LeftButton, ButtonEventType.Down, screenPosision, worldPosision));
				}
			} else {
				MouseClickEvent(new MouseClickEventArgs(ButtonType.LeftButton, ButtonEventType.Holding, screenPosision, worldPosision));
			}
		} else if(Input.GetMouseButtonUp(0)) {
			MouseClickEvent(new MouseClickEventArgs(ButtonType.LeftButton, ButtonEventType.Up, screenPosision, worldPosision));
		}

		

		//Middle mouse button
		if(Input.GetMouseButton(2)) {
			if(Input.GetMouseButtonDown(2)) {
				MouseClickEvent(new MouseClickEventArgs(ButtonType.MiddleButton, ButtonEventType.Down, screenPosision, worldPosision));
			} else {
				MouseClickEvent(new MouseClickEventArgs(ButtonType.MiddleButton, ButtonEventType.Holding, screenPosision, worldPosision));
			}
		} else if(Input.GetMouseButtonUp(2)) {
			MouseClickEvent(new MouseClickEventArgs(ButtonType.MiddleButton, ButtonEventType.Up, screenPosision, worldPosision));
		}


		//Right mouse button
		if(Input.GetMouseButton(1)) {
			if(Input.GetMouseButtonDown(1)) {
				if(gameObjectSelected != null) {
					MouseClickEvent(new MouseClickEventArgs(ButtonType.RightButton, ButtonEventType.Down, screenPosision, worldPosision, gameObjectSelected));
				} else {
					MouseClickEvent(new MouseClickEventArgs(ButtonType.RightButton, ButtonEventType.Down, screenPosision, worldPosision));
				}
			} else {
				MouseClickEvent(new MouseClickEventArgs(ButtonType.RightButton, ButtonEventType.Holding, screenPosision, worldPosision));
			}
		} else if(Input.GetMouseButtonUp(1)) {
			MouseClickEvent(new MouseClickEventArgs(ButtonType.RightButton, ButtonEventType.Up, screenPosision, worldPosision));
		}

		/*
		if(Input.GetMouseButtonDown(1)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 8 | 1 << 9 | 1 << 12 | 1 << 13)) {
				//Right Clicked on unit or building
				if(MouseClick != null) {
					//MouseClick(this, new RightButton_Handler((int)Input.mousePosition.x, (int)Input.mousePosition.y, 1, hit.collider.gameObject.GetComponent<RTSObject>()));
				}
			} else if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 11 | 1 << 18)) {
				//Right clicked on terrain
				if(MouseClick != null) {
					MouseClick(this, new RightButton_Handler((int)Input.mousePosition.x, (int)Input.mousePosition.y, 1, hit.point));
				}
			}
		}

		if(Input.GetMouseButtonDown(2)) {
			if(MouseClick != null) {
				MouseClick(this, new MiddleButton_Handler((int)Input.mousePosition.x, (int)Input.mousePosition.y, 2));
			}
		}
		*/
		

		if(checkForDoubleClick) {
			if((DateTime.Now-timeAtFirstClick).Milliseconds >= doubleClickTime) {
				checkForDoubleClick = false;
			}
		}
	}

	private void DoubleClickCheck(object sender, MouseClickEventArgs e) {
		//if(e.doubleClick || e.buttonUp) return;

		if(checkForDoubleClick) {
			TimeSpan timeBetweenClicks = DateTime.Now-timeAtFirstClick;

			if(timeBetweenClicks.Milliseconds < doubleClickTime) {
				//e.doubleClick = true;
			}
		} else {
			checkForDoubleClick = true;
			timeAtFirstClick = DateTime.Now;
		}
	}
}
