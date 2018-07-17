using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ARDisplayController : MonoBehaviour {

	private enum ARType
	{
		Arrow,
		Check,
		Board
	}

	public enum ArrowDirection
	{
		Up,
		Down,
		Front
	}

	private ARObject.Type lastARType = ARObject.Type.Board;
	private MarkerConstructor markerConstructor;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/* get command from maincontroller
	detect marker show/lost from markerConstructor
	control arrow up/down*/

#region Display AR
	/* point arrow to successor's position.  if null, point to destination */
	public float ShowArrow(GameObject markerObject, bool navigatable)
	{
		Debug.Log(markerObject.name);
		GameObject objectNodeToAugment = markerObject.GetComponent<MarkerData>().GetParentNodeObject();
		GameObject arrowObj = GetARObjectOfType(markerObject, ARObject.Type.Arrow);
		ArrowScript arrowsc = arrowObj.GetComponent<ArrowScript>();
		if(navigatable || objectNodeToAugment.GetComponent<NodeData>().successor != null)
		{
			arrowsc.PointToCoordinate(objectNodeToAugment.GetComponent<NodeData>().successor.GetComponent<NodeData>().position);
			Debug.Log("Point to " + objectNodeToAugment.GetComponent<NodeData>().successor.name);
		}
		else
		{
			Debug.Log("Can't navigatable");
			arrowsc.PointToCoordinate(objectNodeToAugment.GetComponent<NodeData>().position);
		}
		arrowObj.SetActive(true);
		SendLastObject(arrowsc.Type);
		return arrowObj.transform.localRotation.eulerAngles.z;
	}

	/* point directly to node */
	public float ShowArrow(GameObject markerObject, GameObject destinationNode)
	{
		GameObject arrowObj = GetARObjectOfType(markerObject, ARObject.Type.Arrow);
		ArrowScript arrowsc = arrowObj.GetComponent<ArrowScript>();
		arrowsc.PointToCoordinate(destinationNode.GetComponent<NodeData>().position);
		arrowObj.SetActive(true);
		SendLastObject(arrowsc.Type);
		return arrowObj.transform.localRotation.eulerAngles.z;
	}

	public float ShowArrow(GameObject markerObject, bool navigatable, ArrowDirection arrowDirection)
	{
		GameObject arrowObj = GetARObjectOfType(markerObject, ARObject.Type.Arrow);
		ArrowScript arrowsc = arrowObj.GetComponent<ArrowScript>();
		switch (arrowDirection)
		{
			case ArrowDirection.Up: arrowsc.PointArrowUp(); break;
			case ArrowDirection.Down: arrowsc.PointArrowDown(); break;
			case ArrowDirection.Front: arrowsc.PointToZero(); break;
			default: arrowsc.PointToZero(); break;
		}
		arrowObj.SetActive(true);
		SendLastObject(arrowsc.Type);
		return arrowObj.transform.localRotation.eulerAngles.z;
	}

	public void ShowCheck(GameObject markerObject)
	{
		GameObject checkObj = GetARObjectOfType(markerObject, ARObject.Type.Check);
		CheckTrueScript checksc = checkObj.GetComponent<CheckTrueScript>();
		checkObj.SetActive(true);
		SendLastObject(checksc.Type);
	}
	
	public void ShowDescriprionBoard(GameObject markerObject)
	{
		//Debug.Log(markerObject.name + " is marker?  :" + (markerObject.GetComponent<MarkerData>()!=null));
		GameObject boardObj = GetARObjectOfType(markerObject, ARObject.Type.Board);
		DescriptionBoardScript boardsc = boardObj.GetComponent<DescriptionBoardScript>();
		boardsc.SetData();
		boardObj.SetActive(true);
		SendLastObject(boardsc.Type);
	}
#endregion

#region AR detect listener

	/* get last object type and send to markerConstructor */
	public void SendLastObject(ARObject.Type activeType)
	{
		if(markerConstructor == null)
		{
			markerConstructor = GameObject.Find("ARCamera").GetComponent<MarkerConstructor>();
		}
		markerConstructor.SetLastObjectType(activeType);
	}

#endregion

#region Private Method
	
	/* find AR object from marker gameobject, marker may have child */
	private GameObject GetARObjectOfType(GameObject objectMarkerToAugment, ARObject.Type artype)
	{
		for (int i = 0; i < objectMarkerToAugment.transform.childCount; i++)
		{
			GameObject markertrans = objectMarkerToAugment.transform.GetChild(i).gameObject;
			if(artype == ARObject.Type.Arrow && (markertrans.GetComponent<ArrowScript>() != null))
			{
				return markertrans.gameObject;
			}
			else if(artype == ARObject.Type.Check && (markertrans.GetComponent<CheckTrueScript>() != null))
			{
				return markertrans.gameObject;
			}
			else if(artype == ARObject.Type.Board && (markertrans.GetComponent<DescriptionBoardScript>() != null))
			{
				return markertrans.gameObject;
			}
		}
		return null;
	}

	/* find marker from child of node. if that node have marker and marker has 3 child 
	return marker gameObject*/
	private GameObject GetMarkerObjectToAugment(GameObject objectNodeToAugment)
	{
		foreach (Transform markertrans in objectNodeToAugment.transform)
		{
			if(markertrans.childCount == 3)
			{
				return markertrans.gameObject;
			}
		}
		return objectNodeToAugment.transform.GetChild(0).gameObject; //or return null
	}

	/* plus marker orientation to local euler rotation
	for arrow */
	// private void PlusMarkerOrientation(GameObject augmentObject)
	// {
	// 	augmentObject.transform.localEulerAngles = new Vector3(
	// 		augmentObject.transform.localEulerAngles.x,
	// 		augmentObject.transform.localEulerAngles.y,
	// 		augmentObject.transform.localEulerAngles.z 
	// 			+ augmentObject.transform.parent.gameObject.GetComponent<MarkerData>().markerOrientation
	// 	);
	// }
#endregion

}
