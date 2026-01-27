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
    GameObject towerPrefab;

    [SerializeField]
    GameObject generatorPrefab;

    [SerializeField]
    GameObject towerNotBuiltlePrefab;

    [SerializeField]
    GameObject generatorNotBuiltPrefab;

    private InputAction addTowerAction1;
    private InputAction addTowerAction2;
    private InputAction placeTowerAction;

    public enum TowerType
    {
        empty,
        Offensif,
        Generator,
    }

    private Dictionary<TowerType, GameObject> typeToTower = new Dictionary<TowerType, GameObject>();

    void defineTypeToTower()
    {
        typeToTower.Add(TowerType.Offensif, towerNotBuiltlePrefab);
        typeToTower.Add(TowerType.Generator, generatorPrefab);
    }

    private bool constructible;
    private TowerType towerInHand = TowerType.empty;
    private GameObject towerNB = null;
    private MeshRenderer tower_MeshRenderer = null;

    private void OnEnable()
    {
        defineTypeToTower();

        addTowerAction1 = InputSystem.actions.FindAction("Player/AddTower1");
        addTowerAction2 = InputSystem.actions.FindAction("Player/AddTower2");
        placeTowerAction = InputSystem.actions.FindAction("Player/PlaceTower");
    }

    public void add_in_hand(TowerType newTower)
    {
        if (towerNB != null)
        {
            Destroy(towerNB);
        }

        if (newTower != towerInHand && newTower != TowerType.empty)
        {
            towerNB = Instantiate(typeToTower[newTower]);
            towerInHand = newTower;
            constructible = false;
            tower_MeshRenderer = towerNB
                .GetComponent<Transform>()
                .GetChild(0)
                .GetComponent<MeshRenderer>();
        }
        else
        {
            towerInHand = TowerType.empty;
        }
    }

    private void Update()
    {
        if (addTowerAction1.WasPerformedThisFrame())
        {
            add_in_hand(TowerType.Offensif);
        }
        else if (addTowerAction2.WasPerformedThisFrame())
        {
            add_in_hand(TowerType.Generator);
        }

        if (constructible && placeTowerAction.WasPerformedThisFrame())
        {
            Assert.IsTrue(towerInHand != TowerType.empty);
            switch (towerInHand)
            {
                case TowerType.Offensif:
                    Instantiate(
                        towerPrefab,
                        towerNB.transform.position,
                        towerNB.transform.rotation
                    );
                    Map.Instance.SetMapArray(
                        Map.Instance.PositionToCoords(towerNB.transform.position),
                        Map.TileType.construct
                    );
                    break;

                case TowerType.Generator:
                    Instantiate(
                        generatorPrefab,
                        towerNB.transform.position,
                        towerNB.transform.rotation
                    );
                    Map.Instance.SetMapArray(
                        Map.Instance.PositionToCoords(towerNB.transform.position),
                        Map.TileType.generator
                    );
                    break;
            }
            Destroy(towerNB);
            constructible = false;
            towerInHand = TowerType.empty;
        }
    }

    private void FixedUpdate()
    {
        if (towerInHand != TowerType.empty)
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
                tower_MeshRenderer.materials[0].color = Color.red;
            }
        }
    }
}
