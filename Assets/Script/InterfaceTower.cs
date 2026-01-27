using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    private Button archerTowerButton;
    private Button turretTowerButton;
    private Button mageTowerButton;
    private Button cannonTowerButton;
    private Button generatorButton;
    private Button dropOutButton;

    private Player playerScript;

    void OnEnable()
    {
        var uiDocument = this.GetComponent<UIDocument>();
        playerScript = GameObject.Find("Player").GetComponent<Player>();

        archerTowerButton = uiDocument.rootVisualElement.Q("Archer") as Button;
        turretTowerButton = uiDocument.rootVisualElement.Q("Turret") as Button;
        mageTowerButton = uiDocument.rootVisualElement.Q("Mage") as Button;
        cannonTowerButton = uiDocument.rootVisualElement.Q("Cannon") as Button;
        generatorButton = uiDocument.rootVisualElement.Q("Generator") as Button;
        dropOutButton = uiDocument.rootVisualElement.Q("DropOut") as Button;

        archerTowerButton.RegisterCallback<ClickEvent>(ArcherSelected);
        turretTowerButton.RegisterCallback<ClickEvent>(TurretSelected);
        mageTowerButton.RegisterCallback<ClickEvent>(MageSelected);
        cannonTowerButton.RegisterCallback<ClickEvent>(CannonSelected);
        generatorButton.RegisterCallback<ClickEvent>(GeneratorSelected);
        dropOutButton.RegisterCallback<ClickEvent>(DropOutSelected);
    }

    void ArcherSelected(ClickEvent evt)
    {
        playerScript.add_in_hand(Player.TowerType.Archer);
    }

    void TurretSelected(ClickEvent evt)
    {
        playerScript.add_in_hand(Player.TowerType.Turret);
    }

    void MageSelected(ClickEvent evt)
    {
        playerScript.add_in_hand(Player.TowerType.Mage);
    }

    void CannonSelected(ClickEvent evt)
    {
        playerScript.add_in_hand(Player.TowerType.Cannon);
    }

    void GeneratorSelected(ClickEvent evt)
    {
        playerScript.add_in_hand(Player.TowerType.Generator);
    }

    void DropOutSelected(ClickEvent evt)
    {
        playerScript.add_in_hand(Player.TowerType.Empty);
    }
}
