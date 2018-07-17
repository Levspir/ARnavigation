 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class JsonReader : MonoBehaviour
{

    public string fileroomname = "RoomJsonData.json";
    public string[] fileToLoadName = {"BuildingJsonData", "FloorJsonData",
        "RoomJsonData", "NodeJsonData", "MarkerJsonData", "ConnectJsonData"};
    public string[] structTag = { "Building", "Floor", "Room", "Node", "Marker", "Connect" };
    private string path;
    private string jsonString;

    private enum StctType
    {
        /* it is define order of object too */
        Building = 0,
        Floor = 1,
        Room = 2,
        Node = 3,
        Marker = 4,
        Connect = 5
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadBuilding();
            LoadFloor();
            LoadRoom();
            LoadNode();
            LoadConnect();
            LoadMarker();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {


        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            CheckTag("Floor");
        }
    }

    public void ReadJsonData()
    {
        LoadBuilding();
        LoadFloor();
        LoadRoom();
        LoadNode();
        LoadConnect();
        LoadMarker();
    }

    public void InitReading(string filename)
    {
        Debug.Log("initreading " + filename);

        TextAsset jsonStringresource = Resources.Load("JsonData/" + filename) as TextAsset;
        jsonString = jsonStringresource.text;
    }

    #region Building
    private void LoadBuilding()
    /* load building */
    {
        string roomFileJsonName = fileToLoadName[(int)StctType.Building];
        InitReading(roomFileJsonName);

        JBuilding[] buildings = JsonHelper.FromJson<JBuilding>(jsonString);
        Debug.Log("Loading Building total:" + buildings.Length);

        foreach (JBuilding building in buildings)
        {
            GameObject emptyobj = new GameObject(structTag[(int)StctType.Building] + building.buildingID);
            emptyobj.tag = structTag[(int)StctType.Building];
            //define position
            emptyobj.AddComponent<BuildingData>();
            BuildingData bdt = emptyobj.GetComponent<BuildingData>();
            bdt.buidingID = building.buildingID;
            bdt.buildingName = building.buildingName;

            //set position depend on index *1000
        }
    }
    #endregion

    #region Floor
    private void LoadFloor()
    /* Load and Create Floor, floorPlane with material */
    {
        string floorFileJsonName = fileToLoadName[(int)StctType.Floor];
        InitReading(floorFileJsonName);
        Vector3 planePosition = new Vector3(500, 0, 500);
        Quaternion planeRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        Vector3 planeScale = new Vector3(100, 1, 100);

        JFloor[] floors = JsonHelper.FromJson<JFloor>(jsonString);
        Debug.Log("Loading Floor total:" + floors.Length);

        foreach (JFloor floor in floors)
        {
            GameObject emptyobj = new GameObject(structTag[(int)StctType.Floor] + floor.floorID);
            emptyobj.tag = structTag[(int)StctType.Floor];

            // add floor data
            emptyobj.AddComponent<FloorData>();
            FloorData fdt = emptyobj.GetComponent<FloorData>();
            fdt.floorID = floor.floorID;
            fdt.floorName = floor.floorName;
            fdt.floorIndex = Mathf.CeilToInt(floor.floorIndex);
            fdt.fkBuildingID = floor.fkBuildingID;

            //find building parent and set localpos
            FindObjectToAttract(emptyobj, StctType.Building, fdt.fkBuildingID);
            emptyobj.transform.localPosition = new Vector3(Mathf.CeilToInt(floor.floorIndex) * 1000, 0, 0);

            // create plane child
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            Transform planeTransform = plane.transform;
            planeTransform.SetParent(emptyobj.transform);
            planeTransform.localPosition = planePosition;
            planeTransform.localRotation = planeRotation;
            planeTransform.localScale = planeScale;
            ChangeMat(plane, floor.floorImageName);

        }
    }

    private void ChangeMat(GameObject planeObj, string floorImageName)
    {
        Material matt = new Material(Shader.Find("Unlit/UnlitShader01"));
        matt.mainTexture = (Texture)Resources.Load("MapImage/" + floorImageName);
        planeObj.GetComponent<Renderer>().material = matt;
    }

    #endregion

    #region Room
    private void LoadRoom()
    /* load room and attrach to floor */
    {
        string roomFileJsonName = fileToLoadName[(int)StctType.Room];
        InitReading(roomFileJsonName);

        JRoom[] rooms = JsonHelper.FromJson<JRoom>(jsonString);
        Debug.Log("Loading Room total:" + rooms.Length);

        foreach (JRoom room in rooms)
        {
            GameObject emptyobj = new GameObject(structTag[(int)StctType.Room] + room.roomID);
            emptyobj.tag = structTag[(int)StctType.Room];
            //define position
            emptyobj.AddComponent<RoomData>();
            RoomData rdt = emptyobj.GetComponent<RoomData>();
            rdt.roomID = room.roomID;
            rdt.roomName = room.roomName;
            rdt.roomDescription = room.roomDescription;
            rdt.isConnector = room.isConnector;
            rdt.fkFloorID = room.fkFloorID;
            //find parent
            FindObjectToAttract(emptyobj, StctType.Floor, rdt.fkFloorID);
            emptyobj.transform.localPosition = Vector3.zero;
        }
    }

    #endregion

    #region Node
    private void LoadNode()
    {
        string nodeFileJsonName = fileToLoadName[(int)StctType.Node];
        InitReading(nodeFileJsonName);

        JNode[] nodes = JsonHelper.FromJson<JNode>(jsonString);
        Debug.Log("Loading Node total:" + nodes.Length);

        foreach (JNode node in nodes)
        {
            GameObject emptyobj = new GameObject(structTag[(int)StctType.Node] + node.nodeID);
            emptyobj.tag = structTag[(int)StctType.Node];
            //define position
            emptyobj.AddComponent<NodeData>();
            NodeData ndt = emptyobj.GetComponent<NodeData>();
            ndt.nodeID = node.nodeID;
            ndt.position = GetSplitValue(node.position);
            ndt.referencePosition = GetSplitValue(node.referencePosition);
            ndt.orientation = GetSplitValue(node.xzOrientation.Split(' ')[0] + " " + node.yOrientation + " " + node.xzOrientation.Split(' ')[1]); //Warn xz lenght 1
            ndt.fkRoomID = node.fkRoomID;
            //find parent
            Debug.Log("attract Node " + ndt.nodeID + " to " + ndt.fkRoomID + " :"
                + FindObjectToAttract(emptyobj, StctType.Room, ndt.fkRoomID));
            emptyobj.transform.localPosition = ndt.position;
        }
    }

    /* return 3 value. if no or error value will be return vec3.zero */
    private Vector3 GetSplitValue(string vecstring)
    {
        string[] tempstring = vecstring.Split(' ');
        if (tempstring.Length == 3)
        {
            float v1 = 0, v2 = 0, v3 = 0;
            if (!Single.TryParse(tempstring[0], out v1))
            {
                Debug.Log("Error on value 1");
                return Vector3.zero;
            }
            if (!Single.TryParse(tempstring[1], out v2))
            {
                Debug.Log("Error on value 2");
                return Vector3.zero;
            }
            if (!Single.TryParse(tempstring[2], out v3))
            {
                Debug.Log("Error on value 3");
                return Vector3.zero;
            }
            return new Vector3(v1, v2, v3);
        }
        else if (tempstring.Length == 2)
        {
            float v1 = 0, v2 = 0;
            if (!Single.TryParse(tempstring[0], out v1))
            {
                Debug.Log("Error on value 1");
                return Vector3.zero;
            }
            if (!Single.TryParse(tempstring[1], out v2))
            {
                Debug.Log("Error on value 2");
                return Vector3.zero;
            }
            return new Vector3(v1, v2, 0);
        }
        return Vector3.zero;
    }

    #endregion

    #region Connect
    private void LoadConnect() /* this function must work after node has been worked */
    {
        string connectFileJsonName = fileToLoadName[(int)StctType.Connect];
        InitReading(connectFileJsonName);

        JConnect[] cons = JsonHelper.FromJson<JConnect>(jsonString);
        Debug.Log("Loading Connect total:" + cons.Length);

        GameObject[] nodeObjs = GameObject.FindGameObjectsWithTag(structTag[(int)StctType.Node]);

        GameObject firstNode = null, secondNode = null;
        NodeData noddt, firstNodeData, secondNodeData;

        foreach (JConnect con in cons)
        {
            firstNode = null;
            secondNode = null;
            //find 2 node from all node that find
            foreach (GameObject nod in nodeObjs)
            {
                if (nod.GetComponent<NodeData>() != null)
                {
                    noddt = nod.GetComponent<NodeData>();
                    //check nodedata primarykey that equal fk
                    if (noddt.nodeID == con.nID1)
                    {
                        firstNode = nod;
                    }
                    if (noddt.nodeID == con.nID2)
                    {
                        secondNode = nod;
                    }
                }
                //check that we have found 2 node in system, then add data, and break loop all node
                if (firstNode != null && secondNode != null && firstNode != secondNode)
                {
                    firstNodeData = firstNode.GetComponent<NodeData>();
                    secondNodeData = secondNode.GetComponent<NodeData>();
                    //if firstnode.adj not have second node, add it
                    firstNodeData.AddAdjacentNode(secondNode);
                    //if secondnode.adj not have first node, add it
                    secondNodeData.AddAdjacentNode(firstNode);
                }
            }
        }
    }

    #endregion

    #region Marker
    private void LoadMarker()
    /* read marker data and add to markerConstructor's list */
    {
        string markerFileJsonName = fileToLoadName[(int)StctType.Marker];
        InitReading(markerFileJsonName);

        JMarker[] markers = JsonHelper.FromJson<JMarker>(jsonString);
        Debug.Log("Loading Marker total:" + markers.Length);

        MarkerConstructor markerConstructor = GameObject.Find("ARCamera").GetComponent<MarkerConstructor>();
        foreach (JMarker marker in markers)
        {
            markerConstructor.AddDraftMarker(new DraftMarkerData(
                marker.markerID,
                marker.markerImageName,
                Mathf.CeilToInt(marker.priority),  //may not
                marker.markerOrientation,
                marker.fkNodeID
            ));
        }
    }
    #endregion

    #region Object Tag Find
    private bool FindObjectToAttract(GameObject objChild, StctType parentType, string parentName)
    /* find all object from given tag and attract to object that similar name
	parent name may + with string before
	 -> return false if no object */
    {
        GameObject[] tagObjs = GameObject.FindGameObjectsWithTag(structTag[(int)parentType]);
        if (tagObjs.Length == 0)
        {
            return false;
        }
        foreach (GameObject tObj in tagObjs)
        {
            if (structTag[(int)parentType] + parentName == tObj.name)
            {
                objChild.transform.SetParent(tObj.transform);
                return true;
            }
        }
        return false;
    }

    private bool CheckTag(string objdectTag)
    /* print all name of gameobject with that tag */
    {
        GameObject[] tagObjs = GameObject.FindGameObjectsWithTag(objdectTag);
        foreach (GameObject tObj in tagObjs)
        {
            Debug.Log(tObj.name);
        }
        return false;
    }
    #endregion
}


#region Map Struct

[System.Serializable]
public class JMarker
{
    public string markerID;
    public string markerImageName;
    public int priority;
    public float markerOrientation;
    public string fkNodeID;
}

[System.Serializable]
public class JConnect
{
    public string connectID;
    public string nID1;
    public string nID2;
}

[System.Serializable]
public class JNode
{
    public string nodeID;
    public string position;
    public string referencePosition;
    public int yOrientation;
    public string xzOrientation;
    public string fkRoomID;
}

[System.Serializable]
public class JRoom
{
    public string roomID;
    public string roomName;
    public string roomDescription;
    public bool isConnector;
    public string fkFloorID;
}

[System.Serializable]
public class JFloor
{
    public string floorID;
    public string floorName;
    public float floorIndex;
    public string floorImageName;
    public string fkBuildingID;
}

[System.Serializable]
public class JBuilding
{
    public string buildingID;
    public string buildingName;
}

#endregion

