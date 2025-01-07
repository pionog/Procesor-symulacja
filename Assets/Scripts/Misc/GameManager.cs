using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Transform content; // Przypisz Content z hierarchii w inspektorze
    public GameObject tmpTextPrefab; // Prefab TMP_Text jako GameObject (utwórz prefab i przypisz go)

    public TMP_InputField inputFieldA;
    public TMP_InputField inputFieldB;
    public TMP_InputField inputFieldC;
    public TMP_InputField inputFieldTMP1;
    public TMP_InputField inputFieldTMP2;
    public TMP_InputField inputFieldPC;
    public TMP_InputField inputFieldMAR;
    public TMP_InputField inputFieldMDR;

    public TMP_Text Score;
    private int gameScore = 0;
    public Button ClockButton;
    public Button RewindButton;
    public Toggle MultipleCyclesToggle;
    public TMP_InputField Cycles;


    private void Awake()
    {
        // Singleton pattern to ensure only one instance of GameManager exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensure this object persists across scenes
        }
    }

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        Debug.Log("Initializing game...");

        // Pobierz rejestry z RegisterManager
        PopulateRegisters();
        Score.text = gameScore.ToString();
    }

    public void StartGame()
    {
        Debug.Log("Game Started!");
        // Logic for starting the game
    }

    public void EndGame()
    {
        Debug.Log("Game Over!");
        // Logic for ending the game
    }


    private void PopulateRegisters()
    {
        if (RegisterManager.Instance == null)
        {
            Debug.LogError("RegisterManager.Instance jest null. Upewnij siê, ¿e RegisterManager dzia³a poprawnie.");
            return;
        }

        if (content == null)
        {
            Debug.LogError("Referencja 'content' w GameManager jest null.");
            return;
        }

        // Pobierz wszystkie rejestry z grupy "R"
        var generalPurposeRegisters = RegisterManager.Instance.GeneralPurposeRegisters.GetAllRegisters();

        //var singleRegisters = RegisterManager.Instance.SingleRegisters;

        foreach (var register in generalPurposeRegisters)
        {
            string registerText = $"{register.Name}: {register.Value.ToString("X8")}"; // Wyœwietlenie w formacie HEX (np. 00000000)
            AddRegisterToContent(registerText);
        }

        UpdateInputFields();
    }

    private void AddRegisterToContent(string text)
    {
        // SprawdŸ, czy prefab zosta³ przypisany
        if (tmpTextPrefab == null)
        {
            Debug.LogError("Prefab TMP_TextPrefab nie zosta³ przypisany w GameManager.");
            return;
        }

        // Utwórz nowy element tekstowy
        GameObject textObject = Instantiate(tmpTextPrefab, content);
        TMP_Text tmpText = textObject.GetComponent<TMP_Text>();

        if (tmpText == null)
        {
            Debug.LogError("Prefab TMP_TextPrefab nie zawiera komponentu TMP_Text.");
            return;
        }

        tmpText.text = text;
    }

    private void UpdateInputFields()
    {
        inputFieldA.text = RegisterManager.Instance.GetRegisterValue("A").ToString("X8");  // "X8" formatuje na 8 cyfr hex
        inputFieldB.text = RegisterManager.Instance.GetRegisterValue("B").ToString("X8");
        inputFieldC.text = RegisterManager.Instance.GetRegisterValue("C").ToString("X8");
        inputFieldTMP1.text = RegisterManager.Instance.GetRegisterValue("TMP1").ToString("X8");
        inputFieldTMP2.text = RegisterManager.Instance.GetRegisterValue("TMP2").ToString("X8");
        inputFieldPC.text = RegisterManager.Instance.GetRegisterValue("PC").ToString("X8");
        inputFieldMAR.text = RegisterManager.Instance.GetRegisterValue("MAR").ToString("X8");
        inputFieldMDR.text = RegisterManager.Instance.GetRegisterValue("MDR").ToString("X8");
    }

    public void UpdateScore(bool isForward) {
        bool cyclesOn = MultipleCyclesToggle.isOn;
        int cycles = cyclesOn ? Int32.TryParse(Cycles.text, out cycles) ? cycles : 0 : 0;
        int step = isForward ? 1 : -1;
        if (cyclesOn) AddScore(step * cycles); else AddScore(step * 1);
    }

    public void AddScore(int number) {
        if (gameScore + number < 0) {Debug.LogError("Niedozwolona operacja!"); return; }
        gameScore += number;
        Score.text = gameScore.ToString();
    }
}
