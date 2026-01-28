using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    private void Awake()
    {
        if (Player.Instance != null)
        {
            Debug.LogError("Error : Instance of Player already exists");
        }
        Player.Instance = this;
    }

    [SerializeField]
    private int hp = 100;

    private bool dead = false;

    private int score = 0;

    [SerializeField]
    private int money = 100;

    [SerializeField]
    private TowerUI towerUI;

    public int GetScore()
    {
        return this.score;
    }

    public void AddScore(int value)
    {
        towerUI.ChangeScoreText(this.score);
        this.score += value;
    }

    public int GetHp()
    {
        return this.hp;
    }

    public int Damage(int damage)
    {
        this.hp = this.hp - damage;
        towerUI.ChangeLifeText(this.hp);
        return this.hp;
    }

    public int GetMoney()
    {
        return this.money;
    }

    public int GainMoney(int moneyToAdd)
    {
        this.money = this.money + moneyToAdd;
        towerUI.ChangeMoneyText(this.money);
        return this.money;
    }

    public bool AsMoney(int needed)
    {
        return this.money >= needed;
    }

    public void SpendMoney(int moneySpended)
    {
        this.money = this.money - moneySpended;
        towerUI.ChangeMoneyText(this.money);
    }

    [SerializeField]
    private Camera cameraComponent;

    [SerializeField]
    private GameObject archerPrefab;

    [SerializeField]
    private GameObject turretPrefab;

    [SerializeField]
    private GameObject magePrefab;

    [SerializeField]
    private GameObject cannonPrefab;

    [SerializeField]
    private GameObject generatorPrefab;

    [SerializeField]
    private GameObject archerNotBuiltPrefab;

    [SerializeField]
    private GameObject turretNotBuiltPrefab;

    [SerializeField]
    private GameObject mageNotBuiltPrefab;

    [SerializeField]
    private GameObject cannonNotBuiltPrefab;

    [SerializeField]
    private GameObject generatorNotBuiltPrefab;

    [SerializeField]
    private int archerCost = 10;

    [SerializeField]
    private int turretCost = 10;

    [SerializeField]
    private int mageCost = 20;

    [SerializeField]
    private int cannonCost = 10;

    [SerializeField]
    private int generatorCost = 5;

    private InputAction addArcherAction;
    private InputAction addTouretAction;
    private InputAction addMageAction;
    private InputAction addCannonAction;
    private InputAction addGeneratorAction;
    private InputAction placeTowerAction;

    private Color red = new Color(1f, 0, 0, 0.3f);
    private Color green = new Color(0, 1f, 0, 0.3f);

    public enum TowerType
    {
        Empty,
        Archer,
        Turret,
        Mage,
        Cannon,
        Generator,
    }

    private Dictionary<Map.coords, TowerType> dictCoordsTower =
        new Dictionary<Map.coords, TowerType>();

    public Dictionary<Map.coords, TowerType> GetDictCoordsTower()
    {
        return dictCoordsTower;
    }

    private Dictionary<Map.coords, OffensifTower> dictCoordsOffensifTower =
        new Dictionary<Map.coords, OffensifTower>();

    public Dictionary<Map.coords, OffensifTower> GetDictCoordsOffensifTower()
    {
        return dictCoordsOffensifTower;
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

    private Dictionary<TowerType, int> typeToMoney = new Dictionary<TowerType, int>();

    void defineTypeToMoney()
    {
        typeToMoney.Add(TowerType.Archer, archerCost);
        typeToMoney.Add(TowerType.Turret, turretCost);
        typeToMoney.Add(TowerType.Mage, mageCost);
        typeToMoney.Add(TowerType.Cannon, cannonCost);
        typeToMoney.Add(TowerType.Generator, generatorCost);
        typeToMoney.Add(TowerType.Empty, 0);
    }

    private bool constructible;
    private TowerType towerInHand = TowerType.Empty;
    private GameObject towerNB = null;
    private MeshRenderer tower_MeshRenderer = null;
    private TowerNotBuilt towerNBScript = null;

    private void OnEnable()
    {
        defineTypeToTowerNB();
        defineTypeToTower();
        defineTypeToMoney();

        addArcherAction = InputSystem.actions.FindAction("Player/AddTower1");
        addTouretAction = InputSystem.actions.FindAction("Player/AddTower2");
        addMageAction = InputSystem.actions.FindAction("Player/AddTower3");
        addCannonAction = InputSystem.actions.FindAction("Player/AddTower4");
        addGeneratorAction = InputSystem.actions.FindAction("Player/AddTower5");
        placeTowerAction = InputSystem.actions.FindAction("Player/PlaceTower");

        InputSystem.actions.FindActionMap("Player").Enable();

        towerUI.ChangeLifeText(this.hp);
        towerUI.ChangeMoneyText(this.money);
        towerUI.ChangeScoreText(this.score);
    }

    public void add_in_hand(TowerType newTower)
    {
        if (towerNB != null)
        {
            Destroy(towerNB);
        }

        if (newTower == TowerType.Empty)
        {
            towerInHand = TowerType.Empty;
            return;
        }

        if (newTower != towerInHand && AsMoney(typeToMoney[newTower] - typeToMoney[towerInHand]))
        {
            GainMoney(typeToMoney[towerInHand]);
            towerNB = Instantiate(
                typeToTowerNB[newTower],
                Map.Instance.CoordsToPosition(new Map.coords(0, 0)),
                transform.rotation
            );
            towerInHand = newTower;
            constructible = false;
            tower_MeshRenderer = towerNB
                .GetComponent<Transform>()
                .GetChild(0)
                .GetComponent<MeshRenderer>();
            towerNBScript = towerNB.GetComponent<TowerNotBuilt>();
            towerNBScript.positionGhostTile();

            SpendMoney(typeToMoney[newTower]);
        }
        else if (newTower == towerInHand)
        {
            towerInHand = TowerType.Empty;
            GainMoney(typeToMoney[newTower]);
        }
    }

    private void add_to_map(TowerType newTower)
    {
        Assert.IsTrue(newTower != TowerType.Empty);

        OffensifTower t = Instantiate(
                typeToTower[newTower],
                towerNB.transform.position,
                towerNB.transform.rotation
            )
            .GetComponent<OffensifTower>();

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
            dictCoordsTower.Add(
                Map.Instance.PositionToCoords(towerNB.transform.position),
                newTower
            );
            dictCoordsOffensifTower.Add(
                Map.Instance.PositionToCoords(towerNB.transform.position),
                t
            );
        }
    }

    private void Update()
    {
        if (this.hp <= 0 && !this.dead)
        {
            this.dead = true;
            App.Instance.GameOverScreen();
        }
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
                towerNBScript.positionGhostTile();

                if (
                    !constructible
                    && Map.Instance.GetMapArrayCoords(
                        Map.Instance.PositionToCoords(hitInfo.transform.position)
                    ) == Map.TileType.constructible
                )
                {
                    constructible = true;
                    tower_MeshRenderer.materials[0].color = green;
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
                    tower_MeshRenderer.materials[0].color = red;
                }
            }
            else
            {
                constructible = false;
                if (tower_MeshRenderer != null)
                {
                    tower_MeshRenderer.materials[0].color = red;
                }
            }
        }
    }

    public void supplieTower(Map.coords tile)
    {
        dictCoordsOffensifTower[tile].connected();
    }
}
