using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameOver : MonoBehaviour
{
    private Button restartButton;
    private Label score;
    private Button quitButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        var uiDocument = this.GetComponent<UIDocument>();

        restartButton = uiDocument.rootVisualElement.Q("Start") as Button;
        score = uiDocument.rootVisualElement.Q("Score") as Label;
        int nbScore = Player.Instance.GetScore();
        Debug.Log("Player score : " + nbScore.ToString());
        score.text = "Score : " + nbScore.ToString();
        quitButton = uiDocument.rootVisualElement.Q("Quit") as Button;

        restartButton.RegisterCallback<ClickEvent>(restart);
        quitButton.RegisterCallback<ClickEvent>(quit);
    }

    private void restart(ClickEvent evt)
    {
        var scene = SceneManager.GetActiveScene().name;
        App.Instance.Restart();
        Destroy(this.transform.gameObject);
        
    }

    private void quit(ClickEvent evt)
    {
        App.Instance.QuitGameOver();

    }
}
