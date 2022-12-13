using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ƒS[ƒ‹‚ğ§Œä‚·‚éƒNƒ‰ƒX
/// </summary>
public class GoalTrigger : MonoBehaviour
{
    [SerializeField]GameManager gameManager;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            gameManager.gameClear();
        }    
    }

}
