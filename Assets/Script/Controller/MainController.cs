using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{

    public static MainController instance;
    private DijsktraAlgorithm dijsktra;
    private ARDisplayController ar;
    private StateDisplayController stateDisplay;
    private JsonReader jsonReader;
    private CanvasButtonScript canvas;

    public GameObject beginPoint = null;
    public GameObject destinationPoint = null;
    public GameObject reachedPoint = null;
    public GameObject oldBeginPoint = null, oldDestinationPoint = null, oldReachePoint = null;
    private GameObject lastMarker = null;
    public enum AppState
    {
        Idle,
        Navigate
    }

    public AppState appState = AppState.Idle;
    private AppState oldAppState = AppState.Idle;
    public bool navigatable = false; //due arcontrolscr use

    //private SoundManager.SoundType sound;
    private string appstring = "Диплом";
    private string toastmsg = "";

    void Awake()
    {
        if (instance == null)
        {
            Debug.Log("Start MainController");
            instance = this;
            dijsktra = new DijsktraAlgorithm();
            ar = new ARDisplayController();
            jsonReader = new JsonReader();
            jsonReader.ReadJsonData();
            stateDisplay = GameObject.Find("Canvas").GetComponent<StateDisplayController>();
            GameObject.FindWithTag("Building").GetComponent<BuildingData>().GetAllFloorToList();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        /* observer zone, control state changing */
        if (appState != oldAppState)
        {
            //call observer
            //SetDisplay();
            oldAppState = appState;
        }
    }

    #region SetPoint

    public void SetBeginMarker(GameObject getedMarker)
    /* get new detect marker and check that have node parent 
    if yes, set new node point and process*/
    {
        if (this.lastMarker != getedMarker)
        {
            if (getedMarker.GetComponent<MarkerData>() != null) //is real marker
            {
                if (getedMarker.GetComponent<MarkerData>().GetParentNodeObject().GetComponent<NodeData>() != null) //is real node
                {
                    //change beginpoint
                    this.lastMarker = getedMarker;
                    this.beginPoint = getedMarker.GetComponent<MarkerData>().GetParentNodeObject();
                    Debug.Log("Set Begin Point from marker" + beginPoint.GetComponent<NodeData>().GetParentObjectData().roomName
                        + " @node" + beginPoint.GetComponent<NodeData>().nodeID);
                    ProcessBeginPoint();
                }
            }
            this.lastMarker = getedMarker;
            this.oldBeginPoint = beginPoint;
        }

    }
    public void SetBeginPoint(GameObject beginPoint)
    /* get node object from markerConstructor
    for user custom set beginpoint
    change appstate and point value
    process and send to display */
    {
        if (oldBeginPoint != beginPoint)
        {
            if (beginPoint.GetComponent<NodeData>() != null)
            {
                this.beginPoint = beginPoint;
                Debug.Log("Set Begin Point to room " + beginPoint.GetComponent<NodeData>().GetParentObjectData().roomName + " @node"
                     + beginPoint.GetComponent<NodeData>().nodeID);
            }

            ProcessBeginPoint();
            this.oldBeginPoint = beginPoint;
        }
    }

    private void ProcessBeginPoint()
    /* process state and sent display 
    trigger when got new marker, app state change (for call display)*/
    {
        toastmsg = "Сейчас вы: " + this.beginPoint.GetComponent<NodeData>().GetParentObjectData().roomName;
        if (this.destinationPoint == null && this.reachedPoint == null)
        {
            appState = AppState.Idle;
        }
        else if (this.destinationPoint == null && this.reachedPoint != null)
        {
            appState = AppState.Idle;
            if (IsSameRoom(this.beginPoint, this.reachedPoint))
            {

            }
            else
            {

            }
        }
        else if (this.destinationPoint != null)
        {
            if (IsSameRoom(this.beginPoint, this.destinationPoint))
            {
                appState = AppState.Idle;
                this.reachedPoint = this.destinationPoint;
                this.destinationPoint = null;
                this.oldDestinationPoint = null;
            }
            else
            {
                appState = AppState.Navigate;
                Navigate();
            }
        }
        //another case    else if (this.destinationPoint != null && this.reachedPoint != null)
        SetDisplay();
        stateDisplay.ShowToastMessage(toastmsg);
        stateDisplay.ChangeActionText(appstring);
//        stateDisplay.PlaySoundQueue();
    }

    public void SetDestinationPoint(GameObject destinationPoint)
    /* get destination node from user set in find 
    change appstate and point value
    process and send to display*/
    {
        if (destinationPoint != null)
        {
            if (destinationPoint.GetComponent<NodeData>() != null)
            {
                this.destinationPoint = destinationPoint;
                toastmsg = "Навигация По" + this.destinationPoint.GetComponent<NodeData>().GetParentObjectData().roomName;
                stateDisplay.AddSound(SoundManager.SoundType.StartNav, 0);
                Debug.Log("Set Destination Point to room " + destinationPoint.GetComponent<NodeData>().GetParentObjectData().roomName + " @node"
                        + destinationPoint.GetComponent<NodeData>().nodeID);
            }
        }
        else //from clearpoint
        {
            toastmsg = "Навигация Отменена";
            stateDisplay.AddSound(SoundManager.SoundType.CancleNav, 0);
            appstring = "AR Indoor Navigation";
            this.destinationPoint = destinationPoint;
        }

        ProcessDestinationPoint();
        this.oldDestinationPoint = destinationPoint;
    }

    private void ProcessDestinationPoint()
    /* process state, command navigate and sent display 
    trigger when got new marker, app state change (for call display)*/
    {
        if (this.beginPoint == null && this.reachedPoint == null)
        {
            appState = AppState.Idle;
        }
        else if (this.destinationPoint == null && this.reachedPoint == null)
        {
            appState = AppState.Idle;
        }
        else if (this.beginPoint != null && this.destinationPoint != null)
        {
            if (IsSameRoom(this.destinationPoint, this.beginPoint))
            {
                appState = AppState.Idle;
                this.destinationPoint = null;
                this.oldDestinationPoint = null; // may unknow old point
                this.reachedPoint = null;
            }
            else
            {
                appState = AppState.Navigate;
                Navigate();
            }
        }
        SetDisplay();
        stateDisplay.ShowToastMessage(toastmsg);
        stateDisplay.ChangeActionText(appstring);
//        stateDisplay.PlaySoundQueue();
    }
    public void ClearDestinationPoint()
    {
        SetDestinationPoint(null);
        this.reachedPoint = null;
        appState = AppState.Idle;
    }

    #endregion

    #region Navigate

    private bool NavigateIfNotSamePoint() //unused
    /* check before navigate that two new point 
    -didn't at destination room
    -not same node
    change state them
    return true if two point didn't same room and go to navigate*/
    {
        if (this.beginPoint != null && this.destinationPoint != null)
        {
            if (this.beginPoint.GetComponent<NodeData>().nodeID != this.oldBeginPoint.GetComponent<NodeData>().nodeID) //not old point
            {
                if (this.beginPoint.GetComponent<NodeData>().GetParentObjectData().roomName
                                == this.destinationPoint.GetComponent<NodeData>().GetParentObjectData().roomName) //is at destination room
                {
                    this.reachedPoint = this.destinationPoint;
                    this.destinationPoint = null;
                    this.oldDestinationPoint = null;
                    appState = AppState.Idle;
                    return false;
                }
                // if still not destination but new point, run navigate
                appState = AppState.Navigate;
                Navigate();
            }
        }
        return true;
    }

    public void Navigate()
    /* run navigate algorithm by usion beginpoint and destpoint to update all markerdata value
	if in same floor, navigate to connecttor of floor 
	if can't find route, arrow will directly point to destination*/
    {
        if (this.beginPoint.GetComponent<NodeData>().IsSameFloorWith(this.destinationPoint))
        {
            Debug.Log("Same Floor  Navigating " + this.beginPoint.GetComponent<NodeData>().GetParentObjectData().GetParentObjectData().floorName);
            // if (this.beginPoint.GetComponent<NodeData>().GetParentObjectData().roomName
            //     == this.destinationPoint.GetComponent<NodeData>().GetParentObjectData().roomName) //<<<<< Dead Code. check room name
            // {
            //     Debug.Log("=== Founded Destination ===");
            //     appState = AppState.Idle;
            //     Debug.Log(" Reach at Main navigate and idle");
            // }
            if (dijsktra.FindShortestPath(this.beginPoint, this.destinationPoint))
            {
                navigatable = true;
            }
            else
            {
                navigatable = false;
            }
        }
        else
        {
            Debug.Log("==================================== Different Floor  Navigating "
                + this.beginPoint.GetComponent<NodeData>().GetParentObjectData().GetParentObjectData().floorName
                + " To " + this.destinationPoint.GetComponent<NodeData>().GetParentObjectData().GetParentObjectData().floorName);

            bool begintoLift = dijsktra.FindShortestPath(this.beginPoint, GetConnectorNode(this.beginPoint));
            Debug.Log("==================================== brk");
            bool liftToDest = dijsktra.FindShortestPath(GetConnectorNode(this.destinationPoint), this.destinationPoint);
            if (begintoLift && liftToDest)
            {
                navigatable = true;
            }
            else
            {
                navigatable = false;
            }
        }
        //case point to null of successor
    }


    #endregion

    #region Display
    private void SetDisplay()
    /* set display from state and current node
    trigger when appstate change and after process new point finish */
    {
        stateDisplay.ChangeActionBarColor(appState);
        if (appState == AppState.Idle)
        {
            if (this.beginPoint == null && this.destinationPoint == null && this.reachedPoint == null) //startapp
            {

            }
            else if (this.beginPoint != null && this.destinationPoint == null && this.reachedPoint == null)
            {
                if (toastmsg != "Навигация отменена") //is pass canclenav
                {
                    appstring = "Вы в : " + this.beginPoint.GetComponent<NodeData>().GetParentObjectData().roomName;
                    stateDisplay.AddSound(SoundManager.SoundType.Located, 1);
                }
                ar.ShowDescriprionBoard(this.lastMarker);
            }
            else if (this.beginPoint != null && this.destinationPoint != null && this.reachedPoint == null) //idle unreach
            {
                if (IsSameRoom(this.beginPoint, this.destinationPoint))
                {
                    ar.ShowCheck(this.lastMarker);
                    string roomname = this.beginPoint.GetComponent<NodeData>().GetParentObjectData().roomName;
                    stateDisplay.AddSound(SoundManager.SoundType.Reached, 1);
                    stateDisplay.AddSound(SoundManager.SoundType.EndNav, 2);
                    appstring = "Вы на месте: " + this.beginPoint.GetComponent<NodeData>().GetParentObjectData().roomName;
                    toastmsg = ("Чтобы добраться до пункта назначения: " + roomname);
                }
                else
                {
                    appstring = "You are AT: " + this.beginPoint.GetComponent<NodeData>().GetParentObjectData().roomName;
                    stateDisplay.AddSound(SoundManager.SoundType.Located, 1);
                    DisplayArrowToAR();
                }
            }
            else if (this.beginPoint != null && this.destinationPoint == null && this.reachedPoint != null)
            {
                if (IsSameRoom(this.beginPoint, this.reachedPoint))
                {
                    ar.ShowCheck(this.lastMarker);
                    string roomname = this.beginPoint.GetComponent<NodeData>().GetParentObjectData().roomName;
                    stateDisplay.AddSound(SoundManager.SoundType.Reached, 1);
                    stateDisplay.AddSound(SoundManager.SoundType.EndNav, 2);
                    appstring = "Вы на месте: " + this.beginPoint.GetComponent<NodeData>().GetParentObjectData().roomName;
                    toastmsg = ("Прибыл в пункт назначения: " + roomname);
                }
                else
                {
                    if (this.oldBeginPoint == null || this.oldDestinationPoint == null)
                    {
                        stateDisplay.AddSound(SoundManager.SoundType.StartNav, 0);
                    }
                    else //may unreach due displayarrow
                    {
                        stateDisplay.AddSound(SoundManager.SoundType.Located, 1);
                    }
                    ar.ShowDescriprionBoard(this.lastMarker);
                }
            }
            else if (this.beginPoint == null && this.destinationPoint != null && this.reachedPoint == null) //has select destpoint
            {
                stateDisplay.AddSound(SoundManager.SoundType.InitFornav, 0);
                //toast find marker
            }
            else if (this.beginPoint == null && this.destinationPoint != null && this.reachedPoint != null) //all case unreach
            {
                toastmsg = ("Error case: no Begin point but have destination and reachpoint");
                if (IsSameRoom(this.destinationPoint, this.reachedPoint))
                {
                    ar.ShowDescriprionBoard(this.lastMarker);
                }
                else
                {
                    ar.ShowDescriprionBoard(this.lastMarker);
                }
            }
            else if (this.beginPoint == null && this.destinationPoint == null && this.reachedPoint != null) //all case unreach
            {
                toastmsg = ("Error case: no Begin point and Destination point but have reachpoint");
                ar.ShowDescriprionBoard(this.lastMarker);
            }
            else if (this.beginPoint != null && this.destinationPoint != null && this.reachedPoint != null)
            {
                // three point can't be equal in result
                if (this.oldBeginPoint == null || this.oldDestinationPoint == null)
                {
                    stateDisplay.AddSound(SoundManager.SoundType.StartNav, 0);
                }
                else //may unreach due displayarrow
                {
                    stateDisplay.AddSound(SoundManager.SoundType.Located, 1);
                }
                appstring = "You are AT: " + this.beginPoint.GetComponent<NodeData>().GetParentObjectData().roomName;
                DisplayArrowToAR();
                if (IsSameRoom(this.beginPoint, this.destinationPoint))
                {
                    ar.ShowCheck(this.lastMarker);
                    string roomname = this.beginPoint.GetComponent<NodeData>().GetParentObjectData().roomName;
                    stateDisplay.AddSound(SoundManager.SoundType.Reached, 1);
                    stateDisplay.AddSound(SoundManager.SoundType.EndNav, 2);
                    appstring = "Вы на месте: " + this.beginPoint.GetComponent<NodeData>().GetParentObjectData().roomName;
                    toastmsg = ("มาถึงปลายทางแล้ว: " + roomname);
                }
            }
        }
        else if (appState == AppState.Navigate)
        {
            // header violet
            if (this.beginPoint == null && this.destinationPoint == null && this.reachedPoint == null) //startapp navigate unreach
            {

            }
            else if (this.beginPoint != null && this.destinationPoint == null && this.reachedPoint == null) //navigate unreach
            {
                appstring = "Вы находитесь: " + this.destinationPoint.GetComponent<NodeData>().GetParentObjectData().roomName;
                ar.ShowDescriprionBoard(this.lastMarker);
            }
            else if (this.beginPoint != null && this.destinationPoint != null && this.reachedPoint == null)
            {
                if (IsSameRoom(this.beginPoint, this.destinationPoint))
                {
                    ar.ShowCheck(this.lastMarker);
                    string roomname = this.beginPoint.GetComponent<NodeData>().GetParentObjectData().roomName;
                    stateDisplay.AddSound(SoundManager.SoundType.Reached, 1);
                    stateDisplay.AddSound(SoundManager.SoundType.EndNav, 2);
                    appstring = "Вы на месте: " + this.destinationPoint.GetComponent<NodeData>().GetParentObjectData().roomName;
                    toastmsg = ("Прибыл в пункт назначения: " + roomname);
                }
                else
                {
                    if (this.oldBeginPoint == null || this.oldDestinationPoint == null)
                    {
                        stateDisplay.AddSound(SoundManager.SoundType.StartNav, 0);
                    }
                    else //may unreach due displayarrow
                    {
                        stateDisplay.AddSound(SoundManager.SoundType.Located, 1);
                    }
                    appstring = "Двигайтесь к: " + this.destinationPoint.GetComponent<NodeData>().GetParentObjectData().roomName;
                    DisplayArrowToAR();
                }
            }
            else if (this.beginPoint != null && this.destinationPoint == null && this.reachedPoint != null) //navigate unreach
            {
                if (IsSameRoom(this.beginPoint, this.reachedPoint))
                {
                    ar.ShowCheck(this.lastMarker);
                    string roomname = this.beginPoint.GetComponent<NodeData>().GetParentObjectData().roomName;
                    stateDisplay.AddSound(SoundManager.SoundType.Reached, 1);
                    stateDisplay.AddSound(SoundManager.SoundType.EndNav, 2);
                    appstring = "Вы на месте: " + this.beginPoint.GetComponent<NodeData>().GetParentObjectData().roomName;
                    toastmsg = ("Чтобы добраться до пункта назначения: " + roomname);
                }
                else
                {
                    stateDisplay.AddSound(SoundManager.SoundType.Located, 1);
                    ar.ShowDescriprionBoard(this.lastMarker);
                }
            }
            else if (this.beginPoint == null && this.destinationPoint != null && this.reachedPoint == null) //has select destpoint nav unreach
            {
                //toast find marker
                appstring = "Двигайтесь к: " + this.destinationPoint.GetComponent<NodeData>().GetParentObjectData().roomName;
            }
            else if (this.beginPoint == null && this.destinationPoint != null && this.reachedPoint != null) //all case unreach
            {
                if (IsSameRoom(this.destinationPoint, this.reachedPoint))
                {
                    appstring = "Направляйтесь к: " + this.destinationPoint.GetComponent<NodeData>().GetParentObjectData().roomName;
                    ar.ShowDescriprionBoard(this.lastMarker);
                }
                else
                {
                    appstring = "Направляйтесь к: " + this.destinationPoint.GetComponent<NodeData>().GetParentObjectData().roomName;
                    ar.ShowDescriprionBoard(this.lastMarker);
                }
            }
            else if (this.beginPoint == null && this.destinationPoint == null && this.reachedPoint != null) //all case unreach
            {
                ar.ShowDescriprionBoard(this.lastMarker);
            }
            else if (this.beginPoint != null && this.destinationPoint != null && this.reachedPoint != null)
            {
                // three point can't be equal in result
                if (this.oldBeginPoint == null || this.oldDestinationPoint == null)
                {
                    stateDisplay.AddSound(SoundManager.SoundType.StartNav, 0);
                }
                else //may unreach due displayarrow
                {
                    stateDisplay.AddSound(SoundManager.SoundType.Located, 1);
                }
                DisplayArrowToAR();
                appstring = "Направляйтесь к: " + this.destinationPoint.GetComponent<NodeData>().GetParentObjectData().roomName;
                if (IsSameRoom(this.beginPoint, this.destinationPoint))
                {
                    ar.ShowCheck(this.lastMarker);
                    string roomname = this.beginPoint.GetComponent<NodeData>().GetParentObjectData().roomName;
                    stateDisplay.AddSound(SoundManager.SoundType.Reached, 1);
                    stateDisplay.AddSound(SoundManager.SoundType.EndNav, 2);
                    appstring = "Вы на месте: " + this.destinationPoint.GetComponent<NodeData>().GetParentObjectData().roomName;
                    toastmsg = ("Вы пришли: " + roomname);
                }
            }
        }
    }

    private void DisplayArrowToAR()
    {
        float arrowZrotation = 0f;
        if (navigatable)
        {
            arrowZrotation = ar.ShowArrow(this.lastMarker, navigatable);
            stateDisplay.AddSound(stateDisplay.GetSoundDirection(arrowZrotation), 1);
        }
        else
        {
            // process arrow direction
            int beginfloor = this.beginPoint.GetComponent<NodeData>().GetParentObjectData().GetParentObjectData().floorIndex;
            int destfloor = this.destinationPoint.GetComponent<NodeData>().GetParentObjectData().GetParentObjectData().floorIndex;
            if (beginfloor < destfloor)
            {
                arrowZrotation = ar.ShowArrow(this.lastMarker, navigatable, ARDisplayController.ArrowDirection.Up);
                stateDisplay.AddSound(SoundManager.SoundType.Upstairs, 1);
            }
            else if (beginfloor > destfloor)
            {
                arrowZrotation = ar.ShowArrow(this.lastMarker, navigatable, ARDisplayController.ArrowDirection.Down);
                stateDisplay.AddSound(SoundManager.SoundType.Downstairs, 1);
            }
            else
            {
                arrowZrotation = ar.ShowArrow(this.lastMarker, this.destinationPoint);
                stateDisplay.AddSound(stateDisplay.GetSoundDirection(arrowZrotation), 1);
            }
        }
        
    }
    #endregion

    #region Public calculate method
    /* get connector node from nodeobject  currenly return first objectof roomobject
    return node that have room are connector */
    public GameObject GetConnectorNode(GameObject nodeObj)
    {
        //Transform[] roomlist = nodeObj.GetComponent<NodeData>().GetParentObjectData().GetParentObjectData().gameObject.transform;
        foreach (GameObject room in nodeObj.GetComponent<NodeData>().GetParentObjectData().GetParentFloorObject().transform)
        {
            if (room.GetComponent<RoomData>().isConnector)
            {
                return room.transform.GetChild(0).gameObject;
            }
        }
        // return first child of floor, room
        return nodeObj.GetComponent<NodeData>().GetParentObjectData().GetParentObjectData().gameObject.transform.GetChild(0).GetChild(0).gameObject;
    }
    #endregion

    #region Private calculate method
    /* compare roomname from node data */
    private bool IsSameRoom(GameObject node1, GameObject node2)
    {
        if (node1.GetComponent<NodeData>() != null && node2.GetComponent<NodeData>() != null)
        {
            RoomData node1roomData = node1.GetComponent<NodeData>().GetParentObjectData();
            RoomData node2roomData = node2.GetComponent<NodeData>().GetParentObjectData();
            return node1roomData.roomName == node2roomData.roomName;
        }
        return false;
    }
    #endregion
}
