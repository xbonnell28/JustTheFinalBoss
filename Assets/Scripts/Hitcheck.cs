using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitcheck : MonoBehaviour
{
    public string hazardLayer;
    [SerializeField] private PlayerController playerController;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer(hazardLayer))
        {
            playerController.pogoKnockback();
        }
    }
}
