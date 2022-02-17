using UnityEngine;
using UnityEngine.SceneManagement;

public class KnoppenScriptTetris : MonoBehaviour
{
    public void nieuweTetris()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void terugNaarMenu()
    {
        SceneManager.LoadScene("SpellenOverzicht");
    }
}
