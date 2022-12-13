using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �S�[���𐧌䂷��N���X
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
