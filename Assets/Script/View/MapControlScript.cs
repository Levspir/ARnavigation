using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class MapControlScript : MonoBehaviour, IPointerClickHandler
{

    public GameObject mapImageObject;
    public GameObject markerPrefab;
    private GameObject navline, userDot;
    private int tabCount = 0;
    private float maxDoubleTabTime = 0.5f;
    private float newTime;
    private float zoomSpeed = 0.5f;
    private bool IsNormalSize;
    private bool hasMovedFlag = false;
    private bool isUserInThisFloor = false;
    float mapPadding = 0;

    private GameObject[] nodeForLineArr = new GameObject[3];
    private GameObject showingFloor;
    private BuildingData building;

    RectTransform mapImage;

    // Use this for initialization
    void Start()
    {
        StartMapControl();
    }

    void Awake()
    {
        StartMapControl();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 2)
        {
            //condition that 2 finger once in map
            Debug.Log("2 Touch" + Input.GetTouch(0).phase + "|" + Input.GetTouch(1).phase);
            ZoomMap();
        }
    }

    public void StartMapControl()
    {
        mapImage = this.gameObject.GetComponent<RectTransform>();
        navline = this.transform.Find("Line").gameObject;
        userDot = this.transform.Find("UserDot").gameObject;
        // !!! warn !!!  showingfloor change when change bulding
        building = GameObject.FindWithTag("Building").GetComponent<BuildingData>();
        showingFloor = building.floorList[0];
    }

    public void OnPointerClick(PointerEventData eventData)
    //implement from ipointerclickhandler for click one time only
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                hasMovedFlag = true;
            }
            if (touch.phase == TouchPhase.Ended)
            {
                if (!hasMovedFlag)
                {
                    tabCount += 1;
                }
                hasMovedFlag = false;
            }

            if (tabCount == 1)
            {
                newTime = Time.time + maxDoubleTabTime;
            }
            else if (tabCount >= 2 && Time.time <= newTime)
            {
                if (!IsNormalSize)
                {
                    RestoreMap();
                }
                else
                {
                    ZoomMapx2();
                }
                //Whatever you want after a dubble tap    
                tabCount = 0;
            }
        }
        if (Time.time > newTime)
        {
            tabCount = 0;
        }
    }

    private void ZoomMapx2()
    {
        ChangeMapSize(
                mapImage.sizeDelta.x * 1.5f,
                mapImage.sizeDelta.y * 1.5f
            );
        IsNormalSize = false;
    }

    private void ZoomMap()
    {
        // Store both touches.
        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        // Find the position in the previous frame of each touch.
        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        // Find the magnitude of the vector (the distance) between the touches in each frame.
        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        // Find the difference in the distances between each frame.
        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        mapImage.sizeDelta = new Vector2(                               // warn sizedelta use 2 here
            mapImage.sizeDelta.x - (deltaMagnitudeDiff * zoomSpeed),
            mapImage.sizeDelta.y - (deltaMagnitudeDiff * zoomSpeed)
        );
        ChangeMapSize(
            Mathf.Clamp(mapImage.sizeDelta.x, Screen.width - mapPadding, 3000),
            Mathf.Clamp(mapImage.sizeDelta.y, Screen.width - mapPadding, 3000)
        );
        //mapImage.anchoredPosition = Vector2.zero;
        IsNormalSize = false;
    }

    private void RestoreMap()
    {
        int mapsize = Mathf.FloorToInt(Screen.width - mapPadding);
        ChangeMapSize(700, 700); //Screen.width - mapPadding, Screen.width - mapPadding
        mapImage.anchoredPosition = new Vector2(0, 0);
        IsNormalSize = true;
    }

    public void UpdateMap(GameObject floorObject)
    //recive floor obj to change from canvasbutton 
    //change floor pic and find path
    {
        RestoreMap();
        // get material from first child of floorData 
        Material floorMaterial = floorObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().materials[0];
        this.gameObject.GetComponent<Image>().material = floorMaterial;
        ShowMarkerOfFloor(floorObject);

        FloorData floorObjectData = floorObject.GetComponent<FloorData>();

        GameObject beginPoint = MainController.instance.beginPoint;
        GameObject destinationPoint = MainController.instance.destinationPoint;

        //check floor, start stop of that floor for line
        if (MainController.instance.appState == MainController.AppState.Navigate
            && beginPoint != null && destinationPoint != null)
        {
            NodeData beginPointData = beginPoint.GetComponent<NodeData>();
            NodeData destinationPointData = destinationPoint.GetComponent<NodeData>();
            GameObject beginFloor = MainController.instance.beginPoint.GetComponent<NodeData>().GetParentObjectData().GetParentFloorObject();
            GameObject destinationFloor = MainController.instance.destinationPoint.GetComponent<NodeData>().GetParentObjectData().GetParentFloorObject();
            FloorData beginFloorData = beginFloor.GetComponent<FloorData>();
            FloorData destinationFloorData = destinationFloor.GetComponent<FloorData>();

            if (beginPointData.IsSameFloorWith(destinationPoint) && beginFloorData.floorIndex == floorObjectData.floorIndex) //loking fl in same
            {
                nodeForLineArr[0] = beginPoint;
                nodeForLineArr[1] = destinationPoint;
            }
            else
            {
                if (beginFloorData.floorIndex == floorObjectData.floorIndex)
                { //swap to begin fl
                    nodeForLineArr[0] = beginPoint;
                    nodeForLineArr[1] = MainController.instance.GetConnectorNode(beginPoint);
                }
                else if (destinationFloorData.floorIndex == floorObjectData.floorIndex)
                { //swap in dest fl
                    nodeForLineArr[0] = MainController.instance.GetConnectorNode(destinationPoint);
                    nodeForLineArr[1] = destinationPoint;
                }
                else
                {
                    //check is looking floor are inbeetween 
                    //if yes green dot in lift (or current connector)
                    if (beginFloorData.floorIndex < destinationFloorData.floorIndex
                        && floorObjectData.floorIndex < destinationFloorData.floorIndex
                        && floorObjectData.floorIndex > beginFloorData.floorIndex)
                    {
                        nodeForLineArr[0] = GetNearestConnector(beginPointData, floorObject);  //floorObjectData.connectorList[0];
                        nodeForLineArr[1] = null;
                    }
                    else if (beginFloorData.floorIndex > destinationFloorData.floorIndex
                      && floorObjectData.floorIndex > destinationFloorData.floorIndex
                      && floorObjectData.floorIndex < beginFloorData.floorIndex)
                    {
                        nodeForLineArr[0] = GetNearestConnector(beginPointData, floorObject); //floorObjectData.connectorList[0];
                        nodeForLineArr[1] = null;
                    }
                    else
                    {
                        nodeForLineArr[0] = null;
                        nodeForLineArr[1] = null;
                    }
                    //if no, will not show line in 
                }
            }
             
        }
        else if (MainController.instance.appState == MainController.AppState.Idle)
        {
            nodeForLineArr[0] = null;
            nodeForLineArr[1] = null;
        }
        
        DrawLine();
        
        //check floor and current position for user dot
        userDot.SetActive(false);
        isUserInThisFloor = false;
        if (beginPoint != null)
        {
            if (MainController.instance.beginPoint.GetComponent<NodeData>().GetParentObjectData().GetParentFloorObject() == floorObject)
            {
                ShowUserDot(MainController.instance.beginPoint);
                isUserInThisFloor = true;
            }
        }  
        showingFloor = floorObject;
    }

    public void ChangeMapSize(float xSize, float ySize)
    {
        mapImage.sizeDelta = new Vector2(xSize, ySize);
        navline.GetComponent<RectTransform>().sizeDelta = mapImage.sizeDelta;
        navline.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        ShowMarkerOfFloor();
        DrawLine();
        if (isUserInThisFloor) { ShowUserDot(MainController.instance.beginPoint); }
    }


    #region In Map Component

    public void ShowMarkerOfFloor() /* resize marker with map size */
    {
        GameObject markers = mapImage.transform.GetChild(0).gameObject;
        //destroy all marker
        foreach (Transform ch in markers.transform)
        {
            Destroy(ch.gameObject);
        }
        //create marker prefab
        foreach (Transform roomTrans in showingFloor.transform)
        {
            foreach (Transform nodeTrans in roomTrans)
            {
                GameObject nodeob = nodeTrans.gameObject;
                //instantiate marker at child of Markers
                GameObject markerDot = Instantiate(markerPrefab);
                NodeData nodeData = nodeob.GetComponent<NodeData>();
                markerDot.transform.SetParent(markers.transform);
                //recttransform coordinate xy 1000/mapimage.sizedelta
                markerDot.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                    nodeData.position.x * (mapImage.GetComponent<RectTransform>().sizeDelta.x / 1000),
                    nodeData.position.z * (mapImage.GetComponent<RectTransform>().sizeDelta.y / 1000)
                );
            }
        }
    }

    public void ShowMarkerOfFloor(GameObject floorObject) /* show node point in map */
    {
        GameObject markers = mapImage.transform.GetChild(0).gameObject;
        //destroy all marker
        foreach (Transform ch in markers.transform)
        {
            Destroy(ch.gameObject);
        }
        //create marker prefab
        foreach (Transform roomTrans in floorObject.transform)
        {
            foreach (Transform nodeTrans in roomTrans)
            {
                GameObject nodeob = nodeTrans.gameObject;
                //instantiate marker at child of Markers
                GameObject markerDot = Instantiate(markerPrefab);
                NodeData nodeData = nodeob.GetComponent<NodeData>();
                markerDot.transform.SetParent(markers.transform);
                //recttransform coordinate xy 1000/mapimage.sizedelta
                markerDot.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                    nodeData.position.x * (mapImage.GetComponent<RectTransform>().sizeDelta.x / 1000),
                    nodeData.position.z * (mapImage.GetComponent<RectTransform>().sizeDelta.y / 1000)
                );
            }
        }
    }

    private void DrawLine() /* draw line with map size */
    {
        if (nodeForLineArr[0] == null && nodeForLineArr[1] == null)
        {
            ClearLine();
        }
        else if (nodeForLineArr[0] != null && nodeForLineArr[1] == null)
        {
            ShowLine(nodeForLineArr[0]);
        }
        else if (nodeForLineArr[0] != null && nodeForLineArr[1] != null)
        {
            ShowLine(nodeForLineArr[0], nodeForLineArr[1]);
        }
    }

    private void ShowLine(GameObject begin, GameObject destination) /* show green navigate line on map */
    {
        UILineRenderer line = navline.GetComponent<UILineRenderer>();
        line.Points.Clear();
        NodeData checkPoint = begin.GetComponent<NodeData>();
        int i = 0;
        //Debug.Log("Write Line At " + checkPoint.markerName + " " + line.Points(i));
        while (checkPoint.successor != null)
        {
            // last point point to marker position
            line.Points.Add(new Vector2(
                checkPoint.referencePosition.x * (mapImage.GetComponent<RectTransform>().sizeDelta.x / 1000),
                checkPoint.referencePosition.z * (mapImage.GetComponent<RectTransform>().sizeDelta.y / 1000)
            ));
            i++;
            checkPoint = checkPoint.successor.GetComponent<NodeData>();
        }
        // add last point
        line.Points.Add(new Vector2(
            checkPoint.referencePosition.x * (mapImage.GetComponent<RectTransform>().sizeDelta.x / 1000),
            checkPoint.referencePosition.z * (mapImage.GetComponent<RectTransform>().sizeDelta.y / 1000)
        ));
        line.Points.Add(new Vector2(
            checkPoint.position.x * (mapImage.GetComponent<RectTransform>().sizeDelta.x / 1000),
            checkPoint.position.z * (mapImage.GetComponent<RectTransform>().sizeDelta.y / 1000)
        ));
        line.SetVerticesDirty();
    }

    private void ShowLine(GameObject point) /*draw line on connector point */
    {
        UILineRenderer line = navline.GetComponent<UILineRenderer>();
        line.Points.Clear();
        NodeData checkPoint = point.GetComponent<NodeData>();

        line.Points.Add(new Vector2(
            checkPoint.position.x * (mapImage.GetComponent<RectTransform>().sizeDelta.x / 1000),
            checkPoint.position.z * (mapImage.GetComponent<RectTransform>().sizeDelta.y / 1000)
        ));
        line.Points.Add(new Vector2(
            checkPoint.referencePosition.x * (mapImage.GetComponent<RectTransform>().sizeDelta.x / 1000),
            checkPoint.referencePosition.z * (mapImage.GetComponent<RectTransform>().sizeDelta.y / 1000)
        ));
        line.SetVerticesDirty();
    }

    private void ClearLine() /* don't show line in that floor */
    {
        UILineRenderer line = navline.GetComponent<UILineRenderer>();
        line.Points.Clear();
        line.SetVerticesDirty();
    }

    private void ShowUserDot(GameObject point)
    {
        userDot.SetActive(true);
        NodeData nodeData = point.GetComponent<NodeData>();
        RectTransform dotRect = userDot.GetComponent<RectTransform>();
        dotRect.anchoredPosition = new Vector2(
            nodeData.referencePosition.x * (mapImage.GetComponent<RectTransform>().sizeDelta.x / 1000),
            nodeData.referencePosition.z * (mapImage.GetComponent<RectTransform>().sizeDelta.y / 1000)
        );
        float deltaX = nodeData.referencePosition.x - nodeData.position.x;
        float deltaY = nodeData.referencePosition.z - nodeData.position.z;
        dotRect.rotation = Quaternion.Euler(new Vector3(0, 0,
            (((Mathf.Atan2(deltaY, deltaX)) * 180 / Mathf.PI) + 90)
        ));
    }
    #endregion

    #region Beetween Floor Calculation
    /* Compare position of begin floor node with all current floor connection nodes.
     will return Nearest position that IsConnector
	return first node of floor if none*/
    public GameObject GetNearestConnector(NodeData beginPointData, GameObject floorObject)
    {
        float nearestDistance = Single.PositiveInfinity;
        float distance = 0;
        GameObject nearestConnectNode = floorObject.transform.GetChild(0).GetChild(0).gameObject;
        foreach (Transform roomTransform in floorObject.transform)
        {
            if (roomTransform.gameObject.GetComponent<RoomData>().isConnector)
            {
                foreach (Transform nodeTransform in roomTransform)
                {
                    NodeData nodedt = nodeTransform.gameObject.GetComponent<NodeData>();
                    distance = Vector3.Distance(nodedt.position, beginPointData.position);
                    if (distance < nearestDistance)
                    {
                        nearestConnectNode = nodeTransform.gameObject;
                        nearestDistance = distance;
                    }
                }
            }
        }
        return nearestConnectNode;

    }
    #endregion
}
