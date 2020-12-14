using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockDoor : MonoBehaviour
{
    public GameObject GameObject;

    private bool isDestroyed = false;
    // Update is called once per frame
    void Update()
    {
        if (this.GameObject == false)
        {
            isDestroyed = true;
        }
        if (isDestroyed == true)
        {
            Destroy(this.gameObject);
        }
    }
}
