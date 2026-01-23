using System.Collections;
using System.Diagnostics;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMouvement : MonoBehaviour
{
    protected Map map;

    [SerializeField]
    float SPEED = 20f;

    [SerializeField]
    Vector2 cameraBorder = new Vector2(12, 12);

    [SerializeField]
    Vector2 zoomLimit = new Vector2(2, 24);

    [SerializeField]
    GameObject towerPrefab;

    [SerializeField]
    GameObject generatorPrefab;

    [SerializeField]
    GameObject towerConstructablePrefab;

    [SerializeField]
    GameObject generatorConstructablePrefab;

    private Camera cameraComponent;
    private Transform cameraTransform;

    private InputAction moveAction;
    private InputAction zoomAction;
    private InputAction addTowerAction1;
    private InputAction addTowerAction2;
    private InputAction placeTowerAction;

    private Vector2 mouvement;
    private float zoom;

    private bool towerInHand;
    private bool constructible;
    private GameObject tower;
    private MeshRenderer tower_MeshRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        cameraTransform = GetComponent<Transform>();
        cameraComponent = GetComponent<Camera>();

        moveAction = InputSystem.actions.FindAction("Player/Move");
        zoomAction = InputSystem.actions.FindAction("Player/Zoom");
        addTowerAction1 = InputSystem.actions.FindAction("Player/AddTower1");
        addTowerAction2 = InputSystem.actions.FindAction("Player/AddTower2");
        placeTowerAction = InputSystem.actions.FindAction("Player/PlaceTower");
    }

    private void Awake()
    {
        towerInHand = false;
    }

    private Vector3 define_translate(Vector2 mouvement, float zoom)
    {
        if (
            (cameraTransform.position.x + mouvement.x > cameraBorder.x && mouvement.x > 0)
            || (cameraTransform.position.x + mouvement.x < -cameraBorder.x && mouvement.x < 0)
        )
        {
            mouvement.x = 0;
        }

        if (
            (cameraTransform.position.z + mouvement.y > cameraBorder.y && mouvement.y > 0)
            || (cameraTransform.position.z + mouvement.y < -cameraBorder.y && mouvement.y < 0)
        )
        {
            mouvement.y = 0;
        }

        if (
            (cameraTransform.position.y + zoom < zoomLimit.x && zoom > 0)
            || (cameraTransform.position.y + zoom > zoomLimit.y && zoom < 0)
        )
        {
            zoom = 0;
        }

        return new Vector3(mouvement.x, mouvement.y, zoom);
    }

    // Update is called once per frame
    private void Update()
    {
        //Mouvement de la caméra
        mouvement = moveAction.ReadValue<Vector2>() * SPEED * Time.deltaTime;
        zoom = zoomAction.ReadValue<float>() * SPEED * Time.deltaTime / 2;
        Vector3 translate = define_translate(mouvement, zoom);
        cameraTransform.Translate(translate);

        //Gestion d'une nouvelle tour
        if (addTowerAction1.WasPerformedThisFrame() || addTowerAction2.WasPerformedThisFrame())
        {
            if (!towerInHand)
            {
                towerInHand = true;

                if (addTowerAction1.WasPerformedThisFrame())
                {
                    tower = Instantiate(towerConstructablePrefab);
                }
                else
                {
                    tower = Instantiate(generatorConstructablePrefab);
                }

                constructible = false;
                tower_MeshRenderer = tower
                    .GetComponent<Transform>()
                    .GetChild(0)
                    .GetComponent<MeshRenderer>();
            }
            else
            {
                towerInHand = false;
                Destroy(tower);
            }
        }

        if (constructible && placeTowerAction.WasPerformedThisFrame())
        {
            constructible = false;
            towerInHand = false;

            Instantiate(towerPrefab, tower.transform.position, tower.transform.rotation);
            Destroy(tower);
        }
    }

    private void FixedUpdate()
    {
        if (towerInHand)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = cameraComponent.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                tower.transform.position = hitInfo.transform.position;

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
        }
    }
}
