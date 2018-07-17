using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingData : MonoBehaviour {

	public List<GameObject> floorList;
	public string buidingID;
	public string buildingName = "Building";

	// Use this for initialization
	void Start () {
		// int index = 0;
		// foreach (Transform childTransform in transform)
		// {
		// 	GameObject objToAdd = childTransform.gameObject;
		// 	if(objToAdd.GetComponent<FloorData>() != null)
		// 	{
		// 		//buiding name didn't get name from gameobject name
		// 		Debug.Log("Get Floor " + objToAdd.GetComponent<FloorData>().floorName);
		// 		objToAdd.GetComponent<FloorData>().floorIndex = index;
		// 		index++;
		// 		floorList.Add(objToAdd);
		// 	}
		// }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/* check all floor in parent and add to list
	Call this after initiate all object */
	public void GetAllFloorToList()
	{
		floorList = new List<GameObject>();
		foreach (Transform floor in gameObject.transform)
		{
			if(floor.gameObject.GetComponent<FloorData>() != null)
			{
				floorList.Add(floor.gameObject);
			}
		}
		//sort by index
		floorList.Sort((x,y) => x.GetComponent<FloorData>().floorIndex.CompareTo(y.GetComponent<FloorData>().floorIndex));
	}

	/* return gemeobject of cycled floor  if not found return null */
	public GameObject GetNextFloor(string floorName)
	{		
		for (int i = 0; i < floorList.Count; i++)
		{
			FloorData fl = floorList[i].GetComponent<FloorData>();
			if (floorName == fl.floorName && i >= floorList.Count-1) { 	// last index return first
				//Debug.Log(i + ">" + (floorList.Count-1) +" is Last index   RETURN " + floorList[0].GetComponent<FloorData>().floorName + " @"+floorList[0].GetComponent<FloorData>().floorIndex);
				return floorList[0]; 
			} else if (floorName == fl.floorName) { 					//name ard equal return next index
				//Debug.Log(i + "=" + fl.floorName +" name equal  RETURN " + floorList[i+1].GetComponent<FloorData>().floorName + " @"+floorList[i+1].GetComponent<FloorData>().floorIndex);
				return floorList[i+1];
			}
		}
		return null; 													//not found return null
	}

	public GameObject GetPreviousFloor(string floorName)
	{
		for (int i = floorList.Count-1; i >= 0; i--)
		{
			FloorData fl = floorList[i].GetComponent<FloorData>();
			if (floorName == fl.floorName && i <= 0) {
				//Debug.Log(i + "<" + 0 +" is First index   RETURN " + floorList[floorList.Count-1].GetComponent<FloorData>().floorName + " @"+floorList[floorList.Count-1].GetComponent<FloorData>().floorIndex);
				return floorList[floorList.Count-1];
			} else if (floorName == fl.floorName) { 					//name ard equal return previous index
				//Debug.Log(i + "=" + fl.floorName +" name equal  RETURN " + floorList[i-1].GetComponent<FloorData>().floorName + " @"+floorList[i-1].GetComponent<FloorData>().floorIndex);
				return floorList[i-1];
			}
		}
		return null; 											//not found return null
	}






	// public GameObject GetNextFloor(string floorName)
	// {
	// 	for (int i = 0; i < floorList.Count; i++)
	// 	{
	// 		FloorData fl = floorList[i].GetComponent<FloorData>();
	// 		if (floorName == fl.floorName && i >= floorList.Count-1) { 	// last index return first
	// 			return floorList[0]; 
	// 		} else if (floorName == fl.floorName) { 					//name ard equal return next index
	// 			return floorList[i+1];
	// 		}
	// 	}
	// 	return null; 													//not found return null
	// }

	// public GameObject GetPreviousFloor(string floorName)
	// {
	// 	for (int i = floorList.Count-1; i >= 0; i--)
	// 	{
	// 		FloorData fl = floorList[i].GetComponent<FloorData>();
	// 		if (floorName == fl.floorName && i <= 0) {
	// 			return floorList[floorList.Count-1];
	// 		} else if (floorName == fl.floorName) { 					//name ard equal return previous index
				
	// 			return floorList[i-1];
	// 		}
	// 	}
	// 	return null; 											//not found return null
	// }

	// public bool IsSameFloor(GameObject firstNode, GameObject secondNode) /* check bool is same floor */
	// {
	// 	return (firstNode.GetComponent<MarkerData>().floor == secondNode.GetComponent<MarkerData>().floor);
	// }

	// public GameObject GetConnector(GameObject Node) /* get gameObject of connector of that node floor, May have overloading*/
	// {
	// 	if(Node.GetComponent<MarkerData>().IsConnector) {
	// 		return Node;
	// 	} else { //return first of connector
	// 		return Node.GetComponent<MarkerData>().GetFloor().GetComponent<FloorData>().connectorList[0];
	// 	}
	// }
}
