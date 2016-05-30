using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;
using NoMercyForIntruder.Game.Building;

namespace NoMercyForIntruder.Game.Controller
{

    internal enum OperationState
    {
        None,
        Add,
        Delete
    }

    public class PlayerOperator : MonoBehaviour
    {

        static public GameObject trainMenu;

        [SerializeField] private GameObject buildingPrefab;
        [SerializeField] private PlayerProfile m_player;
        [SerializeField] private GameObject[] trainUnits;
        [SerializeField] private GameObject[] placeBuildings;
        [SerializeField] private GameObject buildingMenu;
        [SerializeField] private GameObject trainingMenu;

        private int m_curBuildingIndex;
        private OperationState m_curOperationState;
        private BuildingDefaultBehaviour m_curSelectedBuilding;
        private GameFieldInitialization m_buildField;

        // Use this for initialization
        void Start()
        {
            m_curBuildingIndex = 0;
            m_curOperationState = OperationState.None;
            m_curSelectedBuilding = null;
            m_buildField = GetComponent<GameFieldInitialization>();
            trainMenu = trainingMenu;
        }

        // Update is called once per frame
        void Update()
        {
            FSM();
        }

        void FSM()          // Handle input in all status
        {
            switch(m_curOperationState)
            {
                case OperationState.None:
                    CheckInputForNone();
                    break;
                case OperationState.Add:
                    CheckInputForAdd();
                    break;
                case OperationState.Delete:
                    CheckInputForDelete();
                    break;
            }
        }

        void CheckInputForNone()
        {
            if (Input.GetMouseButton(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "Building")
                    {
                        BuildingDefaultBehaviour build = hit.collider.GetComponent<BuildingDefaultBehaviour>();
                        if (build)
                        {
                            if (m_curSelectedBuilding != null)
                            {
                                m_curSelectedBuilding.Select(false);
                            }
                            m_curSelectedBuilding = build;
                            build.Select(true);
                        }
                    }
                    else
                    {
                        if (m_curSelectedBuilding != null)
                        {
                            m_curSelectedBuilding.Select(false);
                            m_curSelectedBuilding = null;
                        }
                    }
                }
            }
        }

        void CheckInputForAdd()
        {
            if (Input.GetMouseButton(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "BuildingBase")
                    {
                        BuildingBaseBehaviour buildBase = hit.collider.GetComponent<BuildingBaseBehaviour>();
                        if (buildBase.IsAvailable())
                        {
                            GameObject go = Instantiate(placeBuildings[m_curBuildingIndex], buildBase.transform.position, placeBuildings[m_curBuildingIndex].transform.rotation) as GameObject;
                            BuildingDefaultBehaviour building = go.GetComponent<BuildingDefaultBehaviour>();
                            if (building.CheckForBuild(buildBase.GetPosition(0), buildBase.GetPosition(1)) && m_player.PayForBuilding(building.GetCost))
                            {
                                building.TakeBuildingBase(buildBase.GetPosition(0), buildBase.GetPosition(1));
                            }
                            else
                                Destroy(go);
                        }
                    }
                }
            }
        }

        void CheckInputForDelete()
        {
            if (Input.GetMouseButton(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "Building")
                    {
                        BuildingDefaultBehaviour building = hit.collider.GetComponent<BuildingDefaultBehaviour>();
                        if (!building) building = hit.collider.GetComponentInParent<BuildingDefaultBehaviour>();
                        building.Sell();
                    }
                }
            }
        }

        public void SwitchState(int index)
        {
            m_curOperationState = (OperationState)index;
            switch(m_curOperationState)
            {
                case OperationState.Add:
                    m_buildField.ShowBuildField();
                    buildingMenu.SetActive(true);
                    break;
                case OperationState.Delete:
                    m_buildField.ShowBuildField();
                    buildingMenu.SetActive(false);
                    break;
                case OperationState.None:
                    m_buildField.HideBuildField();
                    buildingMenu.SetActive(false);
                    break;
            }
        }

        public void TrainUnitInSelectedFactory(int unitID)
        {
            if (m_curSelectedBuilding)
            {
                FactoryBuildingBehaviour fac = m_curSelectedBuilding.GetComponent<FactoryBuildingBehaviour>();
                fac.TrainUnit(trainUnits[unitID]);
            }
        }

        public void ChangeCurrentBuildingIndex(int index)
        {
            m_curBuildingIndex = index;
        }
    }
}
