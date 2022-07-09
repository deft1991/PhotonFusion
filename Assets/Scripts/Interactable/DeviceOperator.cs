using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceOperator : MonoBehaviour
{
    /*
     * Distance for enable operate possibility
     */
    public float radius = 1.5f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire3"))
        {
            /*
             * OverlapSphere - return list of closest objects
             */
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider hitCollider in colliders)
            {
                Vector3 direction = hitCollider.transform.position - transform.position;
                
                /*
                 * Send message only if player looks on object
                 */
                if (Vector3.Dot(transform.forward, direction) > .5f)
                {
                    /*
                     * Try to call naming function
                     */
                    hitCollider.SendMessage("Operate", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }
}