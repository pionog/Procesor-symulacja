using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using Unity.VisualScripting;

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
    public GameObject summaryPanel;
    public GameObject tryAgainPanel;

    public TMP_Text Score;
    public TMP_Text Score2;
    private string nickname;
    private int gameScore = 0;
    public Button ClockButton;
    public Button RewindButton;
    public Toggle MultipleCyclesToggle;
    public TMP_InputField Cycles;
    private int resets = 0;
    public TMP_Text resetsText;
    private int definedMnemonics = 0;
    public TMP_Text definedMnemonicsText;
    private int definedInstructions = 0;
    public TMP_Text definedInstructionsText;
    private int actualMnemonics = 0;
    public TMP_Text actualMnemonicsText;
    private int actualInstructions = 0;
    public TMP_Text actualInstructionsText;

    private MicrocodeManager MicrocodeManager;
    private MemoryManager MemoryManager;
    private RegisterManager RegisterManager;
    private InstructionManager InstructionManager;
    private MicrocodeExecutor MicrocodeExecutor;
    private readonly int[] winCondition = new int[32] 
    { 
        0x1,    0x3,    0x6,    0xA,    0xF,    0x15,   0x1C,   0x24, 
        0x2D,   0x37,   0x42,   0x4E,   0x5B,   0x69,   0x78,   0x88, 
        0x98,   0xA8,   0xB8,   0xC8,   0xD8,   0xE8,   0xF8,   0x108,
        0x118,  0x128,  0x138,  0x148,  0x158,  0x168,  0x178,  0x188
    };

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

    /// <summary>
    /// Inicjalizowanie gry
    /// </summary>
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

    /// <summary>
    /// Pobieranie liczby zdefiniowanych mneomników przez całą grę
    /// </summary>
    /// <returns>
    /// <c>int</c> liczba zdefiniowanych mnemoników przez całą grę</returns>
    public int GetDefinedMnemonics() {
        return definedMnemonics;
    }
    /// <summary>
    /// Pobieranie liczby zdefiniowanych instrukcji przez całą grę
    /// </summary>
    /// <returns>
    /// <c>int</c> liczba zdefiniowanych instrukcji przez całą grę</returns>
    public int GetDefinedInstructions()
    {
        return definedInstructions;
    }
    /// <summary>
    /// Pobieranie liczby mnemoników w trakcie gry
    /// </summary>
    /// <returns>
    /// <c>int</c> liczba mnemoników w trakcie gry</returns>
    public int GetActualMnemonics() {
        return actualMnemonics;
    }
    /// <summary>
    /// Pobieranie liczby instrukcji w trakcie gry
    /// </summary>
    /// <returns>
    /// <c>int</c> liczba instrukcji w trakcie gry</returns>
    public int GetActualInstructions()
    {
        return actualInstructions;
    }
    /// <summary>
    /// Ustawianie liczby zdefiowanych mnemoników przez całą grę
    /// </summary>
    /// <param name="number">
    /// Aktualna liczba zdefiowanych mnemoników przez całą grę
    /// </param>
    public void SetDefinedMnemonics(int number) { 
        definedMnemonics = number;
    }
    /// <summary>
    /// Ustawianie liczby zdefiowanych instrukcji przez całą grę
    /// </summary>
    /// <param name="number">
    /// Aktualna liczba zdefiowanych instrukcji przez całą grę
    /// </param>
    public void SetDefinedInstructions(int number)
    {
        definedInstructions = number;
    }
    /// <summary>
    /// Ustawianie liczby mnemoników pod koniec gry
    /// </summary>
    /// <param name="number">
    /// Aktualna liczba dostępnych mnemoników w trakcie gry
    /// </param>
    public void SetActualMnemonics(int number)
    {
        actualMnemonics = number;
    }

    /// <summary>
    /// Ustawianie liczby instrukcji pod koniec gry
    /// </summary>
    /// <param name="number">
    /// Aktualna liczba dostępnych instrukcji w trakcie gry
    /// </param>
    public void SetActualInstructions(int number)
    {
        actualInstructions = number;
    }

    /// <summary>
    /// Tworzenie i wypełnianie rejestrów
    /// </summary>
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

        UpdateInputFields();
    }

    /// <summary>
    /// Aktualizowanie pól wyświetlających zawartość poszczególnych rejestrów
    /// </summary>
    public void UpdateRegisterDisplay()
    {
        var generalPurposeRegisters = this.RegisterManager.GeneralPurposeRegisters.GetAllRegisters();

        foreach (var register in generalPurposeRegisters)
        {
            // Sprawdzanie, czy dany rejestr jest wyświetlany
            if (registerTexts.TryGetValue(register.Name, out TMP_Text tmpText))
            {
                // Aktualizowanie wartości tekstu
                tmpText.text = $"{register.Name}: {register.Value.ToString("X8")}";
            }
            else
            {
                Debug.LogWarning($"Rejestr {register.Name} nie zosta� znaleziony w widoku. Mo�e trzeba go doda�.");
            }
        }
    }

    /// <summary>
    /// Dodawanie do pola jedynej instancji MemoryManager
    /// </summary>
    private void PopulateMemory() {
        this.MemoryManager = MemoryManager.Instance;
    }
    /// <summary>
    /// Dodawanie do pola jedynej instancji MicrocodeManager
    /// </summary>
    private void PopulateMicrocode() {
        this.MicrocodeManager = MicrocodeManager.Instance;
    }
    /// <summary>
    /// Dodawanie do pola jedynej instancji InstructionManager
    /// </summary>
    private void PopulateInstruction() { 
        this.InstructionManager = InstructionManager.Instance;
    }
    /// <summary>
    /// Dodawanie do pola jedynej instancji MicrocodeExecutor
    /// </summary>
    private void PopulateMicrocodeExecutor() {
        this.MicrocodeExecutor = MicrocodeExecutor.Instance;
    }

    /// <summary>
    /// Dodawanie nowego rejestru do wyświetlania
    /// </summary>
    /// <param name="registerName">
    /// Nazwa rejestru, który ma być wyświetlany
    /// </param>
    /// <param name="text">
    /// Reprezentacja tekstowa zawartości dodawanego rejestru
    /// </param>
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
            //Debug.LogWarning($"Rejestr o nazwie {registerName} ju� istnieje w widoku.");
        }
    }

    /// <summary>
    /// Aktualizowanie pól wyświetlających zawartość poszczególnych rejestrów
    /// </summary>
    private void UpdateInputFields()
    {
        // "X8" formatuje na 8 cyfr hex
        inputFieldA.text = RegisterManager.Instance.GetRegisterValue("A").ToString("X8");  
        inputFieldB.text = RegisterManager.Instance.GetRegisterValue("B").ToString("X8");
        inputFieldC.text = RegisterManager.Instance.GetRegisterValue("C").ToString("X8");
        inputFieldTMP1.text = RegisterManager.Instance.GetRegisterValue("TMP1").ToString("X8");
        inputFieldTMP2.text = RegisterManager.Instance.GetRegisterValue("TMP2").ToString("X8");
        inputFieldPC.text = RegisterManager.Instance.GetRegisterValue("PC").ToString("X8");
        inputFieldMAR.text = RegisterManager.Instance.GetRegisterValue("MAR").ToString("X8");
        inputFieldMDR.text = RegisterManager.Instance.GetRegisterValue("MDR").ToString("X8");
    }

    /// <summary>
    /// Pokazanie komunikatu o ukończeniu gry
    /// </summary>
    public void ViewSummary() {
        summaryPanel.SetActive(true);
        resetsText.text = resets.ToString();
        definedMnemonicsText.text = definedMnemonics.ToString();
        definedInstructionsText.text = definedInstructions.ToString();
        actualMnemonicsText.text = actualMnemonics.ToString();
        actualInstructionsText.text = actualInstructions.ToString();
        Score2.text = gameScore.ToString();
    }

    /// <summary>
    /// Pokazanie komunikatu o spróbowaniu zagrania w grę ponownie
    /// </summary>
    public void ViewTryAgain() {
        tryAgainPanel.SetActive(true);
    }

    /// <summary>
    /// Sprawdzanie czy gracz ukończył grę
    /// </summary>
    /// <returns></returns>
    public bool CheckWin() {
        for (int i = 0; i < 32; i++) {
            if (MemoryManager.Instance.ReadInt(0x300 + i * 4) != winCondition[i]) {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Wykonywanie akcji w grze
    /// </summary>
    /// <param name="isForward">
    /// Określenie, w którym kierunku został wykonany krok w grze
    /// </param>
    public void MakeAnAction(bool isForward)
    {
        bool cyclesOn = MultipleCyclesToggle.isOn;
        int cycles = cyclesOn ? Int32.TryParse(Cycles.text, out cycles) ? cycles : 0 : 0;
        int stepDirection = isForward ? 1 : -1;
        int steps = cyclesOn ? stepDirection * cycles : stepDirection * 1;
        int stepsMade = ExecuteMicrocode(steps);
        Debug.Log(stepsMade.ToString());
        

        UpdateScore(stepsMade);
        UpdateInputFields();
        UpdateRegisterDisplay();



        if (InstructionManager.GetNoMoreLeft())
        {
            bool isCompleted = CheckWin();
            if (isCompleted)
            {
                tryAgainPanel.SetActive(false);
                ViewSummary();
            }
            else
            {
                summaryPanel.SetActive(false);
                ViewTryAgain();
            }
        }
        else {
            tryAgainPanel.SetActive(false);
            summaryPanel.SetActive(false);
        }

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
        public int resets;
        public int definedMnemonics;
        public int definedInstructions;
        public int actualMnemonics;
        public int actualInstructions;
        public string timestamp;
        public List<List<string>> instructionList;
        public List<MicrocodeTableEntry> microcodeTables;

        public GameData() { }

        public GameData(int score, string nickname, List<string[]> instructions, Dictionary<string, MicrocodeTable> microcodeTableDict, int resets = 0, int definedMnemonics = 0, int definedInstructions = 0, int actualMnemonics = 0, int actualInstructions = 0)
        {
            this.score = score;
            this.nickname = nickname;
            this.resets = resets;
            this.actualMnemonics = actualMnemonics;
            this.actualInstructions = actualInstructions;
            this.definedInstructions = definedMnemonics;
            this.definedMnemonics = definedInstructions;
            this.timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

            this.instructionList = new List<List<string>>();

            foreach (var array in instructions)
            {
                this.instructionList.Add(new List<string>(array));
            }
            this.microcodeTables = new List<MicrocodeTableEntry>();

            foreach (var kvp in microcodeTableDict)
            {
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

    /// <summary>
    /// Zapisywanie stanu gry
    /// </summary>
    public void saveGame() {

        GameData gameData = new GameData(gameScore, nickname, InstructionManager.Instance.getInstructionList(), MicrocodeManager.Instance.getmicrocodeTables());

        string jsonData = JsonConvert.SerializeObject(gameData, Formatting.Indented);

        File.WriteAllText(Application.persistentDataPath + "/save_" + nickname 
            + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".json", jsonData);

        Debug.Log("zapisano w: " + Application.persistentDataPath + "/save_" + nickname 
            + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".json");
    }

    /// <summary>
    /// Zapisywanie wyniku gry
    /// </summary>
    public void saveScore() {
        ScoreData scoreData = new ScoreData(gameScore, nickname);
        string jsonData = JsonUtility.ToJson(scoreData, true);

        File.WriteAllText(Application.persistentDataPath + "/score_" + nickname 
            + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".json", jsonData);

        Debug.Log("zapisano w: " + Application.persistentDataPath + "/score_" + nickname 
            + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".json");
    }


    /// <summary>
    /// Wczytywanie zapisu gry
    /// </summary>
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
            resets = gameData.resets;
            actualMnemonics = gameData.actualMnemonics;
            actualInstructions = gameData.actualInstructions;
            definedInstructions = gameData.definedInstructions;
            definedMnemonics = gameData.definedMnemonics;

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

    /// <summary>
    /// Ustawianie pseudonimu gracza na podstawie tekstu wpisanego w odpowiednim polu tekstowym
    /// </summary>
    public void setNickname() {
        nickname = inputFieldNickname.text;
    }

    /// <summary>
    /// Aktualizowanie wyniku gry
    /// </summary>
    /// <param name="number">
    /// Liczba kroków o ile ma zostać zmieniony wynik
    /// </param>
    public void UpdateScore(int number) {
        if (gameScore + number < 0) {Debug.LogError("Niedozwolona operacja!"); return; }
        gameScore += number;
        Score.text = gameScore.ToString();
    }

    /// <summary>
    /// Restartowanie gry
    /// </summary>
    public void RestartGame() {
        MemoryManager.Instance.ResetMemory();
        RegisterManager.Instance.InitializeRegisters();
        RegisterManager.SetRegisterValue("A", 0);
        RegisterManager.SetRegisterValue("B", 0);
        RegisterManager.SetRegisterValue("C", 0);
        RegisterManager.SetRegisterValue("TMP1", 0);
        RegisterManager.SetRegisterValue("TMP2", 0);
        RegisterManager.SetRegisterValue("uAR", 0);
        RegisterManager.SetRegisterValue("PC", 0);
        RegisterManager.SetRegisterValue("IR", 0);
        RegisterManager.SetRegisterValue("MAR", 0);
        RegisterManager.SetRegisterValue("MDR", 0);
        RegisterManager.GeneralPurposeRegisters = new RegisterGroup("R", 32);
        MicrocodeExecutor.Instance.SetCurrentInstruction(0);
        MicrocodeExecutor.Instance.SetStartBool(true);
        gameScore = 0;
        Score.text = gameScore.ToString();
        inputFieldIR.text = "";
        UpdateInputFields();
        UpdateRegisterDisplay();
        Debug.Log("Resetowanie gry");
        resets++;
        tryAgainPanel.SetActive(false);
        summaryPanel.SetActive(false);
    }


    /// <summary>
    /// Wykonywanie mikrokodu
    /// </summary>
    /// <param name="steps">
    /// Liczba kroków, ile powinno zostać wykonanych przez program
    /// </param>
    /// <returns>
    /// <c>Liczba kroków, ile zostało wykonanych przez program</c>
    /// </returns>
    public int ExecuteMicrocode(int steps) {
        int currentMicrocodeRow = CurrentMicrocodeRow;
        int currentInstruction = MicrocodeExecutor.Instance.GetCurrentInstruction() / 4;
        inputFieldIR.text = InstructionManager.Instance.GetInstruction(currentInstruction)[0] + " " + InstructionManager.Instance.GetInstruction(currentInstruction)[1];

        int currentStep = 0;
        string[] instructionArray;
        string mnemonic;
        int lastStepIndex;
        bool changedState = false;
        while (currentStep < steps)
        {
            instructionArray = InstructionManager.Instance.GetInstruction(currentInstruction);
            mnemonic = instructionArray[0];
            string[] args = TextParser.SplitText(instructionArray[1]);
            
            
            int[] argsType = TextParser.AnalyzeWords(args);
            MicrocodeTable currentTable;
            bool isStart = MicrocodeExecutor.Instance.GetStartBool();
            
            if (isStart)
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
                string insStr = " ";
                foreach (var arg in args) { insStr += arg + " "; };
                Debug.Log(mnemonic + insStr);
                string regCon = "";
                int index = 0;
                foreach (var arg in argsType) {
                    if (arg == 0)
                    {
                        regCon += args[index] + ": ";
                        regCon += RegisterManager.Instance.GetRegisterValue(args[index]) + "\n";
                    }
                    index++;
                }
                MicrocodeExecutor.Instance.Execute(currentMicrocodeRow, args, argsType);
                currentMicrocodeRow++;
                CurrentMicrocodeRow++;
                currentStep++;
                if (changedState) {
                    // at this moment if it is a cyclic execution first row of START mnemonic must be done
                    currentInstruction = MicrocodeExecutor.Instance.GetCurrentInstruction() / 4;
                    instructionArray = InstructionManager.Instance.GetInstruction(currentInstruction);
                    if (instructionArray == null)
                    {
                        return currentStep;
                    }
                    mnemonic = instructionArray[0];
                    args = TextParser.SplitText(instructionArray[1]);
                    argsType = TextParser.AnalyzeWords(args);
                    changedState = false;
                }
                if (!isStart && MicrocodeExecutor.Instance.GetStartBool()){
                    Debug.Log("Przechodze do mnemonika START");
                    changedState = true;
                    break;
                }
                if (currentStep == steps) {
                    UpdateInputFields();
                    inputFieldIR.text = InstructionManager.Instance.GetInstruction(currentInstruction)[0] + " " + InstructionManager.Instance.GetInstruction(currentInstruction)[1];
                    break; 
                }
            }
            if ((currentMicrocodeRow == lastStepIndex)||changedState)
            {
                currentMicrocodeRow = 0;
                CurrentMicrocodeRow = 0;
                currentInstruction = MicrocodeExecutor.Instance.GetCurrentInstruction() / 4;
            }
        }
        return currentStep;
    }
}
