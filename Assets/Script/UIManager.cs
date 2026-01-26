using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    private Button offensifTowerButton;
    private Button GeneratorButton;
    private Button DropOutButton;

    private Player playerScript;

    void OnEnable()
    {
        var uiDocument = this.GetComponent<UIDocument>();
        playerScript = GameObject.Find("Player").GetComponent<Player>();

        offensifTowerButton = uiDocument.rootVisualElement.Q("Offensif") as Button;
        GeneratorButton = uiDocument.rootVisualElement.Q("Generator") as Button;
        DropOutButton = uiDocument.rootVisualElement.Q("DropOut") as Button;

        offensifTowerButton.RegisterCallback<ClickEvent>(OffensifSelected);
        GeneratorButton.RegisterCallback<ClickEvent>(GeneratorSelected);
        DropOutButton.RegisterCallback<ClickEvent>(DropOutSelected);
    }

    void OffensifSelected(ClickEvent evt)
    {
        playerScript.add_in_hand(Player.TowerType.Offensif);
    }

    void GeneratorSelected(ClickEvent evt)
    {
        playerScript.add_in_hand(Player.TowerType.Generator);
    }

    void DropOutSelected(ClickEvent evt)
    {
        playerScript.add_in_hand(Player.TowerType.empty);
    }
}
