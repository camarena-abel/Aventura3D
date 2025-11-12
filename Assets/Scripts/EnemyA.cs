using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyA : EnemyX
{
    Vector3 raySource = Vector3.zero;
    Vector3 rayTarget = Vector3.zero;


    [SerializeField]
    LayerMask detectionRayMask;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player") 
        {           
            // el player ha entrado en la zona de deteccion, pero vamos a confirmar
            // que existe vision directa, lanzando un rayo hasta el player
            raySource = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
            rayTarget = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);
            Vector3 rayDirection = (rayTarget - raySource).normalized;

            // primero calculamos el vector el producto escalar entre hacia donde mira el enemigo
            // y la direccion del rayo
            float dot = Vector3.Dot(new Vector3(rayDirection.x, 0f, rayDirection.z), new Vector3(transform.forward.x, 0f, transform.forward.z));
            if (dot < 0f)
            {
                // el player esta fuera del cono de vision del enemigo
                return;
            }

            RaycastHit detectionInfo;
            Physics.queriesHitTriggers = false;
            if (Physics.Raycast(raySource, rayDirection, out detectionInfo, proximityTriggerZone.radius, detectionRayMask))
            {
                if (detectionInfo.collider.gameObject.tag == "Player")
                {
                    targetFound = true;
                    fotgotTarget = 10f; // en 10 segundos olviadrá al player si no lo vuelve a ver
                }
            }
        }
    }    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(raySource, rayTarget);
    }

}
