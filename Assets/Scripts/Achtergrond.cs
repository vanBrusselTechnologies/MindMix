using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Achtergrond : MonoBehaviour
{
    [SerializeField] private Sprite voorplaat;
    [SerializeField] private GameObject achtergrond;
    [SerializeField] private RectTransform achtergrondRect;
    [SerializeField] private Image achtergrondImg;
    private Color wisselColor = Color.white;
    private int klaar = 0;
    private bool isPaused = false;
    private bool wasPaused = false;
    private float vorigeScreenWidth;
    private float vorigeSafezoneY;
    private float vorigeSafezoneX;
    private GegevensHouder gegevensScript;
    [HideInInspector] public List<Color> kleuren = new List<Color>();
    private bool wisselendeKleur;
    [HideInInspector] public bool aangepasteKleur = false;
    private SaveScript saveScript;
    [HideInInspector] public List<TMP_Dropdown.OptionData> colorOptionData = new List<TMP_Dropdown.OptionData>();
    private List<int> beschikbareKleuren = new List<int>();
    [HideInInspector] public List<TMP_Dropdown.OptionData> gekochteColorOptionData = new List<TMP_Dropdown.OptionData>();
    [HideInInspector] public List<TMP_Dropdown.OptionData> imageOptionData = new List<TMP_Dropdown.OptionData>();
    private List<int> beschikbareAfbeeldingen = new List<int>();
    [HideInInspector] public List<TMP_Dropdown.OptionData> gekochteImageOptionData = new List<TMP_Dropdown.OptionData>();

    private void Awake()
    {
        gegevensScript = GetComponent<GegevensHouder>();
        saveScript = GetComponent<SaveScript>();
        List<System.Drawing.Color> listOfDrawingColours = new List<System.Drawing.Color>();
        bool isKleur = false;
        foreach (System.Drawing.KnownColor knownColor in System.Enum.GetValues(typeof(System.Drawing.KnownColor)))
        {
            System.Drawing.Color col = System.Drawing.Color.FromKnownColor(knownColor);
            listOfDrawingColours.Add(col);
            if (col.Name.Equals("AliceBlue"))
            {
                isKleur = true;
            }
            if (isKleur)
            {
                colorOptionData.Add(new TMP_Dropdown.OptionData(col.Name));
                Color color = new Color(col.R / 256f, col.G / 256f, col.B / 256f, col.A / 256f);
                kleuren.Add(color);
            }
            if (col.Name.Equals("YellowGreen"))
            {
                isKleur = false;
            }
        }
        foreach(Sprite afbeelding in gegevensScript.achtergronden)
        {
            imageOptionData.Add(new TMP_Dropdown.OptionData(afbeelding));
        }
        SceneManager.activeSceneChanged += OnSceneLoaded;
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(achtergrond.transform.parent.gameObject);
        DontDestroyOnLoad(Camera.main.gameObject);
        vorigeScreenWidth = Screen.width;
        vorigeSafezoneY = Screen.safeArea.y;
        vorigeSafezoneX = Screen.safeArea.x;
    }

    public void StartValues()
    {
        beschikbareKleuren.Clear();
        beschikbareAfbeeldingen.Clear();
        for (int i = -1; i < 140; i++)
        {
            if(saveScript.intDict["kleur" + i + "gekocht"] == 1)
            {
                beschikbareKleuren.Add(i);
            }
        }
        for (int i = 0; i < 50; i++)
        {
            if (saveScript.intDict["afbeelding" + i + "gekocht"] == 1)
            {
                beschikbareAfbeeldingen.Add(i);
            }
        }
        beschikbareKleuren.Sort();
        beschikbareAfbeeldingen.Sort();
        gekochteColorOptionData.Clear();
        gekochteImageOptionData.Clear();
        for (int ii = 0; ii < beschikbareKleuren.Count; ii++)
        {
            if (beschikbareKleuren[ii] == -1)
            {
                gekochteColorOptionData.Add(new TMP_Dropdown.OptionData("Changing Color"));
            }
            else
            {
                gekochteColorOptionData.Add(colorOptionData[beschikbareKleuren[ii]]);
            }
            if (ii < beschikbareAfbeeldingen.Count)
            {
                gekochteImageOptionData.Add(imageOptionData[beschikbareAfbeeldingen[ii]]);
            }
        }
        SetBackground();
    }

    private void OnSceneLoaded(Scene scene1, Scene scene2)
    {
        SetBackground();
    }

    private void FixedUpdate()
    {
        if (!isPaused && wasPaused)
        {
            SetBackground();
        }
        wasPaused = isPaused;
        if (aangepasteKleur)
        {
            aangepasteKleur = false;
            SetBackground();
        }
        if (wisselendeKleur)
        {
            SetBackground();
        }
        if (vorigeScreenWidth == Screen.width && vorigeSafezoneY == Screen.safeArea.y && vorigeSafezoneX == Screen.safeArea.x)
        {
            if (klaar < 3)
            {
                klaar += 1;
                SetBackground();
            }
            return;
        }
        klaar = 0;
        vorigeScreenWidth = Screen.width;
        vorigeSafezoneY = Screen.safeArea.y;
        vorigeSafezoneX = Screen.safeArea.x;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        isPaused = pauseStatus;
    }

    private void SetBackground()
    {
        List<int> BGList = gegevensScript.AchtergrondList();
        wisselendeKleur = false;
        if (BGList.Count < 2)
        {
            return;
        }
        int bgSoort = BGList[0];
        if (achtergrondImg)
        {
            if (bgSoort == 1)
            {
                int afbInt = BGList[1];
                achtergrondImg.color = Color.white;
                if (afbInt == -1)
                {
                    achtergrondImg.sprite = voorplaat;
                    Rect imgRect1 = achtergrondImg.sprite.rect;
                    float width1 = Screen.width;
                    float height1 = Screen.height;
                    if (imgRect1.width / width1 < imgRect1.height / height1)
                    {
                        height1 = 1000 * width1;
                    }
                    else
                    {
                        width1 = 1000 * height1;
                    }
                    achtergrondRect.sizeDelta = new Vector2(width1, height1);
                    return;
                }
                achtergrondImg.sprite = gegevensScript.achtergronden[afbInt];
                Rect imgRect = achtergrondImg.sprite.rect;
                float width = Screen.width;
                float height = Screen.height;
                if (imgRect.width / width < imgRect.height / height)
                {
                    height = 1000 * width;
                }
                else
                {
                    width = 1000 * height;
                }
                achtergrondRect.sizeDelta = new Vector2(width, height);
                return;
            }
            if (bgSoort == 0)
            {
                int kleurInt = BGList[1];
                achtergrondImg.sprite = gegevensScript.spriteWit;
                if (kleurInt == -1)
                {
                    wisselendeKleur = true;
                    WisselKleur(achtergrondImg, wisselColor);
                }
                else
                {
                    achtergrondImg.color = kleuren[kleurInt];
                }
                int width = Screen.width;
                int height = Screen.height;
                if (width > height)
                {
                    height = width;
                }
                else
                {
                    width = height;
                }
                achtergrondRect.sizeDelta = new Vector2(width, height);
                return;
            }
        }
    }

    private void WisselKleur(Image achtergrondImg, Color oldColor)
    {
        if(oldColor.Equals(Color.white))
        {
            float tmpR = saveScript.floatDict["color.r"];
            float tmpG = saveScript.floatDict["color.g"];
            float tmpB = saveScript.floatDict["color.b"];
            if (tmpR == 0 && tmpG == 0 && tmpB == 0)
            {
                oldColor = Color.red;
                oldColor.a = 1f;
            }
            else
            {
                oldColor = new Color(tmpR, tmpG, tmpB, 1f);
            }
        }
        Color nextColor = oldColor;
        if (Mathf.Approximately(nextColor.r, 1f) && !Mathf.Approximately(nextColor.g, 1f) && Mathf.Approximately(nextColor.b, 0f))
        {
            nextColor.g += 1f / 255f;
            nextColor.g = Mathf.Min(nextColor.g, 1f);
        }
        else if ((Mathf.Approximately(nextColor.r, 1f) && Mathf.Approximately(nextColor.g, 1f)) || (!Mathf.Approximately(nextColor.r, 0f) && Mathf.Approximately(nextColor.g, 1f)))
        {
            nextColor.r -= 1f / 255f;
            nextColor.r = Mathf.Max(nextColor.r, 0f);
        }
        else if (Mathf.Approximately(nextColor.r, 0f) && Mathf.Approximately(nextColor.g, 1f) && !Mathf.Approximately(nextColor.b, 1f))
        {
            nextColor.b += 1f / 255f;
            nextColor.b = Mathf.Min(nextColor.b, 1f);
        }
        else if ((Mathf.Approximately(nextColor.g, 1f) && Mathf.Approximately(nextColor.b, 1f)) || (!Mathf.Approximately(nextColor.g, 0f) && Mathf.Approximately(nextColor.b, 1f)))
        {
            nextColor.g -= 1f / 255f;
            nextColor.g = Mathf.Max(nextColor.g, 0f);
        }
        else if (Mathf.Approximately(nextColor.b, 1f) && !Mathf.Approximately(nextColor.r, 1f))
        {
            nextColor.r += 1f / 255f;
            nextColor.r = Mathf.Min(nextColor.r, 1f);
        }
        else if ((Mathf.Approximately(nextColor.b, 1f) && Mathf.Approximately(nextColor.r, 1f)) || (!Mathf.Approximately(nextColor.b, 0f) && Mathf.Approximately(nextColor.r, 1f)))
        {
            nextColor.b -= 1f / 255f;
            nextColor.b = Mathf.Max(nextColor.b, 0f);
        }
        achtergrondImg.color = nextColor;
        wisselColor = nextColor;
        saveScript.floatDict["color.r"] = nextColor.r;
        saveScript.floatDict["color.g"] = nextColor.g;
        saveScript.floatDict["color.b"] = nextColor.b;
    }

    public void KleurGekocht(int kleurIndex)
    {
        beschikbareKleuren.Add(kleurIndex);
        beschikbareKleuren.Sort();
        gekochteColorOptionData.Clear();
        for (int i = 0; i < beschikbareKleuren.Count; i++)
        {
            if (beschikbareKleuren[i] == -1)
            {
                gekochteColorOptionData.Add(new TMP_Dropdown.OptionData("Changing Color"));
            }
            else
            {
                gekochteColorOptionData.Add(colorOptionData[beschikbareKleuren[i]]);
            }
        }
    }

    public void AfbeeldingGekocht(int kleurIndex)
    {
        beschikbareAfbeeldingen.Add(kleurIndex);
        beschikbareAfbeeldingen.Sort();
        gekochteImageOptionData.Clear();
        for (int i = 0; i < beschikbareAfbeeldingen.Count; i++)
        {
            gekochteImageOptionData.Add(imageOptionData[beschikbareAfbeeldingen[i]]);
        }
    }
}
