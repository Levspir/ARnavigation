using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescriptionBoardScript : MonoBehaviour, IARObject
{

    private ARObject.Type artype = ARObject.Type.Board;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitAR()
    {
        gameObject.SetActive(false);
        SetData();
        Debug.Log(transform.parent.name + " attract desrptboard to idle");
    }

    public ARObject.Type Type
    {
        get
        {
            return artype;
        }
    }

    public void SetData()
    {
        SetRoomName(transform.parent.GetComponent<MarkerData>().GetParentObjectData().GetParentObjectData().roomName);
        SetRoomDest(transform.parent.GetComponent<MarkerData>().GetParentObjectData().GetParentObjectData().roomDescription);
    }

    public void SetRoomName(string roomname)
    {
        gameObject.transform.Find("RoomNameText").gameObject.GetComponent<TextMesh>().text = roomname;
        //if string longer than xxx resize 
    }

    public void SetRoomDest(string roomdest)
    {
        gameObject.transform.Find("RoomDescriptionText").gameObject.GetComponent<TextMesh>().text = roomdest;
        //add linebreaker and fit board with text
    }


}
