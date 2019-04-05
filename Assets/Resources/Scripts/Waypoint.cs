using UnityEngine;
using System.Collections;

public class Waypoint {
	
	public int x;
	public int y;
	public float g = 0;
	public float h = 0;
	public float f = 0;
	public Waypoint parent;

	public Waypoint(int x, int y) {
		this.x = x;
		this.y = y;
	}
}
