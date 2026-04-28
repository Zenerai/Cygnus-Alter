using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Start_Menu_Controller : MonoBehaviour
{
   public void OnStartClick()
   {
        SceneManager.LoadScene("Chapter1");
   }

   public void OnExitClick()
   {
        Application.Quit();
   }

    public void OnCreditsClick()
    {
        SceneManager.LoadScene("Credits_Screen");
    }

    public void OnTitleClick()
    {
        SceneManager.LoadScene("Title_Screen");
    }

}
