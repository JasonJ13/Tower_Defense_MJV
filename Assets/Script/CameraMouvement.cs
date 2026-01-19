using System.Collections;
using System.Diagnostics;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMouvement : MonoBehaviour
{
    [SerializeField]
    float SPEED = 20f;

    [SerializeField]
    Vector2 cameraBorder = new Vector2(12, 12);

    [SerializeField]
    Vector2 zoomLimit = new Vector2(2, 24);

    [SerializeField]
    GameObject towerPrefab;

    private Camera cameraComponent;
    private Transform cameraTransform;

    private InputAction moveAction;
    private InputAction zoomAction;
    private InputAction addTowerAction;
    private InputAction placeTowerAction;

    private Vector2 mouvement;
    private float zoom;

    private bool towerInHand;
    private Transform tower;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        cameraTransform = GetComponent<Transform>();
        cameraComponent = GetComponent<Camera>();
        moveAction = InputSystem.actions.FindAction("Player/Move");
        zoomAction = InputSystem.actions.FindAction("Player/Zoom");
        addTowerAction = InputSystem.actions.FindAction("Player/AddTower");
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
        if (addTowerAction.WasPerformedThisFrame())
        {
            if (!towerInHand)
            {
                towerInHand = true;
                Instantiate(towerPrefab, cameraTransform, false);
                newTowerOnCamera();
            }
            else
            {
                towerInHand = false;
            }
        }
    }

    private void newTowerOnCamera()
    {
        tower = cameraTransform.GetChild(0);
        //tower.position.y -= cameraTransform.position.y;
    }

    private void FixedUpdate()
    {
        if (towerInHand)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = cameraComponent.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                string name = hitInfo.collider.name;
            }
        }
    }
}
