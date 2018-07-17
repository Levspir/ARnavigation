using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeData : MonoBehaviour
{

    public string nodeID;
    public Vector3 position = Vector3.zero;
    public Vector3 referencePosition = Vector3.zero;
    public Vector3 orientation = Vector3.zero;
    public float cost = Single.PositiveInfinity;
    public GameObject predecessor = null;
    public GameObject successor = null;
    public List<GameObject> adjacentNodeList;

    public string fkRoomID;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    #region Parent Calling
    /* get parent data  (Roomdata) */
    public RoomData GetParentObjectData()
    {
        return this.transform.parent.gameObject.GetComponent<RoomData>();
    }

    /* get parent object  (Room) */
	public GameObject GetParentObject()
	{
		return this.transform.parent.gameObject;
	}

    /* get parent object  (Room) */
	public GameObject GetParentRoomObject()
	{
		return gameObject.transform.parent.gameObject;
	}
    #endregion

	/* check all node in list. If we already have that given node. will not add this */
    public bool AddAdjacentNode(GameObject newNode)
    {
        if(adjacentNodeList == null)
        {
            //Debug.Log("Init adjnodelist of" +nodeID);
            adjacentNodeList = new List<GameObject>();
        }
        foreach (GameObject adjnode in adjacentNodeList)
        {
			//Debug.Log(" add node compare" + adjnode.name + " " + newNode.name);
            if (adjnode.name == newNode.name)
            {
				//Debug.Log("--duplicate found");
				return false;
            }
        }
		//Debug.Log(" add node " + newNode.name);
		adjacentNodeList.Add(newNode);
        return true;
    }

    #region Floor Method

    public GameObject GetFloor() /* return gameObject of floorData */
    {
        if(this.transform.parent.parent.gameObject.GetComponent<FloorData>() != null)
        {
            return this.transform.parent.parent.gameObject;
        }
        return null;
    }

    /* assume that get nodedata object to compare */
    public bool IsSameFloorWith(GameObject compareNodeObj)
    {
        if (this.GetParentObjectData().fkFloorID == compareNodeObj.GetComponent<NodeData>().GetParentObjectData().fkFloorID)
        {
            return true;
        }
        return false;
    }
    #endregion

    private bool AttractFloor()
    /* for attract with floor and set position */
    {
        GameObject[] floorObjs = GameObject.FindGameObjectsWithTag("Floor");
        if (floorObjs.Length == 0)
        {
            return false;
        }
        foreach (GameObject flObj in floorObjs)
        {
            Debug.Log(flObj.name);

        }
        return true;
    }


    public void SetupSelf()
    {

    }

    private void AttractRoom()
    /* to be heirachy of room */
    {

    }

}
