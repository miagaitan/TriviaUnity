using UnityEngine;
using Supabase; 
using Supabase.Interfaces;
using System.Threading; 
using Postgrest.Models; 
using TMPro;
using UnityEngine.UI; 
using System.Threading.Tasks; 
using UnityEngine.SceneManagement;



public class SupabaseManager : MonoBehaviour
{
    [Header("Campos de Interfaz")]
    [SerializeField] TMP_InputField _userIDInput; 
    [SerializeField] TMP_InputField _userPassInput; 
    [SerializeField] TextMeshProUGUI _stateText; 

    public static int CurrentUserId { get; private set; } 
    public static string CurrentUsername { get; private set; } 
    string supabaseUrl = "https://uzfacolewgnmhebpfulo.supabase.co";
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InV6ZmFjb2xld2dubWhlYnBmdWxvIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MTYyNTc0MDUsImV4cCI6MjAzMTgzMzQwNX0.gWx7SFGIvhve1FoG98yIV5H_9koMoLM4JQuG_ncARCA"; 

    Supabase.Client clientSupabase;
    private usuarios _usuarios = new usuarios();

    public bool canPlay = false;

    public void Start()
    {

    }

    public async void UserLogin()
    {
   
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);

        var test_response = await clientSupabase
            .From<usuarios>()
            .Select("*")
            .Get();
        Debug.Log(test_response.Content);

        var login_password = await clientSupabase
            .From<usuarios>()
            .Select("id, password")
            .Where(usuarios => usuarios.username == _userIDInput.text)
            .Get(); 

        if (login_password.Models.Count > 0)
        {
            var usuario = login_password.Models[0];
            if (usuario.password.Equals(_userPassInput.text))
            {
                print("LOGIN SUCCESSFUL");
                _stateText.text = "LOGIN SUCCESSFUL";
                _stateText.color = Color.green;
                SupabaseManager.CurrentUsername = _userIDInput.text; 
                SupabaseManager.CurrentUserId = usuario.id;
                canPlay = true;
            }
            else
            {
                print("WRONG PASSWORD");
                _stateText.text = "WRONG PASSWORD";
                _stateText.color = Color.red;
            }
        }
        else
        {
            print("USER NOT FOUND");
            _stateText.text = "USER NOT FOUND";
            _stateText.color = Color.red; 
        }
    }

    public async void InsertarNuevoUsuario()
    {
      
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);

  
        var ultimoId = await clientSupabase
            .From<usuarios>()
            .Select("id")
            .Order(usuarios => usuarios.id, Postgrest.Constants.Ordering.Descending)
            .Get();

        int nuevoId = 1;

        if (ultimoId != null)
        {
            nuevoId = ultimoId.Models[0].id + 1; 
        }

        
        var nuevoUsuario = new usuarios
        {
            id = nuevoId,
            username = _userIDInput.text,
            age = Random.Range(0, 100),
            password = _userPassInput.text,
        };

     
        var resultado = await clientSupabase
            .From<usuarios>()
            .Insert(new[] { nuevoUsuario });

      
        if (resultado.ResponseMessage.IsSuccessStatusCode)
        {
            _stateText.text = "Usuario Correctamente Ingresado";
            _stateText.color = Color.green;
            canPlay = true;
        }
        else
        {
            _stateText.text = "Error en el registro de usuario";
            _stateText.text = resultado.ResponseMessage.ToString();
            _stateText.color = Color.red;
        }
    }

    public void Play()
    {
        if (canPlay == true)
        {
            SceneManager.LoadScene("TriviaSelectScene");
        }
    }
}
