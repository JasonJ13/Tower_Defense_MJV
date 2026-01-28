using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class JustPull : MonoBehaviour
{
    [SerializeField] VideoPlayer video3star;
    [SerializeField] VideoPlayer video5star;

    private Button pullButton;
    private Label gems;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        var uiDocument = this.GetComponent<UIDocument>();

        pullButton = uiDocument.rootVisualElement.Q("Pull") as Button;
        gems = uiDocument.rootVisualElement.Q("Gems") as Label;

        pullButton.RegisterCallback<ClickEvent>(Pull);
    }


    private void Pull(ClickEvent evt)
    {
        Debug.Log(this.gems.text.Split(" ")[2]);
        int nbgems = Int32.Parse(this.gems.text.Split(" ")[2]);
        if (nbgems > 200)
        {
            this.gems.text = "Gems : " + (nbgems-200).ToString();
            StartCoroutine(Pull3());
        }
    }

    private IEnumerator Pull3()
    {
        Debug.Log("Test");
        this.GetComponent<UIDocument>().enabled = false;
        this.video3star.Play();
        yield return new WaitForSecondsRealtime(8);
        this.GetComponent<UIDocument>().enabled = true;

    }

    private IEnumerator Pull5()
    {
        Debug.Log("Test");
        this.GetComponent<UIDocument>().enabled = false;
        this.video5star.Play();
        yield return new WaitForSecondsRealtime(8);
        this.GetComponent<UIDocument>().enabled = true;

    }


}
