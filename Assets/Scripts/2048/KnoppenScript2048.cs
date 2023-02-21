using TMPro;
using UnityEngine;

public class KnoppenScript2048 : BaseUIHandler
{
    [Header("Other scene specific")]
    [SerializeField] private TMP_Dropdown sizeDropdown;

    protected override void Start()
    {
        baseLayout = GetComponent<Layout2048>();
        base.Start();
    }

    public void StartNew2048()
    {
        saveScript.IntDict["grootte2048"] = sizeDropdown.value;
        StartNewGame();
    }
}
