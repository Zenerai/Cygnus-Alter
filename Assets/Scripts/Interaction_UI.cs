using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Interaction_UI : MonoBehaviour
{
   public static Interaction_UI instance;
    [SerializeField] TMP_Text interactionText;

    private void Awake()
   {
   instance = this;
   //interactionText = GameObject.Find("InteractText").GetComponent<TextMeshProUGUI>();
   }

   public void EnableInteractionText()
   {
    interactionText.gameObject.SetActive(true);
   }

   public void DisableInteractionText()
   {
    interactionText.gameObject.SetActive(false);
   }
}
