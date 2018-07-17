using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData : MonoBehaviour
{

    public string roomID;
    public string roomName;
    public string roomDescription;
    public bool isConnector = false;
    public string fkFloorID = "0";

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    #region Parent Calling
    /* get parent data  (FloorData) */
    public FloorData GetParentObjectData()
    {
        return this.transform.parent.gameObject.GetComponent<FloorData>();
    }

    /* get parent object  (Floor) */
    public GameObject GetParentObject()
    {
        return this.transform.parent.gameObject;
    }

    /* get parent object  (Floor) */
    public GameObject GetParentFloorObject()
    {
        return gameObject.transform.parent.gameObject;
    }
    #endregion

    #region Floor Method

    // public GameObject GetFloor() /* return gameObject of floorData */
    // {
    //     //if(this.transform.parent.gameObject.GetComponent<FloorData>() != null)
    //     return this.transform.parent.gameObject;
    // }

    public bool IsSameFloorWith(string compareFloor)
    {
        if (this.fkFloorID == compareFloor)
        {
            return true;
        }
        return false;
    }

    public bool IsSameFloorWith(GameObject compareFloorObj)
    {
        if (this.fkFloorID == compareFloorObj.GetComponent<RoomData>().fkFloorID)
        {
            return true;
        }
        return false;
    }
    #endregion

}
