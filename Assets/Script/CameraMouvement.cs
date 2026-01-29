using System.Collections;
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

    private Transform cameraTransform;

    private InputAction moveAction;
    private InputAction zoomAction;

    private Vector2 mouvement;
    private float zoom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private IEnumerator Start()
    {
        cameraTransform = GetComponent<Transform>();

        moveAction = InputSystem.actions.FindAction("Player/Move");
        zoomAction = InputSystem.actions.FindAction("Player/Zoom");

        while (Map.Instance.height == 0)
        {
            yield return null;
        }
        
        cameraBorder = new Vector2(Map.Instance.height, Map.Instance.width);            

    }

    private Vector3 define_translate(Vector2 mouvement, float zoom)
    {
        if (
            (cameraTransform.position.x + mouvement.x > cameraBorder.x && mouvement.x > 0)
            || (cameraTransform.position.x + mouvement.x < 0 && mouvement.x < 0)
        )
        {
            mouvement.x = 0;
        }

        if (
            (cameraTransform.position.z + mouvement.y > cameraBorder.y && mouvement.y > 0)
            || (cameraTransform.position.z + mouvement.y < 0 && mouvement.y < 0)
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
    }
}
