using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public float speed = 1f;
    Rigidbody rb;
    bool pressable = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                StartCoroutine(MoveUpDown());
            }
        }
    }

    IEnumerator MoveUpDown()
    {
        if (pressable)
        {
            rb.AddForce(Vector3.up * speed, ForceMode.Impulse);
            pressable = false;
            Debug.Log(this.transform.position);
            yield return new WaitForSeconds(5f);
            rb.AddForce(Vector3.down * speed, ForceMode.Impulse);
            pressable = true;
        }
    }
}
