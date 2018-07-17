using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class CanvasButtonScript : MonoBehaviour
{

    public GameObject markerPrefab, roomButtonPrefab;
    public GameObject actionBar;
    public GameObject searchPanel, mapPanel; //panel
    private GameObject hambergerButton, mapButton, searchButton, backButton, clearButton; //button
    private GameObject searchInputField, appName; //InputFields + text
    private Text appNameText;

    private GameObject searchHelpText, searchList, viewPort, scrollbar, searchContent, roomDataPanel, roomDataDialog;
    private GameObject roomNameTitle, roomMapImage, roomDesData, roomNavigateButton;
    private GameObject mapImage, rightButton, leftButton, navline, userDot;


    private GameObject showingFloor;
    private List<GameObject> searchShowList;

    private enum Page
    {
        Main,
        Search,
        Map
    }
    private Page page = Page.Main;

    //private CanvasResolutionScript canvasResolutionScript;
    private MapControlScript mapControl;
    private BuildingData building;
    private StateDisplayController stateDisplay;
    private ToastMessageScript toastMessageScript;

    private bool canQuitApp = true;

    // Use this for initialization
    void Start()
    {
        StartCanvas();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (page)
            {
                case Page.Search: OnCloseSearch(); break;
                case Page.Map: OnCloseMap(); break;
                case Page.Main:
                    if (canQuitApp)
                    {
                        Application.Quit();
                    }
                    else
                    {
                        canQuitApp = true;
                        //showToast
                    }
                    break;
                default: OnCloseSearch(); break;
            }
        }
    }

    public void StartCanvas()
    {
        Screen.fullScreen = false;
        building = GameObject.FindWithTag("Building").GetComponent<BuildingData>();
        Debug.Log(building.name);
        showingFloor = building.floorList[0];
        searchShowList = new List<GameObject>();

        //canvasResolutionScript = gameObject.GetComponent<CanvasResolutionScript>();
        toastMessageScript = gameObject.GetComponent<ToastMessageScript>();
        stateDisplay = gameObject.GetComponent<StateDisplayController>();
        stateDisplay.ShowToastMessage("ส่องกล้องไปยังจุดต่างๆ เช่น ป้ายบอกทาง เลขห้อง เพื่อเริ่มต้นระบุตำแหน่งของคุณ", false);
        stateDisplay.AddSound(SoundManager.SoundType.InitApp, 0);
//        stateDisplay.PlaySoundQueue();

        hambergerButton = actionBar.gameObject.transform.Find("HambergerButton").gameObject;
        mapButton = actionBar.gameObject.transform.Find("MapButton").gameObject;
        searchButton = actionBar.gameObject.transform.Find("SearchButton").gameObject;
        appName = actionBar.gameObject.transform.Find("AppName").gameObject;
        appNameText = appName.GetComponent<Text>();

        backButton = actionBar.gameObject.transform.Find("BackButton").gameObject;
        searchInputField = actionBar.gameObject.transform.Find("SearchInputField").gameObject;
        clearButton = actionBar.gameObject.transform.Find("ClearSearchButton").gameObject;

        /* search */
        searchHelpText = searchPanel.transform.Find("HelpText").gameObject;
        searchList = searchPanel.transform.Find("Scroll View").gameObject;
        viewPort = searchList.gameObject.transform.Find("Viewport").gameObject;
        scrollbar = searchList.gameObject.transform.Find("Scrollbar Vertical").gameObject;
        searchContent = viewPort.gameObject.transform.Find("Content").gameObject;
        //searchContent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(Screen.width - 50, 100);
        roomDataPanel = searchPanel.transform.Find("RoomDataPanel").gameObject;
        roomDataDialog = roomDataPanel.transform.Find("RoomDataDialog").gameObject;
        roomNameTitle = roomDataDialog.transform.Find("RoomNameTitle").gameObject;
        roomMapImage = roomDataDialog.transform.Find("RoomMapImage").gameObject;
        roomDesData = roomDataDialog.transform.Find("RoomData").gameObject;
        roomNavigateButton = roomDataDialog.transform.Find("NavigateButton").gameObject;

        /* map */
        mapImage = mapPanel.transform.Find("MapScrollViewArea").gameObject;
        rightButton = mapPanel.transform.Find("RightButton").gameObject;
        leftButton = mapPanel.transform.Find("LeftButton").gameObject;
        mapControl = mapImage.transform.Find("Mask/MapImage").gameObject.GetComponent<MapControlScript>();

        backButton.SetActive(false);
        searchInputField.SetActive(false);
        clearButton.SetActive(false);
    }

    public void OnBackButton()
    {
        switch (page)
        {
            case Page.Search: OnCloseSearch(); break;
            case Page.Map: OnCloseMap(); break;
            default: OnCloseSearch(); break;
        }
    }

    public void DummyOnclickFunction()
    {
        Debug.Log("Dummy");
    }

    #region Open New Page

    public void OnOpenSearch()
    {
        page = Page.Search;
        mapPanel.SetActive(false);
        searchPanel.SetActive(true);
        hambergerButton.SetActive(false);
        mapButton.SetActive(false);
        searchButton.SetActive(false);
        appName.SetActive(false);
        roomDataPanel.SetActive(false);

        backButton.SetActive(true);
        searchInputField.SetActive(true);
        clearButton.SetActive(true);

        // canvasResolutionScript.SetBackButtonInSearch();
        // canvasResolutionScript.SetClearButtonInSearch();
        // canvasResolutionScript.SetSearchFieldInSearch();

        // canvasResolutionScript.SetHelpTextInSearch();
        // canvasResolutionScript.SetScrollListInSearch();
        // canvasResolutionScript.SetContentInSearch();
        OnTyping();
    }

    public void OnCloseSearch()
    {
        page = Page.Main;
        mapPanel.SetActive(false);
        searchPanel.SetActive(false);
        hambergerButton.SetActive(true);
        mapButton.SetActive(true);
        searchButton.SetActive(true);
        appName.SetActive(true);
        roomDataPanel.SetActive(false);

        backButton.SetActive(false);
        searchInputField.SetActive(false);
        clearButton.SetActive(false);

        // canvasResolutionScript.SetHambergerButtonInMain();
        // canvasResolutionScript.SetMapButtonInMain();
        // canvasResolutionScript.SetSearchButtonInMain();
        // canvasResolutionScript.SetAppNameInMain();
    }

    public void OnOpenMap()
    {
        page = Page.Map;
        mapPanel.SetActive(true);
        searchPanel.SetActive(false);
        hambergerButton.SetActive(false);
        mapButton.SetActive(false);
        searchButton.SetActive(false);
        appName.SetActive(true);
        roomDataPanel.SetActive(false);

        backButton.SetActive(true);
        searchInputField.SetActive(false);
        clearButton.SetActive(false);

        // canvasResolutionScript.SetAppNameInMap();
        // canvasResolutionScript.SetAppNameInMap();

        // canvasResolutionScript.SetMapImageInMap();
        // canvasResolutionScript.SetArrowButtonInMap();
        if (MainController.instance.beginPoint != null)
        {
            showingFloor = MainController.instance.beginPoint.GetComponent<NodeData>().GetParentObjectData().GetParentFloorObject();
        }
        mapControl.UpdateMap(showingFloor);
    }

    public void OnCloseMap()
    {
        page = Page.Main;
        mapPanel.SetActive(false);
        searchPanel.SetActive(false);
        hambergerButton.SetActive(true);
        mapButton.SetActive(true);
        searchButton.SetActive(true);
        appName.SetActive(true);
        roomDataPanel.SetActive(false);

        backButton.SetActive(false);
        searchInputField.SetActive(false);
        clearButton.SetActive(false);

        // canvasResolutionScript.SetHambergerButtonInMain();
        // canvasResolutionScript.SetMapButtonInMain();
        // canvasResolutionScript.SetSearchButtonInMain();
        // canvasResolutionScript.SetAppNameInMain();
    }

    public void OnOpenRoomDialoge(GameObject roomObj, bool isDestination)
    /* show room dialoge box data with map */
    {
        NodeData nddt = roomObj.GetComponent<NodeData>();
        roomDataPanel.SetActive(true);
        roomNameTitle.GetComponent<Text>().text = "Информация: " + nddt.GetParentObjectData().roomName;
        Material floorMaterial = nddt.GetParentObjectData().GetParentFloorObject().transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().materials[0];
        roomMapImage.GetComponent<Image>().material = floorMaterial;
        roomDesData.GetComponent<Text>().text = nddt.GetParentObjectData().roomDescription;
        if (!isDestination)
        {
            roomNavigateButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Навигация.";
            roomNavigateButton.GetComponent<Button>().onClick.RemoveAllListeners();
            roomNavigateButton.GetComponent<Button>().onClick.AddListener(delegate { SelectToNavigate(roomObj); });
            if (MainController.instance.beginPoint != null)
            {
                if (nddt.GetParentObjectData().roomName ==
                    MainController.instance.beginPoint.GetComponent<NodeData>().GetParentObjectData().roomName)
                {
                    roomNavigateButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Здесь текущая комната";
                    roomNavigateButton.GetComponent<Button>().onClick.RemoveAllListeners();
                    roomNavigateButton.GetComponent<Button>().onClick.AddListener(delegate
                    {
							stateDisplay.ShowToastMessage("Ваш последний момент - это то, где вы находитесь. Выберите другой пункт назначения.");
                    });
                }
            }
        }
        else
        {
            roomNavigateButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Cancle Navigate";
            roomNavigateButton.GetComponent<Button>().onClick.RemoveAllListeners();
            roomNavigateButton.GetComponent<Button>().onClick.AddListener(ClearPoint);
        }

        //mark point and navigate draw line to destination
        GameObject destDot = roomMapImage.transform.Find("MarkDot/DestMarker").gameObject;
        destDot.GetComponent<RectTransform>().anchoredPosition = new Vector2(
            nddt.position.x * (roomMapImage.GetComponent<RectTransform>().sizeDelta.x / 1000),
            nddt.position.z * (roomMapImage.GetComponent<RectTransform>().sizeDelta.y / 1000)
        );

    }

    public void OnCloseRoomDialog()
    {
        roomDataPanel.SetActive(false);
    }

    private void SelectToNavigate(GameObject roomObj)
    {
        MainController.instance.SetDestinationPoint(roomObj);
        OnCloseRoomDialog();
        OnCloseSearch();
    }

    private void ClearPoint()
    {
        MainController.instance.ClearDestinationPoint();
        OnCloseRoomDialog();
        OnCloseSearch();
    }

    #endregion

    #region Search Action

    /* create room list from word typed, and send to show room button from list */
    public void OnTyping()
    {
        string typingWord = searchInputField.GetComponent<InputField>().text;
        searchShowList.Clear();
        foreach (GameObject floor in building.floorList)
        {
            foreach (GameObject nodet in floor.GetComponent<FloorData>().GetNodesList())
            {
                NodeData markerData = nodet.GetComponent<NodeData>();
                if (typingWord == "" && !IsDuplicateShowingRoom(searchShowList, markerData.GetParentObjectData().roomName))
                {
                    Debug.Log(typingWord + " In " + markerData.GetParentObjectData().roomName);
                    searchShowList.Add(nodet);
                }
                else if (markerData.GetParentObjectData().roomName.Contains(typingWord)
                    && !IsDuplicateShowingRoom(searchShowList, markerData.GetParentObjectData().roomName))
                {
                    Debug.Log(typingWord + " In " + markerData.GetParentObjectData().roomName);
                    searchShowList.Add(nodet);
                }
            }
        }
        ShowAllRoomOf(searchShowList);
    }

    // detect too much loop here
    private bool IsDuplicateShowingRoom(List<GameObject> searchLst, string findingMarkerName) /* false if already has marker of that room */
    {
        foreach (GameObject mk in searchLst)
        {
            if (mk.GetComponent<NodeData>().GetParentObjectData().roomName == findingMarkerName)
            {
                return true;
            }
        }
        return false;
    }

    private void ShowAllRoomOf(List<GameObject> searchMarkerList)
    {
        //if system have begin point, need to color it, set to false
        bool beginColored = !(MainController.instance.beginPoint != null);
        bool destColored = !(MainController.instance.destinationPoint != null);
        
        //destroy all list
        foreach (Transform ch in searchContent.transform)
        {
            Destroy(ch.gameObject);
        }
        foreach (GameObject nodeob in searchMarkerList)
        {
            GameObject roomButton = Instantiate(roomButtonPrefab);
            roomButton.GetComponent<RoomButtonScript>().room = nodeob;
            NodeData nodedata = nodeob.GetComponent<NodeData>();
            roomButton.transform.SetParent(searchContent.transform, false);
            Text RoomNameContent = roomButton.transform.GetChild(0).gameObject.GetComponent<Text>();
            RoomNameContent.text = nodedata.GetParentObjectData().roomName;
            Text RoomDescriptionContent = roomButton.transform.GetChild(1).gameObject.GetComponent<Text>();
            RoomDescriptionContent.text = nodedata.GetParentObjectData().roomDescription;
            if (!beginColored)
            {
                if (MainController.instance.beginPoint.GetComponent<NodeData>().GetParentObjectData().roomName
                    == nodedata.GetParentObjectData().roomName)
                {
                    roomButton.GetComponent<Image>().color = new Color32(0, 0, 0, 255);
                    RoomNameContent.color = Color.white;
                    RoomDescriptionContent.color = Color.white;
                    RoomNameContent.text = " ⦿ Вы находитесь: " + nodedata.GetParentObjectData().roomName;
                    beginColored = true;
                }
            }
            if (!destColored)
            {
                if (MainController.instance.destinationPoint.GetComponent<NodeData>().GetParentObjectData().roomName
                    == nodedata.GetParentObjectData().roomName)
                {
                    roomButton.GetComponent<RoomButtonScript>().isDestination = true;
                    roomButton.GetComponent<Image>().color = new Color32(0, 0, 0, 255);
                    RoomNameContent.color = Color.white;
                    RoomDescriptionContent.color = Color.white;
                    //roomButton.GetComponent<Image>().color = new Color32(126, 60, 255, 255); // purple
                    RoomNameContent.text = " ⦿ Назначение: " + nodedata.GetParentObjectData().roomName;
                    destColored = true;
                }
            }
        }
    }
    #endregion

    #region Map Action

    public void OnShiftMap(bool isForward)
    {
        //BuildingData building = showingFloor.GetComponent<FloorData>().GetBuilding().GetComponent<BuildingData>();

        //get next floor from buildingData
        // GameObject floorObject = isForward ?
        //     building.GetNextFloor(showingFloor.GetComponent<FloorData>().floorName) :
        //     building.GetPreviousFloor(showingFloor.GetComponent<FloorData>().floorName);
        GameObject floorObject; // = building.floorList[(int)Random.Range(0,4)];
        if(isForward)
        {
            floorObject = building.GetNextFloor(showingFloor.GetComponent<FloorData>().floorName);
        }
        else{
            floorObject = building.GetPreviousFloor(showingFloor.GetComponent<FloorData>().floorName);
        }
        
        mapControl.UpdateMap(floorObject);
        showingFloor = floorObject;
    }

    #endregion

}
