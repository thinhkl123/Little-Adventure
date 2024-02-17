using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCaster : MonoBehaviour
{
    private Collider _damageCasterCollider;
    public int damage = 30;
    public string targetTag;
    private List<Collider> _damagedTargetList;

    private void Awake()
    {
        _damageCasterCollider = GetComponent<Collider>();
        _damageCasterCollider.enabled = false;
        _damagedTargetList = new List<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.tag == targetTag && !_damagedTargetList.Contains(other))
        {
            Character targetCC = other.GetComponent<Character>();
            if (targetCC != null)
            {
                targetCC.ApplyDame(damage, transform.parent.position);

                PlayerVFXManager playerVFXManager = transform.parent.GetComponent<PlayerVFXManager>();

                if (playerVFXManager != null)
                {
                    RaycastHit hit;

                    Vector3 originalPos = transform.position + (-_damageCasterCollider.bounds.extents.z) * transform.forward;

                    bool isHit = Physics.BoxCast(originalPos, _damageCasterCollider.bounds.extents / 2, transform.forward, out hit, transform.rotation, _damageCasterCollider.bounds.extents.z, 1 << 6);
                    
                    if (isHit)
                    {
                        playerVFXManager.PlaySlash(hit.point + new Vector3(0, 0.5f, 0));
                    }
                }
            }
            _damagedTargetList.Add(other);
        }
    }

    public void EnableDamageCaster()
    {
        _damagedTargetList.Clear();
        _damageCasterCollider.enabled=true;
    }

    public void DisableDamageCaster()
    {
        _damagedTargetList.Clear();
        _damageCasterCollider.enabled=false;
    }

    private void OnDrawGizmos()
    {
        if (_damageCasterCollider == null)
        {
            _damageCasterCollider = GetComponent<Collider> ();
        }
        RaycastHit hit;

        Vector3 originalPos = transform.position + (-_damageCasterCollider.bounds.extents.z) * transform.forward;

        bool isHit = Physics.BoxCast(originalPos, _damageCasterCollider.bounds.extents / 2, transform.forward, out hit, transform.rotation, _damageCasterCollider.bounds.extents.z, 1 << 6);

        if (isHit)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(hit.point, 0.5f);
        }
    }
}
