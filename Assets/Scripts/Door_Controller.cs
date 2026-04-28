using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door_Controller : MonoBehaviour
{
    public bool isOpen;
    public string targetScene;

    public static bool levelTwoLocked = true;
    public static bool levelThreeLocked = true;

    public void Open()
    {
        if (this.gameObject.name == "Chapter2_Door" && !levelTwoLocked) //open L2 Door
        {

            isOpen = true;
            SceneManager.LoadScene(targetScene);

        }
        else if (this.gameObject.name == "Chapter3_Door" && !levelThreeLocked)  //open L3 Door
        {

            isOpen = true;
            SceneManager.LoadScene(targetScene);

        }
        else if (this.gameObject.name == "Ending_DoorL1") //finishing L1 Door
        {

            levelTwoLocked = false;
            Debug.Log("opening L2!");
            isOpen = true;
            SceneManager.LoadScene(targetScene);

        }
        else if (this.gameObject.name == "Ending_DoorL2") //finishing L2 Door
        {

            levelThreeLocked = false;
            Debug.Log("opening L3!");
            isOpen = true;
            SceneManager.LoadScene(targetScene);

        }
        else if (this.gameObject.name != "Chapter3_Door" && this.gameObject.name != "Chapter2_Door")  //ALL OTHER DOORS
        {

            isOpen = true;
            SceneManager.LoadScene(targetScene);

        }
        else  //LOCKED L2 or L3 Door
        {
            isOpen = false;
            Debug.Log("Door not open!");
        }
    }

}
