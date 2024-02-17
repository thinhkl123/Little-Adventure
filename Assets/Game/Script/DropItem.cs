using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    public List<GameObject> weapons;

    public void DropSword()
    {
        foreach (GameObject weapon in weapons)
        {
            weapon.GetComponent<Rigidbody2D>();
            weapon.GetComponent<BoxCollider>();
            weapon.transform.parent = null;
        }
    }
}
