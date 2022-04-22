using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class KnoppenScript : MonoBehaviour
{
    public List<GameObject> knoppen;
    private PlaatsGetallen plaatsScript;
    private AfScript afScript;
    private int gekozendifficulty;
    public int knopIndex = -1;
    public bool isButtonSelected = false;
    private List<int> buttonsGehad = new List<int>();
    //private bool eersteKeer = false;
    [SerializeField] private GameObject showMenuKnop;
    [SerializeField] private GameObject SudokuMenu;
    [SerializeField] private RectTransform SudokuMenuRect;
    [SerializeField] private RectTransform showMenuKnopRect;
    [SerializeField] private RectTransform menuNieuweKnopRect;
    [SerializeField] private RectTransform menuDifficultyRect;
    [SerializeField] private RectTransform terugNaarMenuKnopRect;
    [SerializeField] private GameObject achtergrondNormaalOfNotitieKnop;
    [SerializeField] private GameObject uitlegUI;
    [SerializeField] private RectTransform uitlegTitelRect;
    [SerializeField] private RectTransform uitlegTekstRect;
    [SerializeField] private RectTransform uitlegSluitKnopRect;
    [SerializeField] private GameObject sudoku;
    [SerializeField] private GameObject overigCanvas;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private GameObject instellingenObj;
    [SerializeField] private RectTransform instellingenSluitKnopRect;
    [SerializeField] private RectTransform instellingenScrolldown;
    [SerializeField] private RectTransform instellingenScrolldownContent;
    private GegevensHouder gegevensScript;
    private SaveScript saveScript;
    [SerializeField] private MeshRenderer normaalKnopMesh;
    [SerializeField] private MeshRenderer notitieKnopMesh;
    private bool naamIsEventSystem = true;
    Color dubbelGetalKleurRood = new Color(1f, 0, 0, 1f);
    Color normaalVakjesKleur = new Color(175f / 255f, 175f / 255f, 175f / 255f, 60f / 255f);

    // Use this for initialization
    public void Startt()
    {
        GameObject gegevensHouder = GameObject.Find("gegevensHouder");
        if (gegevensHouder == null)
        {
            return;
        }
        gegevensScript = gegevensHouder.GetComponent<GegevensHouder>();
        saveScript = gegevensHouder.GetComponent<SaveScript>();
        plaatsScript = GetComponent<PlaatsGetallen>();
        afScript = GetComponent<AfScript>();
        buttonsGehad.Clear();
        for (int i = 1; i < 82; i++)
        {
            saveScript.intDict["Button " + i] = 0;
        }
        saveScript.intDict["DeI"] = 0;
        if (!gameObject.name.Equals("EventSystem"))
        {
            naamIsEventSystem = false;
            return;
        }
        SudokuMenu.SetActive(false);
        if (saveScript.intDict["dubbelGetalWarningIsOn"] == 1)
        {
            CheckVoorDubbelGetal();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!naamIsEventSystem)
        {
            return;
        }
        if (afScript.GehaaldCanvas.activeInHierarchy)
        {
            return;
        }
        if (gegevensScript == null)
        {
            return;
        }
        int DeI = saveScript.intDict["DeI"];
        if (knoppen[DeI] == gameObject)
        {
            if (buttonsGehad.IndexOf(DeI) == -1)
            {
                buttonsGehad.Add(DeI);
                saveScript.stringDict["Button " + DeI + " notitie1"] = "  ";
                saveScript.stringDict["Button " + DeI + " notitie2"] = "  ";
                saveScript.stringDict["Button " + DeI + " notitie3"] = "  ";
                saveScript.stringDict["Button " + DeI + " notitie4"] = "  ";
                saveScript.stringDict["Button " + DeI + " notitie5"] = "  ";
                saveScript.stringDict["Button " + DeI + " notitie6"] = "  ";
                saveScript.stringDict["Button " + DeI + " notitie7"] = "  ";
                saveScript.stringDict["Button " + DeI + " notitie8"] = "  ";
                saveScript.stringDict["Button " + DeI + " notitie9"] = "  ";
            }
        }
    }

    public void nieuweSudoku()
    {
        gekozendifficulty = dropdown.value;
        saveScript.intDict["difficulty"] = gekozendifficulty;
        gegevensScript.startNewSudoku = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void nogEenSudoku()
    {
        gegevensScript.startNewSudoku = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void moeilijkerSudoku()
    {
        int difficulty = saveScript.intDict["difficulty"];
        saveScript.intDict["difficulty"] = difficulty + 1;
        gegevensScript.startNewSudoku = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void isSelected()
    {
        GameObject selectedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        for (int i = 0; i < 81; i++)
        {
            if (knoppen[i] == selectedButton)
            {
                saveScript.intDict["Button " + i] = 1;
                saveScript.intDict["DeI"] = i;
            }
            else
            {
                saveScript.intDict["Button " + i] = 0;
            }
        }
    }

    public void terugNaarMenu()
    {
        SceneManager.LoadScene("SpellenOverzicht");
    }

    public void getalGeklikt()
    {
        string selectedNumber = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;
        int dei = saveScript.intDict["DeI"];
        if (saveScript.intDict["Button " + dei] == 1)
        {
            if (plaatsScript.NormaalGetal)
            {
                RectTransform rect = knoppen[dei].transform.GetChild(0).GetComponent<RectTransform>();
                rect.offsetMax = new Vector2(0, 0);
                rect.offsetMin = new Vector2(0, 0);
                TMP_Text text = knoppen[dei].transform.GetChild(0).GetComponent<TMP_Text>();
                text.alignment = TextAlignmentOptions.Center;
                text.fontSize = 285;
                text.text = selectedNumber;
                saveScript.intDict["DoorSpelerIngevuldBij" + dei] = int.Parse(selectedNumber);
                afScript.getallen[dei] = int.Parse(selectedNumber);
                if (saveScript.intDict["notitieBijwerkSettingIsOn"] == 1)
                {
                    WerkNotitiesBij(dei);
                }
            }
            else if (!plaatsScript.NormaalGetal)
            {
                RectTransform rect = knoppen[dei].transform.GetChild(0).GetComponent<RectTransform>();
                rect.offsetMax = new Vector2(0, 0);
                rect.offsetMin = new Vector2(-33f, 0);
                TMP_Text text = knoppen[dei].transform.GetChild(0).GetComponent<TMP_Text>();
                text.alignment = TextAlignmentOptions.Left;
                text.fontSize = 80;
                string een = saveScript.stringDict["Button " + dei + " notitie1"];
                string twee = saveScript.stringDict["Button " + dei + " notitie2"];
                string drie = saveScript.stringDict["Button " + dei + " notitie3"];
                string vier = saveScript.stringDict["Button " + dei + " notitie4"];
                string vijf = saveScript.stringDict["Button " + dei + " notitie5"];
                string zes = saveScript.stringDict["Button " + dei + " notitie6"];
                string zeven = saveScript.stringDict["Button " + dei + " notitie7"];
                string acht = saveScript.stringDict["Button " + dei + " notitie8"];
                string negen = saveScript.stringDict["Button " + dei + " notitie9"];
                switch (selectedNumber)
                {
                    case "1": een = (een == "  ") ? "1" : "  "; break;
                    case "2": twee = (twee == "  ") ? "2" : "  "; break;
                    case "3": drie = (drie == "  ") ? "3" : "  "; break;
                    case "4": vier = (vier == "  ") ? "4" : "  "; break;
                    case "5": vijf = (vijf == "  ") ? "5" : "  "; break;
                    case "6": zes = (zes == "  ") ? "6" : "  "; break;
                    case "7": zeven = (zeven == "  ") ? "7" : "  "; break;
                    case "8": acht = (acht == "  ") ? "8" : "  "; break;
                    case "9": negen = (negen == "  ") ? "9" : "  "; break;
                }
                saveScript.stringDict["Button " + dei + " notitie1"] = een;
                saveScript.stringDict["Button " + dei + " notitie2"] = twee;
                saveScript.stringDict["Button " + dei + " notitie3"] = drie;
                saveScript.stringDict["Button " + dei + " notitie4"] = vier;
                saveScript.stringDict["Button " + dei + " notitie5"] = vijf;
                saveScript.stringDict["Button " + dei + " notitie6"] = zes;
                saveScript.stringDict["Button " + dei + " notitie7"] = zeven;
                saveScript.stringDict["Button " + dei + " notitie8"] = acht;
                saveScript.stringDict["Button " + dei + " notitie9"] = negen;
                string a = een + "  " + twee + "  " + drie + "\n" +
                           vier + "  " + vijf + "  " + zes + "\n" +
                           zeven + "  " + acht + "  " + negen;
                text.text = a;
                saveScript.intDict["DoorSpelerIngevuldBij" + dei] = 0;
                afScript.getallen[dei] = 0;
            }
            if (saveScript.intDict["dubbelGetalWarningIsOn"] == 1)
            {
                CheckVoorDubbelGetal();
            }
        }
        afScript.ietsVeranderd = true;
    }

    public void NormaalOfNotitie()
    {
        achtergrondNormaalOfNotitieKnop.transform.Rotate(new Vector3(0, 180, 180));
        plaatsScript.NormaalGetal = !plaatsScript.NormaalGetal;
        if (!plaatsScript.NormaalGetal)
        {
            notitieKnopMesh.enabled = true;
            normaalKnopMesh.enabled = false;
        }
        else
        {
            notitieKnopMesh.enabled = false;
            normaalKnopMesh.enabled = true;
        }
    }

    public void OpenMenu()
    {
        bool verticaal = Screen.safeArea.width < Screen.safeArea.height;
        bool openen;
        if (showMenuKnop.transform.localEulerAngles == new Vector3(0, 0, 0) || showMenuKnop.transform.localEulerAngles == new Vector3(0, 0, 270))
        {
            showMenuKnop.transform.localEulerAngles += new Vector3(0, 0, 180);
            openen = false;
        }
        else
        {
            showMenuKnop.transform.localEulerAngles -= new Vector3(0, 0, 180);
            openen = true;
        }
        SudokuMenu.SetActive(true);
        if (openen)
        {
            if (verticaal)
            {
                SudokuMenuRect.sizeDelta = new Vector2(Screen.width, Screen.safeArea.y + Screen.safeArea.height * 0.15f);
                SudokuMenuRect.anchoredPosition = new Vector2(0, -SudokuMenuRect.sizeDelta.y / 2f + Screen.safeArea.y);
                float schaal = Mathf.Min(Mathf.Min(Screen.safeArea.height, Screen.safeArea.width) / 1080f * 1.1f, Mathf.Max(Screen.safeArea.height, Screen.safeArea.width) / 2520f * 1.1f);
                menuNieuweKnopRect.anchoredPosition = new Vector2(SudokuMenuRect.sizeDelta.x / 4f, Screen.safeArea.y / 2f);
                menuNieuweKnopRect.localScale = new Vector3(schaal, schaal, 1);
                menuDifficultyRect.anchoredPosition = new Vector2(-SudokuMenuRect.sizeDelta.x / 4f, Screen.safeArea.y / 2f);
                menuDifficultyRect.localScale = new Vector3(schaal, schaal, 1);
            }
            else
            {
                SudokuMenuRect.sizeDelta = new Vector2(Screen.safeArea.x + Screen.safeArea.width * 0.2f, Screen.height);
                SudokuMenuRect.anchoredPosition = new Vector2(Screen.safeArea.x - SudokuMenuRect.sizeDelta.x / 2f - (Screen.width / 2), Screen.height / 2f);
                float schaal = Mathf.Min(Mathf.Min(Screen.safeArea.height, Screen.safeArea.width) / 1080f * 1f, Mathf.Max(Screen.safeArea.height, Screen.safeArea.width) / 2520f * 1f);
                terugNaarMenuKnopRect.transform.SetParent(SudokuMenu.transform);
                terugNaarMenuKnopRect.sizeDelta = new Vector2(Screen.safeArea.height / 11, Screen.safeArea.height / 11);
                terugNaarMenuKnopRect.anchoredPosition = new Vector2(Screen.safeArea.x - SudokuMenuRect.sizeDelta.x / 2f + terugNaarMenuKnopRect.sizeDelta.x / 2f, -Screen.safeArea.y / 2f + Screen.height / 2f - terugNaarMenuKnopRect.sizeDelta.y / 2f);
                menuNieuweKnopRect.anchoredPosition = new Vector2(Screen.safeArea.x / 2f, -Screen.safeArea.height / 8f);
                menuNieuweKnopRect.localScale = new Vector3(schaal, schaal, 1);
                menuDifficultyRect.anchoredPosition = new Vector2(Screen.safeArea.x / 2f, Screen.safeArea.height / 8f);
                menuDifficultyRect.localScale = new Vector3(schaal, schaal, 1);
            }
            dropdown.value = saveScript.intDict["difficulty"];
        }
        StartCoroutine(LaatMenuZien(openen, verticaal));
    }

    private IEnumerator LaatMenuZien(bool welLatenZien, bool verticaal)
    {
        float speed = 50f;
        SudokuMenu.SetActive(true);
        if (verticaal)
        {
            if (welLatenZien)
            {
                showMenuKnop.transform.Translate(Vector3.up * speed);
                SudokuMenu.transform.Translate(Vector3.up * speed);
                if (SudokuMenuRect.anchoredPosition.y > SudokuMenuRect.sizeDelta.y / 2f)
                {
                    showMenuKnopRect.anchoredPosition = new Vector2(0, SudokuMenuRect.sizeDelta.y + showMenuKnopRect.sizeDelta.y / 2f);
                    SudokuMenuRect.anchoredPosition = new Vector2(0, SudokuMenuRect.sizeDelta.y / 2f);
                    StopAllCoroutines();
                }
            }
            else
            {
                showMenuKnop.transform.Translate(1.5f * speed * Vector3.up);
                SudokuMenu.transform.Translate(1.5f * speed * Vector3.down);
                if (SudokuMenuRect.anchoredPosition.y < -SudokuMenuRect.sizeDelta.y / 2f + Screen.safeArea.y)
                {
                    showMenuKnopRect.anchoredPosition = new Vector2(0, Screen.safeArea.y + showMenuKnopRect.sizeDelta.y / 2f);
                    SudokuMenu.SetActive(false);
                    StopAllCoroutines();
                }
            }
        }
        else
        {
            if (welLatenZien)
            {
                showMenuKnop.transform.Translate(Vector3.up * speed);
                SudokuMenu.transform.Translate(Vector3.right * speed);
                if (SudokuMenuRect.anchoredPosition.x > SudokuMenuRect.sizeDelta.x / 2f - (Screen.width / 2))
                {
                    showMenuKnopRect.anchoredPosition = new Vector2(SudokuMenuRect.sizeDelta.x - (Screen.width / 2f) + showMenuKnopRect.sizeDelta.y / 2f, Screen.height / 2f);
                    SudokuMenuRect.anchoredPosition = new Vector2((SudokuMenuRect.sizeDelta.x / 2f) - (Screen.width / 2f), Screen.height / 2f);
                    StopAllCoroutines();
                }
            }
            else
            {
                showMenuKnop.transform.Translate(1.5f * speed * Vector3.up);
                SudokuMenu.transform.Translate(1.5f * speed * Vector3.left);
                if (SudokuMenuRect.anchoredPosition.x < -SudokuMenuRect.sizeDelta.x / 2f - (Screen.width / 2) + Screen.safeArea.x)
                {
                    showMenuKnopRect.anchoredPosition = new Vector2(Screen.safeArea.x - (Screen.width / 2f) + showMenuKnopRect.sizeDelta.y / 2f, Screen.height / 2f);
                    SudokuMenu.SetActive(false);
                    StopAllCoroutines();
                }
            }
        }
        yield return gegevensScript.wachtHonderdste;
        StartCoroutine(LaatMenuZien(welLatenZien, verticaal));
    }

    public void OpenUitleg()
    {
        if (uitlegUI.activeSelf)
        {
            uitlegUI.SetActive(false);
            sudoku.SetActive(true);
            overigCanvas.SetActive(true);
        }
        else
        {
            uitlegUI.SetActive(true);
            uitlegTitelRect.anchoredPosition = new Vector2(0, Screen.safeArea.height * (1f / 3f));
            uitlegTitelRect.sizeDelta = new Vector2(Screen.safeArea.width * 0.75f, Screen.safeArea.height);
            uitlegTekstRect.anchoredPosition = new Vector2(0, -Screen.safeArea.height / 8f);
            uitlegTekstRect.sizeDelta = new Vector2(Screen.safeArea.width * 0.85f, Screen.safeArea.height / 2);
            uitlegSluitKnopRect.anchoredPosition = new Vector2(-(Screen.width - Screen.safeArea.width - Screen.safeArea.x) - (Mathf.Min(Screen.safeArea.height, Screen.safeArea.width) * 0.1f * 0.6f), -(Screen.height - Screen.safeArea.height - Screen.safeArea.y) - (Mathf.Min(Screen.safeArea.height, Screen.safeArea.width) * 0.1f * 0.6f));
            uitlegSluitKnopRect.localScale = new Vector2(Mathf.Min(Screen.safeArea.height, Screen.safeArea.width) * 0.1f, Mathf.Min(Screen.safeArea.height, Screen.safeArea.width) * 0.1f) / 108f;
            sudoku.SetActive(false);
            overigCanvas.SetActive(false);
            float kleinsteKant = Mathf.Min(Screen.safeArea.height, Screen.safeArea.width);
            float grootsteKant = Mathf.Max(Screen.safeArea.height, Screen.safeArea.width);
            if (kleinsteKant - 1440 > 0)
            {
                float factor = Mathf.Min(kleinsteKant / 1500f, grootsteKant / 2500f);
                uitlegTitelRect.localScale = Vector2.one * factor;
                uitlegTitelRect.sizeDelta /= factor;
            }
        }
    }

    public void OpenSudokuSettings()
    {
        if (instellingenObj.activeSelf)
        {
            instellingenObj.SetActive(false);
            sudoku.SetActive(true);
            overigCanvas.SetActive(true);
            if (saveScript.intDict["dubbelGetalWarningIsOn"] == 1)
            {
                CheckVoorDubbelGetal();
            }
            else
            {
                for (int dei = 0; dei < 81; dei++)
                {
                    Button knop = knoppen[dei].GetComponent<Button>();
                    ColorBlock colorBlock = knop.colors;
                    colorBlock.normalColor = normaalVakjesKleur;
                    knop.colors = colorBlock;
                }
            }
        }
        else
        {
            float safeZoneAntiY = (Screen.safeArea.y - (Screen.height - Screen.safeArea.height - Screen.safeArea.y)) / 2f;
            float safeZoneAntiX = (Screen.safeArea.x - (Screen.width - Screen.safeArea.width - Screen.safeArea.x)) / 2f;
            instellingenObj.SetActive(true);
            instellingenSluitKnopRect.anchoredPosition = new Vector2(-(Screen.width - Screen.safeArea.width - Screen.safeArea.x) - (Mathf.Min(Screen.safeArea.height, Screen.safeArea.width) * 0.1f * 0.6f), -(Screen.height - Screen.safeArea.height - Screen.safeArea.y) - (Mathf.Min(Screen.safeArea.height, Screen.safeArea.width) * 0.1f * 0.6f));
            instellingenSluitKnopRect.localScale = new Vector2(Mathf.Min(Screen.safeArea.height, Screen.safeArea.width) * 0.1f, Mathf.Min(Screen.safeArea.height, Screen.safeArea.width) * 0.1f) / 108f;
            sudoku.SetActive(false);
            overigCanvas.SetActive(false);
            Vector3 scrollDownScale = new Vector3(Screen.safeArea.width * 0.98f / 2250f, Screen.safeArea.height * 0.85f / 950f, 1);
            instellingenScrolldown.localScale = scrollDownScale;
            float minScaleDeel = Mathf.Min(scrollDownScale.x, scrollDownScale.y);
            Vector3 scrollDownContentScale = new Vector3(minScaleDeel / scrollDownScale.x, minScaleDeel / scrollDownScale.y, 1);
            instellingenScrolldownContent.localScale = scrollDownContentScale;
            Vector3 scrollDownPosition = new Vector3(safeZoneAntiX, safeZoneAntiY + (Screen.safeArea.height * 0.15f / -2f), 0);
            instellingenScrolldown.anchoredPosition = scrollDownPosition;
        }
    }

    private List<int> KrijgVakEnRijEnKolomVanDeI(int dei)
    {
        List<int> vakRijEnKolom = new List<int>();
        int vak = dei / 9;
        int rij = vak / 3 * 3 + dei % 9 / 3 + 1;
        int kolom = vak % 3 * 3 + dei % 9 % 3 + 1;
        vakRijEnKolom.Add(vak + 1);
        vakRijEnKolom.Add(rij);
        vakRijEnKolom.Add(kolom);
        return vakRijEnKolom;
    }

    private void WerkNotitiesBij(int dei)
    {
        List<int> vakRijEnKolom = KrijgVakEnRijEnKolomVanDeI(dei);
        int vak = vakRijEnKolom[0];
        int rij = vakRijEnKolom[1];
        int kolom = vakRijEnKolom[2];
        List<int> kolomGetallen = plaatsScript.KrijgGetallenTot81(kolom, 0, 1, 1);
        List<int> rijGetallen = plaatsScript.KrijgGetallenTot81(0, rij, 1, 1);
        List<int> vakGetallen = new List<int>();
        for (int i = 0; i < 9; i++)
        {
            vakGetallen.Add((vak - 1) * 9 + i);
        }
        foreach (int getal in kolomGetallen)
        {
            if (getal != dei)
            {
                if (afScript.getallen[getal] == 0)
                {
                    if (saveScript.stringDict["Button " + getal + " notitie" + afScript.getallen[dei]] != "  ")
                    {
                        saveScript.stringDict["Button " + getal + " notitie" + afScript.getallen[dei]] = "  ";
                        string een = saveScript.stringDict["Button " + getal + " notitie1"];
                        string twee = saveScript.stringDict["Button " + getal + " notitie2"];
                        string drie = saveScript.stringDict["Button " + getal + " notitie3"];
                        string vier = saveScript.stringDict["Button " + getal + " notitie4"];
                        string vijf = saveScript.stringDict["Button " + getal + " notitie5"];
                        string zes = saveScript.stringDict["Button " + getal + " notitie6"];
                        string zeven = saveScript.stringDict["Button " + getal + " notitie7"];
                        string acht = saveScript.stringDict["Button " + getal + " notitie8"];
                        string negen = saveScript.stringDict["Button " + getal + " notitie9"];
                        string a = een + "  " + twee + "  " + drie + "\n" +
                                   vier + "  " + vijf + "  " + zes + "\n" +
                                   zeven + "  " + acht + "  " + negen;
                        TMP_Text text = knoppen[getal].transform.GetChild(0).GetComponent<TMP_Text>();
                        text.text = a;
                    }
                }
            }
        }
        foreach (int getal in rijGetallen)
        {
            if (getal != dei)
            {
                if (afScript.getallen[getal] == 0)
                {
                    if (saveScript.stringDict["Button " + getal + " notitie" + afScript.getallen[dei]] != "  ")
                    {
                        saveScript.stringDict["Button " + getal + " notitie" + afScript.getallen[dei]] = "  ";
                        string een = saveScript.stringDict["Button " + getal + " notitie1"];
                        string twee = saveScript.stringDict["Button " + getal + " notitie2"];
                        string drie = saveScript.stringDict["Button " + getal + " notitie3"];
                        string vier = saveScript.stringDict["Button " + getal + " notitie4"];
                        string vijf = saveScript.stringDict["Button " + getal + " notitie5"];
                        string zes = saveScript.stringDict["Button " + getal + " notitie6"];
                        string zeven = saveScript.stringDict["Button " + getal + " notitie7"];
                        string acht = saveScript.stringDict["Button " + getal + " notitie8"];
                        string negen = saveScript.stringDict["Button " + getal + " notitie9"];
                        string a = een + "  " + twee + "  " + drie + "\n" +
                                   vier + "  " + vijf + "  " + zes + "\n" +
                                   zeven + "  " + acht + "  " + negen;
                        TMP_Text text = knoppen[getal].transform.GetChild(0).GetComponent<TMP_Text>();
                        text.text = a;
                    }
                }
            }
        }
        foreach (int getal in vakGetallen)
        {
            if (getal != dei)
            {
                if (afScript.getallen[getal] == 0)
                {
                    if (saveScript.stringDict["Button " + getal + " notitie" + afScript.getallen[dei]] != "  ")
                    {
                        saveScript.stringDict["Button " + getal + " notitie" + afScript.getallen[dei]] = "  ";
                        string een = saveScript.stringDict["Button " + getal + " notitie1"];
                        string twee = saveScript.stringDict["Button " + getal + " notitie2"];
                        string drie = saveScript.stringDict["Button " + getal + " notitie3"];
                        string vier = saveScript.stringDict["Button " + getal + " notitie4"];
                        string vijf = saveScript.stringDict["Button " + getal + " notitie5"];
                        string zes = saveScript.stringDict["Button " + getal + " notitie6"];
                        string zeven = saveScript.stringDict["Button " + getal + " notitie7"];
                        string acht = saveScript.stringDict["Button " + getal + " notitie8"];
                        string negen = saveScript.stringDict["Button " + getal + " notitie9"];
                        string a = een + "  " + twee + "  " + drie + "\n" +
                                   vier + "  " + vijf + "  " + zes + "\n" +
                                   zeven + "  " + acht + "  " + negen;
                        TMP_Text text = knoppen[getal].transform.GetChild(0).GetComponent<TMP_Text>();
                        text.text = a;
                    }
                }
            }
        }
    }

    public void CheckVoorDubbelGetal(int dei = 0)
    {
        if (dei == 81) return;
        List<int> vakRijEnKolom = KrijgVakEnRijEnKolomVanDeI(dei);
        int vak = vakRijEnKolom[0];
        int rij = vakRijEnKolom[1];
        int kolom = vakRijEnKolom[2];
        List<int> kolomGetallen = plaatsScript.KrijgGetallenTot81(kolom, 0, 1, 1);
        List<int> rijGetallen = plaatsScript.KrijgGetallenTot81(0, rij, 1, 1);
        List<int> vakGetallen = new List<int>();
        for (int i = 0; i < 9; i++)
        {
            vakGetallen.Add((vak - 1) * 9 + i);
        }
        Button knop = knoppen[dei].GetComponent<Button>();
        ColorBlock colorBlock = knop.colors;
        foreach (int getal2 in kolomGetallen)
        {
            if (dei == getal2) {; }
            else if (afScript.getallen[dei] == 0)
            {
                for (int i = 1; i <= 9; i++)
                {
                    string _ = saveScript.stringDict["Button " + dei + " notitie" + i];
                    if (!_.Equals("  "))
                    {
                        int __ = int.Parse(_);
                        if (__ == afScript.getallen[getal2])
                        {
                            colorBlock.normalColor = dubbelGetalKleurRood;
                            knop.colors = colorBlock;
                            CheckVoorDubbelGetal(dei + 1);
                            return;
                        }
                    }
                }
            }
            else if (afScript.getallen[dei] == afScript.getallen[getal2])
            {
                colorBlock.normalColor = dubbelGetalKleurRood;
                knop.colors = colorBlock;
                CheckVoorDubbelGetal(dei + 1);
                return;
            }
        }
        foreach (int getal2 in rijGetallen)
        {
            if (dei == getal2) {; }
            else if (afScript.getallen[dei] == 0)
            {
                for (int i = 1; i <= 9; i++)
                {
                    string _ = saveScript.stringDict["Button " + dei + " notitie" + i];
                    if (!_.Equals("  "))
                    {
                        int __ = int.Parse(_);
                        if (__ == afScript.getallen[getal2])
                        {
                            colorBlock.normalColor = dubbelGetalKleurRood;
                            knop.colors = colorBlock;
                            CheckVoorDubbelGetal(dei + 1);
                            return;
                        }
                    }
                }
            }
            else if (afScript.getallen[dei] == afScript.getallen[getal2])
            {
                colorBlock.normalColor = dubbelGetalKleurRood;
                knop.colors = colorBlock;
                CheckVoorDubbelGetal(dei + 1);
                return;
            }
        }
        foreach (int getal2 in vakGetallen)
        {
            if (dei == getal2) {; }
            else if (afScript.getallen[dei] == 0)
            {
                for (int i = 1; i <= 9; i++)
                {
                    string _ = saveScript.stringDict["Button " + dei + " notitie" + i];
                    if (!_.Equals("  "))
                    {
                        int __ = int.Parse(_);
                        if (__ == afScript.getallen[getal2])
                        {
                            colorBlock.normalColor = dubbelGetalKleurRood;
                            knop.colors = colorBlock;
                            CheckVoorDubbelGetal(dei + 1);
                            return;
                        }
                    }
                }
            }
            else if (afScript.getallen[dei] == afScript.getallen[getal2])
            {
                colorBlock.normalColor = dubbelGetalKleurRood;
                knop.colors = colorBlock;
                CheckVoorDubbelGetal(dei + 1);
                return;
            }
        }
        colorBlock.normalColor = normaalVakjesKleur;
        knop.colors = colorBlock;
        CheckVoorDubbelGetal(dei + 1);
    }
}