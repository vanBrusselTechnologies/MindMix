using UnityEngine;
using TMPro;

public class KnoppenScriptMijnenVeger : BaseUIHandler
{
    [Header("Other scene specific")]
    [SerializeField] private GameObject achtergrondVlagOfSchepKnop;
    [SerializeField] private TMP_Dropdown difficultyDropdown;

    private MijnenVegerScript mvScript;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        if (saveScript == null) return;
        baseLayout = GetComponent<MijnenVegerLayout>();
        mvScript = GetComponent<MijnenVegerScript>();
        difficultyDropdown.value = saveScript.intDict["difficultyMijnenVeger"];
    }

    public void VlagOfSchep()
    {
        achtergrondVlagOfSchepKnop.transform.Rotate(new Vector3(0, 180, 180));
        mvScript.vlagNietSchep = !mvScript.vlagNietSchep;
    }

    public void NieuweMijnenveger(bool moreDifficult)
    {
        int chosenDiff = difficultyDropdown.value;
        if (moreDifficult) chosenDiff += 1;
        saveScript.intDict["difficultyMijnenVeger"] = chosenDiff;
        StartNewGame();
    }
}
