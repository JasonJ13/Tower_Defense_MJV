using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    private Button offensifTowerButton;
    private Button GeneratorButton;
    private Button DropOutButton;

    void OnEnable()
    {
        var uiDocument = this.GetComponent<UIDocument>();
        var cameraScript = GameObject.Find("Main Camera").GetComponent<CameraMouvement>();

        offensifTowerButton = uiDocument.rootVisualElement.Q("Offensif") as Button;
        GeneratorButton = uiDocument.rootVisualElement.Q("Generator") as Button;
        DropOutButton = uiDocument.rootVisualElement.Q("DopOut") as Button;

        /*offensifTowerButton.onClick.AddListener(() =>
        {
            cameraScript.add_in_hand(CameraMouvement.TowerType.Offensif);
        });

        GeneratorButton.RegisterCallback<ClickEvent>(
            cameraScript.add_in_hand,
            CameraMouvement.TowerType.Generator,
            TrickleDown.TrickleDown
        );
        DropOutButton.RegisterCallback<ClickEvent>(
            cameraScript.add_in_hand,
            CameraMouvement.TowerType.empty,
            TrickleDown.TrickleDown
        );*/
    }
}
