using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System;
using UnityEngine.UIElements;

public class CameraMouvement : MonoBehaviour
{

    [SerializeField] float SPEED = 20f;

    private GameObject cameraNode;
    private InputAction moveAction;
    private InputAction zoomAction;

    private Vector2 mouvement;
    private float zoom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraNode = get_CameraNode();
        moveAction = InputSystem.actions.FindAction("Player/Move");
        zoomAction = InputSystem.actions.FindAction("Player/Zoom");
    }

    // Update is called once per frame
    void Update()
    {
        
        mouvement = moveAction.ReadValue<Vector2>()*SPEED * Time.deltaTime;
        zoom = zoomAction.ReadValue<float>()*SPEED * Time.deltaTime;
        cameraNode.transform.Translate(mouvement.x,mouvement.y,zoom);


    }

    protected GameObject get_CameraNode()
    {
        return GameObject.Find("Main Camera");
    }
}
