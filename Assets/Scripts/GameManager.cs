using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<question> responseList;
    public int currentTriviaIndex = 0;
    public int randomQuestionIndex = 0;
    public List<string> _answers = new List<string>();
    public bool queryCalled;
    private int _points;
    public int _numQuestionAnswered = 0;
    string _correctAnswer;
    public static GameManager Instance { get; private set; }
    private HashSet<int> _askedQuestions = new HashSet<int>();

    void Awake()
    {
        // Configura la instancia
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartTrivia();
        queryCalled = false;
    }

    void StartTrivia()
    {

    }

    public void CategoryAndQuestionQuery(bool isCalled)
    {
        isCalled = UIManagment.Instance.queryCalled;

        if (!isCalled)
        {
            do
            {
                randomQuestionIndex = Random.Range(0, responseList.Count);
            } while (_askedQuestions.Contains(randomQuestionIndex));

            _correctAnswer = responseList[randomQuestionIndex].CorrectOption;

            _answers.Clear();
            _answers.Add(responseList[randomQuestionIndex].Answer1);
            _answers.Add(responseList[randomQuestionIndex].Answer2);
            _answers.Add(responseList[randomQuestionIndex].Answer3);

            _answers.Shuffle();

            for (int i = 0; i < UIManagment.Instance._buttons.Length; i++)
            {
                UIManagment.Instance._buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = _answers[i];

                int index = i;
                UIManagment.Instance._buttons[i].onClick.RemoveAllListeners(); 
                UIManagment.Instance._buttons[i].onClick.AddListener(() => UIManagment.Instance.OnButtonClick(index));
            }

            _askedQuestions.Add(randomQuestionIndex); 
            UIManagment.Instance.queryCalled = true;
        }
    }

    private void Update()
    {
        
    }
}
