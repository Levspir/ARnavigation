using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftMarkerData {

	public string markerID;
    public string markerImageName;
    public int priority;
	public float markerOrientation;
    public string fkNodeID;

	public DraftMarkerData(string markerID, string markerImageName, int priority, float markerOrientation, string fkNodeID){
		this.markerID = markerID;
		this.markerImageName = markerImageName;
		this.priority = priority;
		this.markerOrientation = markerOrientation;
		this.fkNodeID = fkNodeID;
	}
}
