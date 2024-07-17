using UnityEngine;
using Supabase;
using System.Threading.Tasks;
using Supabase.Interfaces;
using Postgrest.Models;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class Ranking : MonoBehaviour
{
    string supabaseUrl = "https://uzfacolewgnmhebpfulo.supabase.co";
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InV6ZmFjb2xld2dubWhlYnBmdWxvIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MTYyNTc0MDUsImV4cCI6MjAzMTgzMzQwNX0.gWx7SFGIvhve1FoG98yIV5H_9koMoLM4JQuG_ncARCA";
    public Supabase.Client clientSupabase;

    List<trivia> trivias = new List<trivia>();
    List<intentos> attempts = new List<intentos>();
    List<usuarios> users = new List<usuarios>();

    [SerializeField] TMP_Dropdown _category_dropdown;

    [SerializeField] TMP_Text generalRanking;
    [SerializeField] TMP_Text categoryRanking;
    public static int SelectedTriviaId { get; private set; }
    public static Ranking Instance { get; private set; }

    public DatabaseManager databaseManager;

    async void Start()
    {
        Instance = this;
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);

        await SelectTrivias();
        PopulateDropdown();

        await LoadAttemptData();
        await LoadUser();

        _category_dropdown.onValueChanged.AddListener(OnCategoryDropdownValueChanged);

        ShowGeneral();
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

    async Task LoadAttemptData()
    {
        var response = await clientSupabase
            .From<intentos>()
            .Select("*")
            .Get();

        if (response != null)
        {
            attempts = response.Models.ToList();
        }
    }

    async Task LoadUser()
    {
        var response = await clientSupabase
            .From<usuarios>()
            .Select("*")
            .Get();

        if (response != null)
        {
            users = response.Models.ToList();
        }
    }

    void PopulateDropdown()
    {
        _category_dropdown.ClearOptions();

        List<string> categories = new List<string>();

        foreach (var trivia in trivias)
        {
            categories.Add(trivia.category);
        }

        _category_dropdown.AddOptions(categories);
    }

    void ShowGeneral()
    {
        var sortedUsers = attempts.OrderByDescending(x => x.puntaje).Take(7); ;

        string generalRankingText = "  USUARIO           PUNTAJE\n";
        foreach (var intento in sortedUsers)
        {
            var user = users.FirstOrDefault(u => u.id == intento.usuario_id);
            if (user != null)
            {

                generalRankingText += $"{user.username,-24} {intento.puntaje}\n"; 
            }
        }

        this.generalRanking.text = generalRankingText;
    }

    void ShowCategory(string category)
    {
        var selectedCategory = trivias.FirstOrDefault(t => t.category == category);

        if (selectedCategory != null)
        {
            var categoryUsers = attempts.Where(x => x.categoria_id == selectedCategory.id).OrderByDescending(x => x.puntaje).Take(7); ;

            string categoryRankingText = "  USUARIO           PUNTAJE\n";
            foreach (var intento in categoryUsers)
            {
                var user = users.FirstOrDefault(u => u.id == intento.usuario_id);
                if (user != null)
                {
                    categoryRankingText += $"{user.username,-24} {intento.puntaje}\n";
                }
            }
            this.categoryRanking.text = categoryRankingText;
        }
    }
    void OnCategoryDropdownValueChanged(int index)
    {
        string selectedCategory = _category_dropdown.options[index].text;
        ShowCategory(selectedCategory);
    }

    public void MenuButton()
    {
        SceneManager.LoadScene("TriviaSelectScene");
    }

}