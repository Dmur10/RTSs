using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RTSGame.UI.HUD
{
    public class ActionFrame : MonoBehaviour
    {
        public static ActionFrame instance = null;

        [SerializeField] private Button actionButtonUnit = null;
        [SerializeField] private Button actionButtonBuilding = null;

        [SerializeField] private Transform layoutGroup = null;

        private List<Button> buttons = new List<Button>();
        private PlayerActions actionList = null;

        public List<float> spawnQueue = new List<float>();
        public List<GameObject> spawnOrder = new List<GameObject>();

        public GameObject spawnPoint = null;

        private void Awake()
        {
                instance = this;
        
        }

        public void SetActionButtons(PlayerActions actions, GameObject spawnLocation)
        {
            actionList = actions;
            spawnPoint = spawnLocation;

            if (actions.basicUnits.Count>0)
            {
                foreach(Units.BasicUnit unit in actions.basicUnits)
                {
                    Button btn = Instantiate(actionButtonUnit, layoutGroup);
                    btn.name = unit.name;

                    buttons.Add(btn);
                }
            }
            if (actions.basicBuildings.Count > 0)
            {
                foreach (Buildings.BasicBuilding building in actions.basicBuildings)
                {
                    Button btn = Instantiate(actionButtonBuilding, layoutGroup);
                    btn.name = building.name;

                    buttons.Add(btn);
                }
            }

        }

        public void ClearActions()
        {
            foreach(Button btn in buttons)
            { 
                Destroy(btn.gameObject);
            }
            buttons.Clear();
        }

       public void StartSpawnTimer(string objectToSpawn)
        {
            if (IsUnit(objectToSpawn))
            {
                Units.BasicUnit unit = IsUnit(objectToSpawn);
                spawnQueue.Add(unit.SpawnTime);
                spawnOrder.Add(unit.HumanPrefab);
            }
            else
            {
                Debug.Log($"{objectToSpawn} is not spawnable");
            }

            if (spawnQueue.Count == 1) 
            {
                ActionTimer.instance.StartCoroutine(ActionTimer.instance.SpawnQueueTimer());
            } else if (spawnQueue.Count == 0)
            {
                ActionTimer.instance.StopAllCoroutines();
            }
            
        }

        private Units.BasicUnit IsUnit(string name)
        {
            foreach (Units.BasicUnit unit in actionList.basicUnits)
            {
                if (unit.name == name)
                {
                    return unit;
                }
            }
            return null;
        }

        private Buildings.BasicBuilding IsBuilding(string name)
        {
            foreach (Buildings.BasicBuilding building in actionList.basicBuildings)
            {
                if (building.name == name)
                {
                    return building;
                }
            }
            return null;
        }

        public void SpawnObject()
        {
            GameObject spawnedObject = Instantiate(spawnOrder[0], new Vector3(spawnPoint.transform.parent.position.x,spawnPoint.transform.parent.position.y,spawnPoint.transform.parent.position.z), Quaternion.identity);

            Units.Player.PlayerUnit pu = spawnedObject.GetComponent<Units.Player.PlayerUnit>();
            pu.transform.SetParent(GameObject.Find(pu.unitType.type.ToString() + "s").transform);

            spawnedObject.GetComponent<Units.Player.PlayerUnit>().MoveUnit(spawnPoint.transform.position);
            spawnQueue.Remove(spawnQueue[0]);
            spawnOrder.Remove(spawnOrder[0]);
        }
    }

   
   
}
