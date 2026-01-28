using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class JustPull : MonoBehaviour
{
    [SerializeField] VideoPlayer video3star;
    [SerializeField] VideoPlayer video5star;

    [SerializeField] UIDocument uiDocument;

    private Button pullButton;
    private Label gems;
    private int nbGems;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {

        pullButton = uiDocument.rootVisualElement.Q("Pull") as Button;
        this.gems =  uiDocument.rootVisualElement.Q<Label>("Gems");
        this.nbGems = Int32.Parse(this.gems.text.Split(" ")[2]);

        pullButton.RegisterCallback<ClickEvent>(Pull);
    }

    private void Update()
    {
        this.gems.text = "Gems : " + (nbGems).ToString();
    }

    private void Pull(ClickEvent evt)
    {
        Debug.Log(this.gems.text.Split(" ")[2]);
        if (nbGems >= 300)
        {
            nbGems-=300;
            StartCoroutine(Pull3());
        }
    }

    private IEnumerator Pull3()
    {
        Debug.Log("Test");
        this.GetComponent<UIDocument>().enabled = false;
        this.video3star.Play();
        yield return new WaitForSecondsRealtime(7);
        this.GetComponent<UIDocument>().enabled = true;

    }

    private IEnumerator Pull5()
    {
        Debug.Log("Test");
        this.GetComponent<UIDocument>().enabled = false;
        this.video5star.Play();
        yield return new WaitForSecondsRealtime(7);

        this.GetComponent<UIDocument>().enabled = true;

    }


}
