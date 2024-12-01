using UnityEngine;
using Supabase;
using Supabase.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using Postgrest.Models;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TriviaSelection : MonoBehaviour
{
    string supabaseUrl = "https://vdmvxiswfvbmrcadujzt.supabase.co";
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InZkbXZ4aXN3ZnZibXJjYWR1anp0Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzIxNDYzNjksImV4cCI6MjA0NzcyMjM2OX0.EJJpHJsRidOCyICa7fN7tGdHx7o6BkxLYc6VxgGztfI"; //COMPLETAR

    Supabase.Client clientSupabase;
    public static int triviaSelected { get; private set; }
    public static TriviaSelection Instance { get; private set; }
    List<trivia> trivias = new List<trivia>();
    [SerializeField] TMP_Dropdown _dropdown;

    public DatabaseManager databaseManager;

    async void Start()
    {
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);

        await SelectTrivias();
        PopulateDropdown();
    }

    async Task SelectTrivias()
    {
        var response = await clientSupabase
            .From<trivia>()
            .Select("*")
            .Get();

        if (response != null)
        {
            trivias = response.Models;
        }

    }

    void PopulateDropdown()
    {
        
        _dropdown.ClearOptions();

        List<string> categories = new List<string>();

        foreach (var trivia in trivias)
        {
            categories.Add(trivia.category);
        }

        _dropdown.AddOptions(categories);
    }

    public void OnStartButtonClicked()
    {
        int selectedIndex = _dropdown.value;
        string selectedTrivia = _dropdown.options[selectedIndex].text;
        triviaSelected = trivias[selectedIndex].id;

        PlayerPrefs.SetInt("SelectedIndex", selectedIndex+1);
        PlayerPrefs.SetString("SelectedTrivia", selectedTrivia);


        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void RankingButton()
    {
        SceneManager.LoadScene("Ranking");
    }

}
