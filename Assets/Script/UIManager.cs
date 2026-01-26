using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    private Button offensifTowerButton;
    private Button GeneratorButton;
    private Button DropOutButton;

    private CameraMouvement cameraScript;

    void OnEnable()
    {
        var uiDocument = this.GetComponent<UIDocument>();
        cameraScript = GameObject.Find("Main Camera").GetComponent<CameraMouvement>();

        offensifTowerButton = uiDocument.rootVisualElement.Q("Offensif") as Button;
        GeneratorButton = uiDocument.rootVisualElement.Q("Generator") as Button;
        DropOutButton = uiDocument.rootVisualElement.Q("DropOut") as Button;

        offensifTowerButton.RegisterCallback<ClickEvent>(OffensifSelected);
        GeneratorButton.RegisterCallback<ClickEvent>(GeneratorSelected);
        DropOutButton.RegisterCallback<ClickEvent>(DropOutSelected);
    }

    void OffensifSelected(ClickEvent evt)
    {
        cameraScript.add_in_hand(CameraMouvement.TowerType.Offensif);
    }

    void GeneratorSelected(ClickEvent evt)
    {
        cameraScript.add_in_hand(CameraMouvement.TowerType.Generator);
    }

    void DropOutSelected(ClickEvent evt)
    {
        cameraScript.add_in_hand(CameraMouvement.TowerType.empty);
    }
}
