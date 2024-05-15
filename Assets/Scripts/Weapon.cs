using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    Animation weaponAnimation;

    private void Awake()
    {
        weaponAnimation = GetComponent<Animation>();
    }
    public void Attack()
    {
        weaponAnimation.Play();
    }
}
