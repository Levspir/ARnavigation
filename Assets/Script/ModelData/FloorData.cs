using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorData : MonoBehaviour {

	// public List<GameObject> markerList;
	// public List<GameObject> connectorList; //this list may check when remove
	public string floorID;
	public string floorName = "0";
	public int floorIndex = 0;
	public string fkBuildingID = "0";

	// Use this for initialization
	void Start () {
		/* Work after All Marker Data get their position */

		// floorName = gameObject.name.Replace("Floor","");
		// foreach (Transform childTransform in transform)
		// {
		// 	GameObject objToAdd = childTransform.gameObject;
		// 	if(objToAdd.GetComponent<MarkerData>() != null)
		// 	{
		// 		MarkerData objData = objToAdd.GetComponent<MarkerData>();
		// 		Debug.Log("Get Marker " + objData.markerName 
		// 			+ ": " +objData.position);
		// 		objData.floor = floorName;
		// 		markerList.Add(objToAdd);
		// 		if(objData.IsConnector) {connectorList.Add(objToAdd);}
		// 	}
		// }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	#region Parent Calling
	/* get parent data  (BuildingData) */
	public BuildingData GetParentObjectData()
	{
		return this.transform.parent.gameObject.GetComponent<BuildingData>();
	}

	/* get parent object  (Building) */
	public GameObject GetParentObject()
	{
		return this.transform.parent.gameObject;
	}

	/* get parent object  (Building) */
	public GameObject GetParentBuildingObject()
	{
		return gameObject.transform.parent.gameObject;
	}

	public GameObject GetBuilding()
	{
		//if(this.transform.parent.gameObject.GetComponent<BuildingData>() != null)
		return this.transform.parent.gameObject;
	}
	#endregion

	#region Child Calling
	/* return gameobject of room that in this floor */
	public GameObject[] GetRoomsList()
	{
		Component[] roomdatalist = gameObject.GetComponentsInChildren(typeof(RoomData), true);
		GameObject[] roomObj = new GameObject[roomdatalist.Length];
		for (int i = 0; i < roomdatalist.Length; i++)
		{
			roomObj[i] = roomdatalist[i].gameObject;
		}
		return roomObj;
	}

	/* return gameobject of node that in this floor */
	public GameObject[] GetNodesList()
	{
		//count node
		GameObject[] roomlist = GetRoomsList();
		int nodecount = 0;
		foreach (GameObject rmm in roomlist)
		{
			nodecount += rmm.transform.childCount;
		}

		// put node to list
		int i = 0;
		GameObject[] nodeObj = new GameObject[nodecount];
		foreach (GameObject room in roomlist)
		{
			foreach (Transform nodetrans in room.transform)
			{
				nodeObj[i] = nodetrans.gameObject;
				i++;
			}
		}
		return nodeObj;
	}

	#endregion

//moved to maincontroller GetConnectorNode()
	// public GameObject GetConnector() /* no attribute, return first connector on list */
	// {
	// 	return connectorList[0];
	// }

	// public GameObject GetConnector(GameObject node) /* has node, Get Nearest connector object | Fix later */
	// {
	// 	return GetConnector();
	// }

	public static bool operator<(FloorData firstfloor, FloorData secondFloor)
	{
		if(firstfloor.floorIndex < secondFloor.floorIndex)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public static bool operator>(FloorData firstfloor, FloorData secondFloor)
	{
		if(firstfloor.floorIndex < secondFloor.floorIndex)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	// 	public List<GameObject> GetMarkerOfRoom(string rname) /* return all marker object that have same name that provide, nullable */
	// {
	// 	List<GameObject> resultList = null;
	// 	foreach (GameObject marker in markerList)
	// 	{
	// 		if(marker.GetComponent<MarkerData>().roomName == rname) {
	// 			resultList.Add(marker);
	// 		}
	// 	}
	// 	return resultList;
	// }
}
