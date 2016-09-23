using UnityEngine;


/*DESCRIPTION
 * This class creates the links between the Interface's Scene, classes and GameObject and the others
 * */

public class InterfaceLinkManager : LinkManager
{
    //////////////////////////////// singleton fields & methods ////////////////////////////////
    public static string gameObjectName = "InterfaceLinkManager";
    private static InterfaceLinkManager _instance;

    public static InterfaceLinkManager get()
    {
        if (_instance == null)
        {
            Logger.Log("InterfaceLinkManager::get was badly initialized", Logger.Level.WARN);
            GameObject go = GameObject.Find(gameObjectName);
            if (go)
            {
                _instance = go.GetComponent<InterfaceLinkManager>();
            }
        }
        return _instance;
    }

    void Awake()
    {
        _instance = this;
        //Debug.LogError("InterfaceLinkManager awakes with (_instance == null)=="+(_instance == null));
    }

    void OnDestroy()
    {
        //Debug.LogError("InterfaceLinkManager OnDestroy");
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    public static string interfaceLinkManagerGameObjectName = "InterfaceLinkManager";

    public Fade fade;
    public TooltipPanel biobrickTooltipPanel, deviceTooltipPanel;
    public GameObject inventoryDevicePrefab;
    public GameObject listedDevicePrefab;

    public GameObject craftZoneDisplayedBioBrickPrefab;
    public GameObject lastHoveredInfo;
    public GameObject genericInfoWindow;


    bool isCraftMode1 = true;

    // craft screen with activate/deactivate button, device slots, recipes, biobricks sorted by columns
    public GameObject craftScreenPanel1;
    // craft screen with craft button, biobrick-sorting buttons, recipes, inventory link, craft table
    public GameObject craftScreenPanel2;

    private GameObject craftScreenPanel;
    public Transform craftSlotsGrid;


    public GameObject equipedDeviceButtonPrefabPos, equipedDeviceButtonPrefabPos2;
    public UIPanel equipedDevicesSlotsPanel;
    public GameObject equipedDevice, equipedDevice2;
    public GameObject tinyBioBrickIconPrefabPos, tinyBioBrickIconPrefabPos2;
    public CraftFinalizer craftFinalizer;
    public UIPanel inventoryDevicesSlotsPanel;

    public GameObject tutorialArrow;

    public GameObject tutorialPanels;

    public GameObject introduction1, introduction2, okButton1, okButton2, end, pauseIndicator;
    public EndMainMenuButton endMainMenuButton;

    // main menu
    public ControlsMainMenuItemArray controlsArray;
    public AbsoluteWASDButton absoluteWASDButton;
    public LeftClickToMoveButton leftClickToMoveButton;
    public RelativeWASDButton relativeWASDButton;
    public RightClickToMoveButton rightClickToMoveButton;
    public UISprite selectedKeyboardControlTypeSprite;
    public UISprite selectedMouseControlTypeSprite;
    public GameObject modalBackground;
    public GameObject genericModalWindow;
    public MainMenuManager mainMenu;

    public LoggerLabel loggerGUIComponent;

    public VectrosityPanel celliaGraph, roomGraph;
    public GraphMoleculeList graphMoleculeList;

    public override void initialize()
    {

        Debug.Log("InterfaceLinkManager: mainMenu=" + mainMenu);

        // activate everything
        activateAllChildren(true);

        //shortcut
        CraftZoneManager craftZoneManager = CraftZoneManager.get();
        GameStateController gameStateController = GameStateController.get();
        //CraftFinalizer _craftfinalizer = craftFinalizer;
        GUITransitioner guiTransitioner = GUITransitioner.get();
        DevicesDisplayer devicesDisplayer = DevicesDisplayer.get();
        InfoWindowManager infoWindowManager = InfoWindowManager.get();
        AvailableBioBricksManager availableBioBricksManager = AvailableBioBricksManager.get();
        TooltipManager tooltipManager = TooltipManager.get();
        ModalManager modalManager = ModalManager.get();

        //GUITransitioner
        guiTransitioner.celliaGraph = celliaGraph;
        guiTransitioner.roomGraph = roomGraph;

        guiTransitioner.worldScreen = GameObject.Find("WorldScreensPanel");

        if (isCraftMode1)
        {
            craftScreenPanel = craftScreenPanel1;
        }
        else
        {
            craftScreenPanel = craftScreenPanel2;
        }

        guiTransitioner.craftScreen = craftScreenPanel;

        ContinueButton cb = okButton1.GetComponent<ContinueButton>();
        StartGameButton sgb = okButton2.GetComponent<StartGameButton>();

        //GameStateController
        gameStateController.intro = introduction1;
        gameStateController.introContinueButton = cb;
        gameStateController.fadeSprite = fade;
        gameStateController.endWindow = end;
        EndMainMenuButton emmb = endMainMenuButton.GetComponent<EndMainMenuButton>();
        gameStateController.endMainMenuButton = emmb;
        gameStateController.mainMenu = mainMenu;
        MainMenuManager.setInstance(mainMenu);

        gameStateController.pauseIndicator = pauseIndicator;

        //initialization of intro panels
        cb.nextInfoPanel = introduction2;
        cb.nextInfoPanelContinue = sgb;

        //CraftZoneManager
        craftZoneManager.craftFinalizer = craftFinalizer;

        //CraftFinalizer _craftFinalizer2 = CraftZoneManager.get().GetComponent<CraftZoneManager>().craftFinalizer;
        if (null == craftFinalizer.craftFinalizationButton)
            craftFinalizer.craftFinalizationButton = GameObject.Find("CraftButton").GetComponent<CraftFinalizationButton>();

        craftZoneManager.displayedBioBrick = craftZoneDisplayedBioBrickPrefab;
        craftZoneManager.lastHoveredInfoManager = lastHoveredInfo.GetComponent<LastHoveredInfoManager>();


        string assemblyZoneName = isCraftMode1 ? "CraftSlotsPanel" : "AssemblyZonePanel";
        craftZoneManager.assemblyZonePanel = craftScreenPanel.transform.FindChild("TopPanel").transform.FindChild(assemblyZoneName).gameObject;

        if (isCraftMode1)
        {
            LimitedBiobricksCraftZoneManager lbczm = (LimitedBiobricksCraftZoneManager)craftZoneManager;
            lbczm.slotsGrid = craftSlotsGrid;
        }


        //DevicesDisplayer

        devicesDisplayer.equipPanel = equipedDevicesSlotsPanel;
        devicesDisplayer.inventoryPanel = inventoryDevicesSlotsPanel;
        devicesDisplayer.listedInventoryPanel = craftScreenPanel.transform.FindChild("BottomPanel").transform.FindChild("DevicesPanel").GetComponent<UIPanel>();
        devicesDisplayer.listedDevicesGrid = GameObject.Find("ListedDevicesGrid").transform;

        devicesDisplayer.graphMoleculeList = graphMoleculeList;

        devicesDisplayer.equipedDevice = equipedDeviceButtonPrefabPos;
        devicesDisplayer.equipedDevice2 = equipedDeviceButtonPrefabPos2;

        devicesDisplayer.inventoryDevice = inventoryDevicePrefab;
        devicesDisplayer.listedInventoryDevice = listedDevicePrefab;


        //InfoWindowManager
        infoWindowManager.infoPanel = genericInfoWindow;
        infoWindowManager.titleLabel = genericInfoWindow.transform.FindChild("TitleLabel").GetComponent<UILocalize>();
        infoWindowManager.subtitleLabel = genericInfoWindow.transform.FindChild("SubtitleLabel").GetComponent<UILocalize>();
        infoWindowManager.explanationLabel = genericInfoWindow.transform.FindChild("ExplanationLabel").GetComponent<UILocalize>();
        infoWindowManager.bottomLabel = genericInfoWindow.transform.FindChild("BottomLabel").GetComponent<UILocalize>();
        infoWindowManager.infoSprite = genericInfoWindow.transform.FindChild("InfoSprite").GetComponent<UISprite>();

        //ModalManager
        modalManager.modalBackground = modalBackground;
        modalManager.genericModalWindow = genericModalWindow;
        modalManager.titleLabel = genericModalWindow.transform.FindChild("TitleLabel").GetComponent<UILocalize>();
        modalManager.explanationLabel = genericModalWindow.transform.FindChild("ExplanationLabel").GetComponent<UILocalize>();
        modalManager.infoSprite = genericModalWindow.transform.FindChild("InfoSprite").GetComponent<UISprite>();
        modalManager.genericValidateButton = genericModalWindow.transform.FindChild("ValidateButton").gameObject;
        modalManager.genericCenteredValidateButton = genericModalWindow.transform.FindChild("CenteredValidateButton").gameObject;
        modalManager.genericCancelButton = genericModalWindow.transform.FindChild("CancelButton").gameObject;

        //BiobrickInventory

        //AvailableBioBricksManager.get().bioBricksPanel = GameObject.Find("BiobricksPanel");
        availableBioBricksManager.bioBricksPanel = craftScreenPanel.transform.FindChild("BottomPanel").transform.FindChild("BiobricksPanel").gameObject;

        if (isCraftMode1)
        {
            availableBioBricksManager.promoterBrickCategoryGrid = GameObject.Find("PromoterBrickCategoryGrid").transform;
            availableBioBricksManager.rbsBrickCategoryGrid = GameObject.Find("RBSBrickCategoryGrid").transform;
            availableBioBricksManager.geneBrickCategoryGrid = GameObject.Find("CodingSequenceBrickCategoryGrid").transform;
            availableBioBricksManager.terminatorBrickCategoryGrid = GameObject.Find("TerminatorBrickCategoryGrid").transform;
        }
        else
        {
            availableBioBricksManager.availableBioBrick = availableBioBricksManager.bioBricksPanel.transform.FindChild("AvailableDisplayedBioBrickPrefab").gameObject;
        }

        //TooltipManager
        tooltipManager.bioBrickTooltipPanel = biobrickTooltipPanel;
        tooltipManager.deviceTooltipPanel = deviceTooltipPanel;
        tooltipManager.uiCamera = GameObject.Find("Camera").GetComponent<Camera>();

        Logger.get().loggerGUIComponent = loggerGUIComponent;
    }


    public override void finishInitialize()
    {
        GameObject bars = GameObject.Find("CutSceneBlackBars");

        activateAllChildren(false);

        //TODO should be done in gameStateController instead

        // in TutorialPanels
        tutorialPanels.SetActive(true);
        introduction1.SetActive(false);
        introduction2.SetActive(false);
        end.SetActive(false);
        bars.SetActive(true);

        // in WorldScreensPanel
        pauseIndicator.SetActive(false);

        CraftZoneManager.get().initialize();
        AvailableBioBricksManager.get().initialize();
        FocusMaskManager.get().reinitialize();
        DevicesDisplayer.get().initialize();
    }

}
