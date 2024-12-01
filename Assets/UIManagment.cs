using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManagment : MonoBehaviour
{
    string supabaseUrl = "https://vdmvxiswfvbmrcadujzt.supabase.co";
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InZkbXZ4aXN3ZnZibXJjYWR1anp0Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzIxNDYzNjksImV4cCI6MjA0NzcyMjM2OX0.EJJpHJsRidOCyICa7fN7tGdHx7o6BkxLYc6VxgGztfI";

    Supabase.Client clientSupabase;

    [SerializeField] TextMeshProUGUI _categoryText;
    [SerializeField] TextMeshProUGUI _questionText;
    [SerializeField] TextMeshProUGUI _timerText;
    [SerializeField] TextMeshProUGUI _pointsText;

    string _correctAnswer;
    public Button[] _buttons = new Button[3];
    [SerializeField] Button _backButton;

    private List<string> _answers = new List<string>();
    private HashSet<int> _askedQuestions = new HashSet<int>();

    public bool queryCalled;
    public float currentTimer = 0;
    public float startingTimer = 10;
    public int currentPoints = 0;


    private Color _originalButtonColor;

    public static UIManagment Instance { get; private set; }

    void Awake()
    {
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

    private void Start()
    {
        currentTimer = startingTimer;
        currentPoints = 0;
        queryCalled = false;
        _originalButtonColor = _buttons[0].GetComponent<Image>().color;

        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);
        clientSupabase.InitializeAsync();
    }

    void Update()
    {
        _categoryText.text = PlayerPrefs.GetString("SelectedTrivia");
        _questionText.text = GameManager.Instance.responseList[GameManager.Instance.randomQuestionIndex].QuestionText;
        GameManager.Instance.CategoryAndQuestionQuery(queryCalled);

        if (currentTimer > 0)
        {
            currentTimer -= Time.deltaTime;
        }
        else
        {
            currentTimer = 0;
        }
        _timerText.text = currentTimer.ToString("f0");
        _pointsText.text = currentPoints.ToString("f0");
    }

    public void OnButtonClick(int buttonIndex)
    {
        string selectedAnswer = _buttons[buttonIndex].GetComponentInChildren<TextMeshProUGUI>().text;
        _correctAnswer = GameManager.Instance.responseList[GameManager.Instance.randomQuestionIndex].CorrectOption;

        if (selectedAnswer == _correctAnswer)
        {
            Debug.Log("�Respuesta correcta!");
            currentPoints += Mathf.RoundToInt(currentTimer);
            ChangeButtonColor(buttonIndex, Color.green);
            Invoke("RestoreButtonColor", 2f);
            GameManager.Instance._answers.Clear();
            _askedQuestions.Add(GameManager.Instance.randomQuestionIndex);
            Invoke("NextAnswer", 2f);
            currentTimer = startingTimer;
        }
        else
        {
            Debug.Log("Respuesta incorrecta. Int�ntalo de nuevo.");
            ChangeButtonColor(buttonIndex, Color.red);
            Invoke("HandleFailure", 1f);
        }
    }

    private void ChangeButtonColor(int buttonIndex, Color color)
    {
        Image buttonImage = _buttons[buttonIndex].GetComponent<Image>();
        buttonImage.color = color;
    }

    private void RestoreButtonColor()
    {
        foreach (Button button in _buttons)
        {
            Image buttonImage = button.GetComponent<Image>();
            buttonImage.color = _originalButtonColor;
        }
    }

    private void NextAnswer()
    {
        if (AllQuestionsAnswered())
        {
            int usuarioId = SupabaseManager.CurrentUserId;
            int categoriaId = TriviaSelection.triviaSelected;
            int puntajeFinal = currentPoints;
            AttemptSaved(usuarioId, categoriaId, puntajeFinal);
            SceneManager.LoadScene("Victory");
        }
        else
        {
            queryCalled = false;
            currentTimer = startingTimer;
            do
            {
                GameManager.Instance.randomQuestionIndex = Random.Range(0, GameManager.Instance.responseList.Count);
            } while (_askedQuestions.Contains(GameManager.Instance.randomQuestionIndex));

            GameManager.Instance.CategoryAndQuestionQuery(queryCalled);
        }
    }

    private bool AllQuestionsAnswered()
    {
        return _askedQuestions.Count >= GameManager.Instance.responseList.Count;
    }

    private void HandleFailure()
    {
        int usuarioId = SupabaseManager.CurrentUserId;
        int categoriaId = TriviaSelection.triviaSelected;
        int puntajeFinal = currentPoints;
        AttemptSaved(usuarioId, categoriaId, puntajeFinal);
        LoadFailureScene();
    }

    private void LoadFailureScene()
    {
        SceneManager.LoadScene("Failure");
    }

    public async void AttemptSaved(int usuarioId, int categoriaId, int puntajeFinal)
    {
        var ultimoId = await clientSupabase
            .From<intentos>()
            .Select("id")
            .Order(intentos => intentos.id, Postgrest.Constants.Ordering.Descending) 
            .Get();

        int nuevoId = 1; 

        if (ultimoId.Models.Count > 0)
        {
            nuevoId = ultimoId.Models[0].id + 1;
        }

        var nuevoIntento = new intentos
        {
            id = nuevoId,
            usuario_id = usuarioId,
            categoria_id = categoriaId,
            puntaje = puntajeFinal
        };

        var resultado = await clientSupabase
            .From<intentos>()
            .Insert(new[] { nuevoIntento });

        if (resultado.ResponseMessage.IsSuccessStatusCode)
        {
            Debug.Log("Intento guardado correctamente en Supabase.");
        }
        else
        {
            Debug.LogError("Error al guardar el intento en Supabase: " + resultado.ResponseMessage);
        }
    }

    public void PreviousScene()
    {
        Destroy(GameManager.Instance);
        Destroy(UIManagment.Instance);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
