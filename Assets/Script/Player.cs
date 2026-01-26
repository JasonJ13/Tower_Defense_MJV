using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private int hp;

    private int money;

    public int GetHp()
    {
        return this.hp;
    }

    public int Damage(int damage)
    {
        this.hp = this.hp - damage;
        return this.hp;
    }

    public int GetMoney()
    {
        return this.money;
    }

    public int GainMoney(int moneyToAdd)
    {
        this.money = this.money + moneyToAdd;
        return this.money;
    }

    [SerializeField]
    Camera cameraComponent;

    [SerializeField]
    GameObject archerPrefab;

    [SerializeField]
    GameObject turretPrefab;

    [SerializeField]
    GameObject magePrefab;

    [SerializeField]
    GameObject cannonPrefab;

    [SerializeField]
    GameObject generatorPrefab;

    [SerializeField]
    GameObject archerNotBuiltPrefab;

    [SerializeField]
    GameObject turretNotBuiltPrefab;

    [SerializeField]
    GameObject mageNotBuiltPrefab;

    [SerializeField]
    GameObject cannonNotBuiltPrefab;

    [SerializeField]
    GameObject generatorNotBuiltPrefab;

    private InputAction addArcherAction;
    private InputAction addTouretAction;
    private InputAction addMageAction;
    private InputAction addCannonAction;
    private InputAction addGeneratorAction;
    private InputAction placeTowerAction;

    public enum TowerType
    {
        Empty,
        Archer,
        Turret,
        Mage,
        Cannon,
        Generator,
    }

    private Dictionary<TowerType, GameObject> typeToTowerNB =
        new Dictionary<TowerType, GameObject>();

    void defineTypeToTowerNB()
    {
        typeToTowerNB.Add(TowerType.Archer, archerNotBuiltPrefab);
        typeToTowerNB.Add(TowerType.Turret, turretNotBuiltPrefab);
        typeToTowerNB.Add(TowerType.Mage, mageNotBuiltPrefab);
        typeToTowerNB.Add(TowerType.Cannon, cannonNotBuiltPrefab);
        typeToTowerNB.Add(TowerType.Generator, generatorNotBuiltPrefab);
    }

    private Dictionary<TowerType, GameObject> typeToTower = new Dictionary<TowerType, GameObject>();

    void defineTypeToTower()
    {
        typeToTower.Add(TowerType.Archer, archerPrefab);
        typeToTower.Add(TowerType.Turret, turretPrefab);
        typeToTower.Add(TowerType.Mage, magePrefab);
        typeToTower.Add(TowerType.Cannon, cannonPrefab);
        typeToTower.Add(TowerType.Generator, generatorPrefab);
    }

    private bool constructible;
    private TowerType towerInHand = TowerType.Empty;
    private GameObject towerNB = null;
    private MeshRenderer tower_MeshRenderer = null;

    private void OnEnable()
    {
        defineTypeToTowerNB();
        defineTypeToTower();

        addArcherAction = InputSystem.actions.FindAction("Player/AddTower1");
        addTouretAction = InputSystem.actions.FindAction("Player/AddTower2");
        addMageAction = InputSystem.actions.FindAction("Player/AddTower3");
        addCannonAction = InputSystem.actions.FindAction("Player/AddTower4");
        addGeneratorAction = InputSystem.actions.FindAction("Player/AddTower5");
        placeTowerAction = InputSystem.actions.FindAction("Player/PlaceTower");
    }

    public void add_in_hand(TowerType newTower)
    {
        if (towerNB != null)
        {
            Destroy(towerNB);
        }

        if (newTower == TowerType.Empty)
        {
            return;
        }

        if (newTower != towerInHand)
        {
            towerNB = Instantiate(typeToTowerNB[newTower]);
            towerInHand = newTower;
            constructible = false;
            tower_MeshRenderer = towerNB
                .GetComponent<Transform>()
                .GetChild(0)
                .GetComponent<MeshRenderer>();
        }
        else
        {
            towerInHand = TowerType.Empty;
        }
    }

    private void add_to_map(TowerType newTower)
    {
        Assert.IsTrue(newTower != TowerType.Empty);

        Instantiate(typeToTower[newTower], towerNB.transform.position, towerNB.transform.rotation);

        if (newTower == TowerType.Generator)
        {
            Map.Instance.SetMapArray(
                Map.Instance.PositionToCoords(towerNB.transform.position),
                Map.TileType.generator
            );
        }
        else
        {
            Map.Instance.SetMapArray(
                Map.Instance.PositionToCoords(towerNB.transform.position),
                Map.TileType.construct
            );
        }
    }

    private void Update()
    {
        if (addArcherAction.WasPerformedThisFrame())
        {
            add_in_hand(TowerType.Archer);
        }
        else if (addTouretAction.WasPerformedThisFrame())
        {
            add_in_hand(TowerType.Turret);
        }
        else if (addMageAction.WasPerformedThisFrame())
        {
            add_in_hand(TowerType.Mage);
        }
        else if (addCannonAction.WasPerformedThisFrame())
        {
            add_in_hand(TowerType.Cannon);
        }
        else if (addGeneratorAction.WasPerformedThisFrame())
        {
            add_in_hand(TowerType.Generator);
        }

        if (constructible && placeTowerAction.WasPerformedThisFrame())
        {
            add_to_map(towerInHand);
            Destroy(towerNB);
            constructible = false;
            towerInHand = TowerType.Empty;
        }
    }

    private void FixedUpdate()
    {
        if (towerInHand != TowerType.Empty)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = cameraComponent.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                towerNB.transform.position = hitInfo.transform.position;

                if (
                    !constructible
                    && Map.Instance.GetMapArrayCoords(
                        Map.Instance.PositionToCoords(hitInfo.transform.position)
                    ) == Map.TileType.constructible
                )
                {
                    constructible = true;
                    tower_MeshRenderer.materials[0].color = Color.green;
                }
                else if (
                    constructible
                    && !(
                        Map.Instance.GetMapArrayCoords(
                            Map.Instance.PositionToCoords(hitInfo.transform.position)
                        ) == Map.TileType.constructible
                    )
                )
                {
                    constructible = false;
                    tower_MeshRenderer.materials[0].color = Color.red;
                }
            }
            else
            {
                constructible = false;
                if (tower_MeshRenderer != null)
                {
                    tower_MeshRenderer.materials[0].color = Color.red;
                }
            }
        }
    }
}
