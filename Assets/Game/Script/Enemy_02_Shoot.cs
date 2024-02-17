using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_02_Shoot : MonoBehaviour
{
    public Transform shootingPoint;
    public GameObject damageOrb;

    private Character cc;

    private void Awake()
    {
        cc = GetComponent<Character>();
    }
    public void ShootTheDamageOrb()
    {
        Instantiate(damageOrb, shootingPoint.position, Quaternion.LookRotation(shootingPoint.forward));
    }

    private void Update()
    {
        cc.RotateToPlayer();
    }
}
