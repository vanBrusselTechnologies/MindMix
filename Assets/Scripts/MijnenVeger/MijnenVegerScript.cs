using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using VBG.Extensions;

public class MijnenVegerScript : MonoBehaviour
{
    [HideInInspector] public float mvSpeelveldLinks;
    [HideInInspector] public float mvSpeelveldRechts;
    [HideInInspector] public float mvSpeelveldBoven;
    [HideInInspector] public float mvSpeelveldOnder;
    [HideInInspector] public float mvKnopLinks;
    [HideInInspector] public float mvKnopRechts;
    [HideInInspector] public float mvKnopBoven;
    [HideInInspector] public float mvKnopOnder;
    private bool alreadyPressed;
    private List<int> vakjesGehad = new List<int>();
    [HideInInspector] public List<Button> buttons = new List<Button>();
    private int kolom;
    private int rij;
    public Button knop;
    public Sprite vlag;
    public Sprite bom;
    [HideInInspector] public bool vlagNietSchep = false;
    private List<int> bommenList = new List<int>();
    private List<int> randVakjes = new List<int>();
    private int aantalBommen;
    public TMP_Text bommenTeGaan;
    [HideInInspector] public bool GameOver = false;
    private bool gameIsFinished = false;
    [SerializeField] private GameObject speelVeld;
    [SerializeField] private GameObject uitlegUI;
    [SerializeField] private GameObject settingsUIObj;
    [SerializeField] private GameObject gehaaldCanvas;
    [SerializeField] private GameObject overigCanvas;
    [SerializeField] private GameObject MijnenvegerObj;
    [SerializeField] private GameObject MenuUICanvasObj;
    [SerializeField] private TMP_Text beloningText;
    private SaveScript saveScript;
    private GegevensHouder gegevensHouder;
    private BeloningScript beloningScript;
    private MijnenVegerLayout mvLayout;

    private void Start()
    {
        gegevensHouder = GegevensHouder.Instance;
        if (gegevensHouder == null)
        {
            SceneManager.LoadScene("LogoEnAppOpstart");
            return;
        }
        saveScript = SaveScript.Instance;
        beloningScript = BeloningScript.Instance;
        mvLayout = GetComponent<MijnenVegerLayout>();
        int shortSide = Mathf.Min(mvLayout.horizontalSideBoxCount, mvLayout.verticalSideBoxCount);
        int longSide = Mathf.Max(mvLayout.horizontalSideBoxCount, mvLayout.verticalSideBoxCount);
        if (gegevensHouder.startNewGame)
        {
            DeleteProgress();
            for (int i = 0; i < longSide; i++)
            {
                for (int j = 0; j < shortSide; j++)
                {
                    randVakjes.Add(i * 100 + j);
                }
            }
        }
        else
        {
            for (int i = 0; i <= longSide * shortSide; i++)
            {
                int _ = saveScript.intDict["mijnenvegerVakjesgehad" + i];
                //Debug.Log(_);
                if (_ == -1) vakjesGehad.Add(0);
                else if (_ != 0) vakjesGehad.Add(_);
            }
        }
        saveScript.intDict["begonnenAanMV"] = 1;
        Physics.autoSimulation = false;
        Physics.Simulate(1000000f);
        GameOver = false;
        aantalBommen = 25 + (int)(10f * Mathf.Pow(1.75f, saveScript.intDict["difficultyMijnenVeger"]));
        for (int i = 0; i < aantalBommen; i++)
        {
            if (gegevensHouder.startNewGame)
            {
                int getal = randVakjes[Random.Range(0, randVakjes.Count)];
                randVakjes.Remove(getal);
                rij = Mathf.FloorToInt(getal / 100f);
                kolom = getal % 100;
                saveScript.intDict["mijnenvegerBom" + i] = getal;
                bommenList.Add(getal);
            }
            else
            {
                bommenList.Add(saveScript.intDict["mijnenvegerBom" + i]);
            }
        }
        if (!gegevensHouder.startNewGame)
        {
            for (int i = 0; i < vakjesGehad.Count; i++)
            {
                if (saveScript.intDict["mijnenvegerVlaggenGezet" + vakjesGehad[i]] != 1)
                    InstantiateButton(i);
                else
                    InstantiateFlag(i);
            }
            mvLayout.SetButtonsLayout();
        }
        bommenTeGaan.text = aantalBommen.ToString();
    }

    private void Update()
    {
        if (gameIsFinished) return;
        if (vakjesGehad.Count + int.Parse(bommenTeGaan.text) >= mvLayout.horizontalSideBoxCount * mvLayout.verticalSideBoxCount && !GameOver)
        {
            Scene scene = SceneManager.GetActiveScene();
            int diff = saveScript.intDict["difficultyMijnenVeger"];
            saveScript.intDict["MijnenvegerDiff" + diff + "Gespeeld"] += 1;
            saveScript.intDict["MijnenvegersGespeeld"] += 1;
            beloningText.text = beloningScript.Beloning(scene: scene, difficulty: diff, doelwitText: beloningText).ToString();
            gameIsFinished = true;
            OpenGehaaldCanvas();
            DeleteProgress();
        }
        else if (GameOver)
        {
            gameIsFinished = true;
            DeleteProgress();
            StartCoroutine(WachtVoorGehaaldCanvas());
        }
        else
        {
            if (uitlegUI.activeSelf) return;
            if (settingsUIObj.activeSelf) return;
            StartCoroutine(OnTouch());
        }
    }

    private IEnumerator WachtVoorGehaaldCanvas()
    {
        yield return new WaitForSecondsRealtime(1f);
        OpenGehaaldCanvas();
    }

    private List<int> KrijgVakBijPos(Vector3 muisPos)
    {
        float x = muisPos.x - mvSpeelveldLinks;
        float y = muisPos.y - mvSpeelveldOnder;
        if (mvLayout.verticalSideBoxCount > mvLayout.horizontalSideBoxCount)
        {
            kolom = Mathf.FloorToInt(x / (mvSpeelveldRechts - mvSpeelveldLinks) * mvLayout.horizontalSideBoxCount); //0 is linker rij
            rij = Mathf.FloorToInt(y / (mvSpeelveldBoven - mvSpeelveldOnder) * mvLayout.verticalSideBoxCount); //0 is onderste rij
        }
        else
        {
            kolom = Mathf.FloorToInt(y / (mvSpeelveldBoven - mvSpeelveldOnder) * mvLayout.verticalSideBoxCount); //0 is linker rij
            rij = mvLayout.horizontalSideBoxCount - 1 - Mathf.FloorToInt(x / (mvSpeelveldRechts - mvSpeelveldLinks) * mvLayout.horizontalSideBoxCount); //0 is onderste rij
        }
        int buttonNumber = (rij * 100) + kolom;
        if (vakjesGehad.IndexOf(buttonNumber) != -1)
            alreadyPressed = true;
        else
        {
            vakjesGehad.Add(buttonNumber);
            if (buttonNumber == 0) buttonNumber = -1;
            saveScript.intDict["mijnenvegerVakjesgehad" + (vakjesGehad.Count - 1)] = buttonNumber;
            alreadyPressed = false;
        }
        List<int> outputlijst = new List<int> { kolom, rij };
        return outputlijst;
    }

    private int GetMineCount(int kolom, int rij)
    {
        int shortSide = Mathf.Min(mvLayout.horizontalSideBoxCount, mvLayout.verticalSideBoxCount);
        int longSide = Mathf.Max(mvLayout.horizontalSideBoxCount, mvLayout.verticalSideBoxCount);
        int aantalBommen = 0;
        if (bommenList.IndexOf(kolom + (rij * 100)) != -1)
        {
            aantalBommen = 99;
        }
        else
        {
            if (kolom == 0)
            {
                if (rij == 0)
                {
                    if (bommenList.IndexOf(kolom + 1 + (rij * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + 1 + ((rij + 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + ((rij + 1) * 100)) != -1)
                        aantalBommen += 1;
                }
                else if (rij == longSide)
                {
                    if (bommenList.IndexOf(kolom + 1 + (rij * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + 1 + ((rij - 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + ((rij - 1) * 100)) != -1)
                        aantalBommen += 1;
                }
                else
                {
                    if (bommenList.IndexOf(kolom + 1 + ((rij + 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + ((rij + 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + 1 + (rij * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + 1 + ((rij - 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + ((rij - 1) * 100)) != -1)
                        aantalBommen += 1;
                }
            }
            else if (kolom == shortSide)
            {
                if (rij == 0)
                {
                    if (bommenList.IndexOf(kolom - 1 + (rij * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom - 1 + ((rij + 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + ((rij + 1) * 100)) != -1)
                        aantalBommen += 1;
                }
                else if (rij == longSide)
                {
                    if (bommenList.IndexOf(kolom - 1 + (rij * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom - 1 + ((rij - 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + ((rij - 1) * 100)) != -1)
                        aantalBommen += 1;
                }
                else
                {
                    if (bommenList.IndexOf(kolom - 1 + ((rij + 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + ((rij + 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom - 1 + (rij * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom - 1 + ((rij - 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + ((rij - 1) * 100)) != -1)
                        aantalBommen += 1;
                }
            }
            else
            {
                if (rij == 0)
                {
                    if (bommenList.IndexOf(kolom - 1 + (rij * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom - 1 + ((rij + 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + 1 + (rij * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + 1 + ((rij + 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + ((rij + 1) * 100)) != -1)
                        aantalBommen += 1;
                }
                else if (rij == longSide)
                {
                    if (bommenList.IndexOf(kolom - 1 + (rij * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom - 1 + ((rij - 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + 1 + (rij * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + 1 + ((rij - 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + ((rij - 1) * 100)) != -1)
                        aantalBommen += 1;
                }
                else
                {
                    if (bommenList.IndexOf(kolom - 1 + (rij * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom - 1 + ((rij - 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + 1 + (rij * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + 1 + ((rij - 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + ((rij - 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom - 1 + ((rij + 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + 1 + ((rij + 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + ((rij + 1) * 100)) != -1)
                        aantalBommen += 1;
                }
            }
        }
        return aantalBommen;
    }

    private void KrijgVakBijKolomRij(int kolom, int rij)
    {
        int vakjesGehadNummer = (rij * 100) + kolom;
        if (vakjesGehadNummer < 0 || vakjesGehad.IndexOf(vakjesGehadNummer) != -1)
        {
            alreadyPressed = true;
        }
        else
        {
            vakjesGehad.Add(vakjesGehadNummer);
            if (vakjesGehadNummer == 0) vakjesGehadNummer = -1;
            saveScript.intDict["mijnenvegerVakjesgehad" + (vakjesGehad.Count - 1)] = vakjesGehadNummer;
            alreadyPressed = false;
        }
    }

    private void LegeVakken(int kolom, int rij)
    {
        KrijgVakBijKolomRij(kolom, rij);
        if (!alreadyPressed)
            InstantiateButton(kolom: kolom, rij: rij);
    }

    private void VindMeerVakkenZonderBomBuren(int kolom, int rij)
    {
        int shortSide = Mathf.Min(mvLayout.horizontalSideBoxCount, mvLayout.verticalSideBoxCount) - 1;
        int longSide = Mathf.Max(mvLayout.horizontalSideBoxCount, mvLayout.verticalSideBoxCount) - 1;
        if (kolom == 0)
        {
            if (rij == 0)
            {
                LegeVakken(kolom, rij + 1);
                LegeVakken(kolom + 1, rij);
                LegeVakken(kolom + 1, rij + 1);
            }
            else if (rij == longSide)
            {
                LegeVakken(kolom, rij - 1);
                LegeVakken(kolom + 1, rij);
                LegeVakken(kolom + 1, rij - 1);
            }
            else
            {
                LegeVakken(kolom, rij + 1);
                LegeVakken(kolom + 1, rij);
                LegeVakken(kolom + 1, rij + 1);
                LegeVakken(kolom, rij - 1);
                LegeVakken(kolom + 1, rij - 1);
            }
        }
        else if (kolom == shortSide)
        {
            if (rij == 0)
            {
                LegeVakken(kolom, rij + 1);
                LegeVakken(kolom - 1, rij);
                LegeVakken(kolom - 1, rij + 1);
            }
            else if (rij == longSide)
            {
                LegeVakken(kolom, rij - 1);
                LegeVakken(kolom - 1, rij);
                LegeVakken(kolom - 1, rij - 1);
            }
            else
            {
                LegeVakken(kolom, rij + 1);
                LegeVakken(kolom - 1, rij);
                LegeVakken(kolom - 1, rij + 1);
                LegeVakken(kolom, rij - 1);
                LegeVakken(kolom - 1, rij - 1);
            }
        }
        else
        {
            if (rij == 0)
            {
                LegeVakken(kolom, rij + 1);
                LegeVakken(kolom - 1, rij);
                LegeVakken(kolom - 1, rij + 1);
                LegeVakken(kolom + 1, rij);
                LegeVakken(kolom + 1, rij + 1);
            }
            else if (rij == longSide)
            {
                LegeVakken(kolom, rij - 1);
                LegeVakken(kolom - 1, rij);
                LegeVakken(kolom - 1, rij - 1);
                LegeVakken(kolom + 1, rij);
                LegeVakken(kolom + 1, rij - 1);
            }
            else
            {
                LegeVakken(kolom, rij + 1);
                LegeVakken(kolom, rij - 1);
                LegeVakken(kolom - 1, rij);
                LegeVakken(kolom - 1, rij + 1);
                LegeVakken(kolom - 1, rij - 1);
                LegeVakken(kolom + 1, rij);
                LegeVakken(kolom + 1, rij + 1);
                LegeVakken(kolom + 1, rij - 1);
            }
        }
    }

    private void ShowAllBombs()
    {
        for (int i = 0; i < bommenList.Count; i++)
        {
            int bomNum = bommenList[i];
            kolom = bomNum % 100;
            rij = (bomNum - kolom) / 100;
            KrijgVakBijKolomRij(kolom, rij);
            if (!alreadyPressed)
            {
                Button a = Instantiate(knop, Vector3.zero, Quaternion.identity, speelVeld.transform);
                a.name = bomNum.ToString();
                buttons.Add(a);
                a.GetComponentInChildren<TMP_Text>().text = "";
                a.GetComponent<Image>().sprite = bom;
            }
        }
    }

    public void OpenGehaaldCanvas()
    {
        gehaaldCanvas.SetActive(true);
        speelVeld.SetActive(false);
        uitlegUI.SetActive(false);
        overigCanvas.SetActive(false);
        MijnenvegerObj.SetActive(false);
        MenuUICanvasObj.SetActive(false);
        mvLayout.SetLayout();
    }

    private bool IsInGrid(Vector3 mousePosition)
    {
        return mousePosition.x > mvSpeelveldLinks && mousePosition.x < mvSpeelveldRechts && mousePosition.y > mvSpeelveldOnder && mousePosition.y < mvSpeelveldBoven;
    }

    private void DeleteProgress()
    {
        saveScript.intDict["begonnenAanMV"] = 0;
        for (int i = 0; i < 100; i++)
        {
            saveScript.intDict["mijnenvegerBom" + i] = 0;
        }
        for (int i = 0; i < mvLayout.verticalSideBoxCount * mvLayout.horizontalSideBoxCount; i++)
        {
            saveScript.intDict["mijnenvegerVakjesgehad" + i] = 0;
        }
        for (int i = 0; i < 22; i++)
        {
            for (int ii = 0; ii < 22; ii++)
            {
                int index = (i * 100) + ii;
                saveScript.intDict["mijnenvegerVlaggenGezet" + index] = 0;
            }
        }
    }

    private void InstantiateButton(int index = -100, int kolom = 0, int rij = 0)
    {
        int buttonNumber;
        if (index == -100)
        {
            buttonNumber = kolom + rij * 100;
        }
        else
        {
            buttonNumber = vakjesGehad[index];
            rij = Mathf.FloorToInt(buttonNumber / 100f);
            kolom = buttonNumber % 100;
        }
        buttonNumber = buttonNumber == 0 ? -1 : buttonNumber;
        Button a = Instantiate(knop, Vector3.zero, Quaternion.identity, speelVeld.transform);
        a.name = buttonNumber.ToString();
        buttons.Add(a);
        float mines = GetMineCount(kolom, rij);
        if (mines == 0)
        {
            a.GetComponentInChildren<TMP_Text>().text = "-";
            a.tag = "Untagged";
            if (index == -100)
            {
                VindMeerVakkenZonderBomBuren(kolom, rij);
            }
        }
        else if (mines == 99)
        {
            a.GetComponentInChildren<TMP_Text>().text = "";
            a.GetComponent<Image>().sprite = bom;
            ShowAllBombs();
            GameOver = true;
            return;
        }
        else
        {
            a.GetComponentInChildren<TMP_Text>().text = mines.ToString();
            a.tag = "Untagged";
        }
    }

    private void InstantiateFlag(int index = -100)
    {
        int buttonNumber = index == -100 ? ((rij * 100) + kolom) : vakjesGehad[index];
        if (buttonNumber == 0) buttonNumber = -1;
        Button a = Instantiate(knop, Vector3.zero, Quaternion.identity, speelVeld.transform);
        a.name = buttonNumber.ToString();
        buttons.Add(a);
        a.GetComponentInChildren<TMP_Text>().text = "";
        a.GetComponent<Image>().sprite = vlag;
        a.tag = "vlag";
        aantalBommen -= 1;
        if (index == -100)
        {
            bommenTeGaan.text = aantalBommen.ToString();
            saveScript.intDict["mijnenvegerVlaggenGezet" + buttonNumber] = 1;
        }
    }

    private void RemoveFlag()
    {
        int buttonNumber = (rij * 100) + kolom;
        if (buttonNumber == 0) buttonNumber = -1;
        int index = vakjesGehad.IndexOf(buttonNumber);
        if (index == -1)
        {
            vakjesGehad.Add(buttonNumber);
            return;
        }
        for (int i = 0; i < buttons.Count; i++)
        {
            if (int.Parse(buttons[i].name) == buttonNumber)
            {
                if (buttons[i].CompareTag("vlag"))
                {
                    Button a = buttons[i];
                    buttons.RemoveAt(i);
                    Destroy(a.gameObject);
                    aantalBommen += 1;
                    bommenTeGaan.text = aantalBommen.ToString();
                    saveScript.intDict["mijnenvegerVlaggenGezet" + buttonNumber] = 0;
                    for (int ii = index; ii < vakjesGehad.Count; ii++)
                    {
                        saveScript.intDict["mijnenvegerVakjesgehad" + ii] = saveScript.intDict["mijnenvegerVakjesgehad" + (ii + 1)];
                    }
                    saveScript.intDict["mijnenvegerVakjesgehad" + vakjesGehad.Count] = 0;
                    vakjesGehad.Remove(buttonNumber);
                }
                break;
            }
        }
    }

    private IEnumerator OnTouch()
    {
        yield return new WaitForEndOfFrame();
        GameObject selectedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if (selectedButton != null)
        {
            if (int.TryParse(selectedButton.name, out int _))
            {
                if (!vlagNietSchep) yield break;
            }
            else yield break;
        }
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase != TouchPhase.Ended) yield break;
            Vector3 muisPos = Camera.main.ScreenToWorldPoint(touch.position);
            if (IsInGrid(muisPos))
            {
                List<int> kolomRijLijst = KrijgVakBijPos(muisPos);
                kolom = kolomRijLijst[0];
                rij = kolomRijLijst[1];
                if (vlagNietSchep)
                {
                    if (alreadyPressed) RemoveFlag();
                    else InstantiateFlag();
                    mvLayout.SetButtonsLayout();
                }
                else
                {
                    if (!alreadyPressed)
                    {
                        InstantiateButton(kolom: kolom, rij: rij);
                        mvLayout.SetButtonsLayout();
                    }
                }
            }
        }
    }
}
