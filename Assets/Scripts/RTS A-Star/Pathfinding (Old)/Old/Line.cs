using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct Line {

	const float verticalLineGradient = 1e5f;

	float gradient;
	float yIntercept;
	Vector2 pointOnLine1;
	Vector2 pointOnLine2;
	float gradientPerpendicular;
	bool approachSide;

	public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine) {
		float dx = pointOnLine.x - pointPerpendicularToLine.x;
		float dy = pointOnLine.y - pointPerpendicularToLine.y;
		approachSide = false;

		if(dx == 0) {
			gradientPerpendicular = verticalLineGradient;
		} else {
			gradientPerpendicular = dy / dx;
		}

		if(gradientPerpendicular == 0) {
			gradient = verticalLineGradient;
		} else {
			gradient = -1 / gradientPerpendicular;
		}

		//y = mx + c
		//c = y - mx

		yIntercept = pointOnLine.y - gradient * pointOnLine.x;


		pointOnLine1 = pointOnLine;
		pointOnLine2 = pointOnLine + new Vector2(1, gradient);

		approachSide = GetSide(pointPerpendicularToLine);
	}

	bool GetSide(Vector2 point) {
		return (point.x - pointOnLine1.x) * (pointOnLine2.y - pointOnLine1.y) > (point.y - pointOnLine1.y) * (pointOnLine2.x - pointOnLine1.x);
	}

	public bool HasCrossedLine(Vector2 point) {
		return GetSide(point) != approachSide;
	}

	public void DrawGizmos(float length) {
		Vector3 lineDirection = new Vector3(1, 0, gradient).normalized;
		Vector3 lineCenter = new Vector3(pointOnLine1.x, 0, pointOnLine1.y) + Vector3.up;

		Gizmos.DrawLine(lineCenter - lineDirection * length / 2.0f, lineCenter + lineDirection * length / 2.0f);
	}
}