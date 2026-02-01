using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;


[ExecuteInEditMode]
public class ElectricTrapEditorHelper : MonoBehaviour
{
    public Transform Pos1;
    public Transform Pos2;
    public Transform Pos3;
    public Transform Pos4;

    public float maxDistance = 20.0f;

    public Vector3 parentPos;
    
    void Update()
    {
        if (transform.hasChanged == false)
            return;
        
        Vector3 midPoint = transform.parent.position;//(Pos1.position + Pos4.position) * 0.5f;
        parentPos = midPoint;
        midPoint.y -= 3;

        //Debug.Log("Mid Point: " + midPoint);

        // ray cast toward Pos1 and find new Pos1
        {
            Vector3 direction = Pos1.position - midPoint;
            direction = direction.normalized;

            RaycastHit hit;

            if (Physics.Raycast(midPoint, direction, out hit, maxDistance))
                Pos1.position = hit.point;
            else
                Pos1.position = midPoint + (direction * maxDistance);
        }

        // ray cast toward Pos4 and find new Pos4
        {
            Vector3 direction = Pos4.position - midPoint;
            direction = direction.normalized;

            RaycastHit hit;

            if (Physics.Raycast(midPoint, direction, out hit, maxDistance))
                Pos4.position = hit.point;
            else
                Pos4.position = midPoint + (direction * maxDistance);
        }

        // update Pos2 Pos3
        {
            float spacing = Vector3.Distance(Pos1.position, Pos4.position) / 3.0f;
            
            Vector3 direction = Pos4.position - Pos1.position;
            direction = direction.normalized;

            Pos2.position = Pos1.position + (direction * spacing);
            Pos3.position = Pos1.position + (direction * spacing * 2.0f);
        }
    }
}
#endif
