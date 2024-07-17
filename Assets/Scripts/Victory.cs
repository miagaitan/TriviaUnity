using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Victory : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _pointsText;

    void Update()
    {
        _pointsText.text = UIManagment.Instance.currentPoints.ToString("f0");
    }
    public void MenuButton()
    {
        Destroy(GameManager.Instance);
        Destroy(UIManagment.Instance);
        SceneManager.LoadScene("TriviaSelectScene");
    }
}