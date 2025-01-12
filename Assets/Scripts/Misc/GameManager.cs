using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Transform content; // Przypisz Content z hierarchii w inspektorze
    public GameObject tmpTextPrefab; // Prefab TMP_Text jako GameObject (utw�rz prefab i przypisz go)

    public TMP_InputField inputFieldA;
    public TMP_InputField inputFieldB;
    public TMP_InputField inputFieldC;
    public TMP_InputField inputFieldTMP1;
    public TMP_InputField inputFieldTMP2;
    public TMP_InputField inputFieldPC;
    public TMP_InputField inputFieldMAR;
    public TMP_InputField inputFieldMDR;
    public TMP_InputField inputFieldIR;
    public TMP_InputField inputFieldNickname;
    
    private Dictionary<string, TMP_Text> registerTexts = new Dictionary<string, TMP_Text>();

    public GameObject welcomePopup;
    public GameObject backgroundFadeout;
    public GameObject settingsButton;
    public GameObject tabs;
    public GameObject label;
    public GameObject architecturePanel;
    public GameObject scoreText;

    public TMP_Text Score;
    private string nickname;
    private int gameScore = 0;
    public Button ClockButton;
    public Button RewindButton;
    public Toggle MultipleCyclesToggle;
    public TMP_InputField Cycles;

    private MicrocodeManager MicrocodeManager;
    private MemoryManager MemoryManager;
    private RegisterManager RegisterManager;
    private InstructionManager InstructionManager;
    private MicrocodeExecutor MicrocodeExecutor;

    private int CurrentMicrocodeRow = 0;

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
            InitializeGame();
        }
    }

    private void InitializeGame()
    {
        Debug.Log("Initializing game...");

        if(LoadManager.Instance.gameFromSave) { //odkomentuj jak komunikacja działa
            LoadFromSave(); //wylacz popupy, ustaw dane w skryptach, daj na gre od razu
        }

        // Pobierz rejestry z RegisterManager
        PopulateRegisters();
        PopulateMemory();
        PopulateMicrocode();
        PopulateInstruction();
        PopulateMicrocodeExecutor();

        MicrocodeExecutor.Instance.SetMemoryManager(MemoryManager);
        MicrocodeExecutor.Instance.SetRegisterManager(RegisterManager);
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
            Debug.LogError("RegisterManager.Instance jest null. Upewnij si�, �e RegisterManager dzia�a poprawnie.");
            return;
        }

        if (content == null)
        {
            Debug.LogError("Referencja 'content' w GameManager jest null.");
            return;
        }
        this.RegisterManager = RegisterManager.Instance;
        // Pobierz wszystkie rejestry z grupy "R"
        var generalPurposeRegisters = this.RegisterManager.GeneralPurposeRegisters.GetAllRegisters();

        foreach (var register in generalPurposeRegisters)
        {
            string registerText = $"{register.Name}: {register.Value.ToString("X8")}"; // Wy�wietlenie w formacie HEX (np. 00000000)
            AddRegisterToContent(register.Name, registerText);
        }
        RegisterManager.SetRegisterValue("MAR", 16);
        RegisterManager.SetRegisterValue("MDR", 305419896);

        UpdateInputFields();
    }

    public void UpdateRegisterDisplay()
    {
        // Pobierz aktualne rejestry
        var generalPurposeRegisters = this.RegisterManager.GeneralPurposeRegisters.GetAllRegisters();

        foreach (var register in generalPurposeRegisters)
        {
            // Sprawd�, czy dany rejestr jest wy�wietlany
            if (registerTexts.TryGetValue(register.Name, out TMP_Text tmpText))
            {
                // Aktualizuj warto�� tekstu
                tmpText.text = $"{register.Name}: {register.Value.ToString("X8")}";
            }
            else
            {
                Debug.LogWarning($"Rejestr {register.Name} nie zosta� znaleziony w widoku. Mo�e trzeba go doda�.");
            }
        }
    }

    private void PopulateMemory() {
        this.MemoryManager = MemoryManager.Instance;
    }
    private void PopulateMicrocode() {
        this.MicrocodeManager = MicrocodeManager.Instance;
    }
    private void PopulateInstruction() { 
        this.InstructionManager = InstructionManager.Instance;
    }
    private void PopulateMicrocodeExecutor() {
        this.MicrocodeExecutor = MicrocodeExecutor.Instance;
    }

    private void AddRegisterToContent(string registerName, string text)
    {
        // Sprawd�, czy prefab zosta� przypisany
        if (tmpTextPrefab == null)
        {
            Debug.LogError("Prefab TMP_TextPrefab nie zosta� przypisany w GameManager.");
            return;
        }

        // Utw�rz nowy element tekstowy
        GameObject textObject = Instantiate(tmpTextPrefab, content);
        TMP_Text tmpText = textObject.GetComponent<TMP_Text>();

        if (tmpText == null)
        {
            Debug.LogError("Prefab TMP_TextPrefab nie zawiera komponentu TMP_Text.");
            return;
        }

        tmpText.text = text;

        // Dodaj do s�ownika
        if (!registerTexts.ContainsKey(registerName))
        {
            registerTexts.Add(registerName, tmpText);
        }
        else
        {
            Debug.LogWarning($"Rejestr o nazwie {registerName} ju� istnieje w widoku.");
        }
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

        //Debug.Log("Zaktualizowano rejestry");
    }

    public void MakeAnAction(bool isForward)
    {
        bool cyclesOn = MultipleCyclesToggle.isOn;
        int cycles = cyclesOn ? Int32.TryParse(Cycles.text, out cycles) ? cycles : 0 : 0;
        int stepDirection = isForward ? 1 : -1;
        int steps = cyclesOn ? stepDirection * cycles : stepDirection * 1;
        int stepsMade = ExecuteMicrocode(steps);

        UpdateScore(stepsMade);
        UpdateInputFields();
        UpdateRegisterDisplay();
    }

    [System.Serializable]
    public class MicrocodeTableEntry
    {
        [SerializeField] public string Key { get; set; }
        [SerializeField] public List<MicrocodeRow> Rows { get; set; }
        [SerializeField] public int MicrocodeType { get; set; } 
        [SerializeField] public int RegistersNumber { get; set; }
        [SerializeField] public bool Removable { get; set; }
        [SerializeField] public bool Editable { get; set; }

        public MicrocodeTableEntry() { }

        public MicrocodeTableEntry(string key, MicrocodeTable microcodeTable)
        {
            Key = key;
            Rows = microcodeTable.GetAllRowsList();
            MicrocodeType = microcodeTable.GetMicrocodeType();
            RegistersNumber = microcodeTable.GetRegistersNumber();
            Removable = microcodeTable.GetRemovable();
            Editable = microcodeTable.GetEditable();
        }
    }

    [System.Serializable]
    public class GameData
    {
        public int score;
        public string nickname;
        public string timestamp;
        public List<List<string>> instructionList;
        public List<MicrocodeTableEntry> microcodeTables;

        public GameData() { }

        public GameData(int score, string nickname, List<string[]> instructions, Dictionary<string, MicrocodeTable> microcodeTableDict)
        {
            this.score = score;
            this.nickname = nickname;
            this.timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

            this.instructionList = new List<List<string>>();

            foreach (var array in instructions){
                this.instructionList.Add(new List<string>(array));
            }
            this.microcodeTables = new List<MicrocodeTableEntry>();

            foreach (var kvp in microcodeTableDict){
                var entry = new MicrocodeTableEntry(kvp.Key, kvp.Value);
                this.microcodeTables.Add(entry);
            }
        }
    }

    [System.Serializable]
    public class ScoreData {
        public int score;
        public string nickname;
        public string timestamp;

        public ScoreData(int score, string nickname){
            this.score = score;
            this.nickname = nickname;
            this.timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        }
    }

    public void saveGame() {

        GameData gameData = new GameData(gameScore, nickname, InstructionManager.Instance.getInstructionList(), MicrocodeManager.Instance.getmicrocodeTables());

        string jsonData = JsonConvert.SerializeObject(gameData, Formatting.Indented);

        File.WriteAllText(Application.persistentDataPath + "/save_" + nickname 
            + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".json", jsonData);

        Debug.Log("zapisano w: " + Application.persistentDataPath + "/save_" + nickname 
            + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".json");
    }

    public void saveScore() {
        ScoreData scoreData = new ScoreData(gameScore, nickname);
        string jsonData = JsonUtility.ToJson(scoreData, true);

        File.WriteAllText(Application.persistentDataPath + "/score_" + nickname 
            + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".json", jsonData);

        Debug.Log("zapisano w: " + Application.persistentDataPath + "/score_" + nickname 
            + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".json");
    }

    public void LoadFromSave() {
        Debug.Log("Wczytuje zapis.");
        welcomePopup.SetActive(false);
        backgroundFadeout.SetActive(false);
        settingsButton.SetActive(true);
        tabs.SetActive(true);
        label.SetActive(true);
        architecturePanel.SetActive(true);
        scoreText.SetActive(true);

        string filePath = Path.Combine(Application.persistentDataPath, $"save_{LoadManager.Instance.nick}_{LoadManager.Instance.date}.json");

        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);

            GameData gameData = JsonConvert.DeserializeObject<GameData>(jsonData);

            nickname = gameData.nickname;
            gameScore = gameData.score;

            List<List<string>> instructionList = gameData.instructionList;
            List<MicrocodeTableEntry> microcodeTables = gameData.microcodeTables;

            //new data to set
            List<string[]> instructions = new List<string[]>();
            Dictionary<string, MicrocodeTable> microcodeTableDict = new Dictionary<string, MicrocodeTable>();

            foreach (var list in instructionList)
            {
                instructions.Add(list.ToArray());
            }

            foreach (var entry in microcodeTables)
            {
                MicrocodeTable table = new MicrocodeTable();
                

                table.SetMicrocodeType(entry.MicrocodeType);
                table.SetRegistersNumber(entry.RegistersNumber);
                table.SetRemovable(entry.Removable);
                table.SetEditable(entry.Editable);
                
                foreach (var row in entry.Rows) {
                    table.AddRow(row);
                }


                microcodeTableDict[entry.Key] = table;
            }

            if (MicrocodeManager.Instance == null) {
                MicrocodeManager.Instance = gameObject.AddComponent<MicrocodeManager>();
            }
            if (InstructionManager.Instance == null)
            {
                InstructionManager.Instance = gameObject.AddComponent<InstructionManager>();
            }

            MicrocodeManager.Instance.setMicrocodeTables(microcodeTableDict);
            InstructionManager.Instance.setInstructions(instructions);
        }
        else
        {
            Debug.LogWarning($"Save file not found: {filePath}");
        }
    }

    public void setNickname() {
        nickname = inputFieldNickname.text;
    }

    public void UpdateScore(int number) {
        if (gameScore + number < 0) {Debug.LogError("Niedozwolona operacja!"); return; }
        gameScore += number;
        Score.text = gameScore.ToString();
    }

    public int ExecuteMicrocode(int steps) {
        int currentMicrocodeRow = CurrentMicrocodeRow;
        int currentInstruction = MicrocodeExecutor.Instance.GetCurrentInstruction() / 4;
        Debug.Log(currentInstruction.ToString());

        int currentStep = 0;
        string[] instructionArray;
        string mnemonic;
        int lastStepIndex;
        while (currentStep < steps)
        {
            instructionArray = InstructionManager.Instance.GetInstruction(currentInstruction);
            mnemonic = instructionArray[0];
            string[] args = TextParser.SplitText(instructionArray[1]);
            int[] argsType = TextParser.AnalyzeWords(args);
            MicrocodeTable currentTable;
            if (MicrocodeExecutor.Instance.GetStartBool())
            {
                currentTable = MicrocodeManager.Instance.GetMicrocodeTable("START");
            }
            else {
                currentTable = MicrocodeManager.Instance.GetMicrocodeTable(mnemonic);
            }
            lastStepIndex = currentTable.Count();
            while (currentMicrocodeRow < lastStepIndex)
            {
                MicrocodeExecutor.Instance.SetMicrocodeTable(currentTable);
                MicrocodeExecutor.Instance.Execute(currentMicrocodeRow, args, argsType);
                currentMicrocodeRow++;
                CurrentMicrocodeRow++;
                currentStep++;
                if (currentStep == steps) break;
            }
            if (currentMicrocodeRow == lastStepIndex)
            {
                CurrentMicrocodeRow = 0;
            }
        }
        return currentStep;
    }
}
