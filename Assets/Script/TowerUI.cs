using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class TowerUI : MonoBehaviour
{
    private Button archerTowerButton;
    private Button turretTowerButton;
    private Button mageTowerButton;
    private Button cannonTowerButton;
    private Button generatorButton;
    private Button dropOutButton;

    private Label lifeText;
    private Label moneyText;
    private Label scoreText;

    private Player playerScript;

    private void Start()
    {
        var uiDocument = this.GetComponent<UIDocument>();
        this.playerScript = GameObject.Find("Player").GetComponent<Player>();

        this.archerTowerButton = uiDocument.rootVisualElement.Q("Archer") as Button;
        this.turretTowerButton = uiDocument.rootVisualElement.Q("Turret") as Button;
        this.mageTowerButton = uiDocument.rootVisualElement.Q("Mage") as Button;
        this.cannonTowerButton = uiDocument.rootVisualElement.Q("Cannon") as Button;
        this.generatorButton = uiDocument.rootVisualElement.Q("Generator") as Button;
        this.dropOutButton = uiDocument.rootVisualElement.Q("DropOut") as Button;

        this.archerTowerButton.RegisterCallback<ClickEvent>(ArcherSelected);
        this.turretTowerButton.RegisterCallback<ClickEvent>(TurretSelected);
        this.mageTowerButton.RegisterCallback<ClickEvent>(MageSelected);
        this.cannonTowerButton.RegisterCallback<ClickEvent>(CannonSelected);
        this.generatorButton.RegisterCallback<ClickEvent>(GeneratorSelected);
        this.dropOutButton.RegisterCallback<ClickEvent>(DropOutSelected);

        this.lifeText = uiDocument.rootVisualElement.Q("Life") as Label;
        this.moneyText = uiDocument.rootVisualElement.Q("Money") as Label;
        this.scoreText = uiDocument.rootVisualElement.Q("Score") as Label;

        Debug.Log(this.lifeText != null);
    }

    private void ArcherSelected(ClickEvent evt)
    {
        this.playerScript.add_in_hand(Player.TowerType.Archer);
    }

    private void TurretSelected(ClickEvent evt)
    {
        this.playerScript.add_in_hand(Player.TowerType.Turret);
    }

    private void MageSelected(ClickEvent evt)
    {
        this.playerScript.add_in_hand(Player.TowerType.Mage);
    }

    private void CannonSelected(ClickEvent evt)
    {
        this.playerScript.add_in_hand(Player.TowerType.Cannon);
    }

    private void GeneratorSelected(ClickEvent evt)
    {
        this.playerScript.add_in_hand(Player.TowerType.Generator);
    }

    private void DropOutSelected(ClickEvent evt)
    {
        this.playerScript.add_in_hand(Player.TowerType.Empty);
    }

    public void ChangeLifeText(int newLife)
    {
        if (this.lifeText != null)
        {
            this.lifeText.text = "life : " + newLife;
        }
    }

    public void ChangeMoneyText(int newMoney)
    {
        if (this.moneyText != null)
        {
            this.moneyText.text = "Money : " + newMoney;
        }
    }

    public void ChangeScoreText(int newScore)
    {
        if (this.scoreText != null)
        {
            this.scoreText.text = "Score : " + newScore;
        }
    }
}
