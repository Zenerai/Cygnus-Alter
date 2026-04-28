using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Controller : MonoBehaviour
{
    public bool isTalk;
    public string[] lines;

    public void Talk()
    {
        isTalk = true;
        Dialogue.Instance.StartDialogue(lines);
    }
}
