using UnityEngine;

public class OpenAppScript : MonoBehaviour
{
    private StartupAnimation _startupAnimation;
    private SaveScript _saveScript;

    private void Start()
    {
        _startupAnimation = GetComponent<StartupAnimation>();
        Physics.autoSimulation = false;
        Physics.Simulate(1000000f);
        _saveScript = SaveScript.Instance;
        if (_saveScript == null) SceneManager.LoadScene("LogoEnAppOpstart");
    }

    private bool _firstFrame = true;

    private void Update()
    {
        if (_firstFrame && _saveScript.ready)
        {
            _firstFrame = false;
            if (SceneManager.GetActiveScene().name.Equals("LogoEnAppOpstart"))
            {
                SceneManager.LoadScene("inlogEnVoorplaatApp");
            }
        }
    }

    public void OpenGameChoiceMenu()
    {
        if (!_startupAnimation.finished)
            _startupAnimation.finished = true;
        else
            SceneManager.LoadScene("GameChoiceMenu");
    }
}