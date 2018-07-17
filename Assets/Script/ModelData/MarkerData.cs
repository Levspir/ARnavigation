using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerData : MonoBehaviour, ICloneable {

	public string markerID;
	public string markerImageName = "";
	public int priority = 10;
	public float markerOrientation = 0f;
	public string fkNodeID = "";

	public enum NodeType
	{
		None,
		Room,
		Junction,
		Both
	}

	public NodeType nodeType = NodeType.None;



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/* get parent data  (NodeData) */
	public NodeData GetParentObjectData()
	{
		return this.transform.parent.gameObject.GetComponent<NodeData>();
	}

	/* get parent object  (Node) */
	public GameObject GetParentObject()
	{
		return this.transform.parent.gameObject;
	}

	/* get parent object  (Node) */
	public GameObject GetParentNodeObject()
	{
		return gameObject.transform.parent.gameObject;
	}
	

	#region Check Node Type
	/* Method for Check Node Type */
	public bool IsJunctionNode()
	{
		return nodeType == NodeType.Junction || nodeType == NodeType.Both ? true : false ;
	}

	public bool IsRoomNode()
	{
		return nodeType == NodeType.Room || nodeType == NodeType.Both ? true : false ;
	}

	public bool IsNoneNode()
	{
		return nodeType == NodeType.None;
	}
    #endregion

	public object Clone()
    {
		MarkerData markerDataClone = new MarkerData();
		markerDataClone.markerID = this.markerID ;
		markerDataClone.markerImageName = this.markerImageName ;
		markerDataClone.priority = this.priority ;
		markerDataClone.fkNodeID = this.fkNodeID;
		
		return markerDataClone;
    }
	
}
