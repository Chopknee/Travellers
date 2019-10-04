using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MakeOcean : MonoBehaviour
{
    public Vector2 size = new Vector2(20,20);
    public Vector3 positionStart = Vector3.zero;
    public Vector3 meshScale;
    public GameObject prefab;
    public List<GameObject> ocean = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        positionStart = transform.position;
        Vector3 tPos = positionStart;

        //meshScale = prefab.transform.localScale;
        //meshScale.y = 1;
        for (int x = 0; x < size.x; x++)
        {
            tPos.x += meshScale.x * 2;
            GameObject xi = Instantiate(prefab, tPos, Quaternion.identity);
            xi.transform.localScale = meshScale;
            
            xi.transform.parent = transform;
            ocean.Add(xi);
            for (int y = 0; y < size.y; y++)
            {
                
                tPos.z += meshScale.z * 2;
                GameObject yi = Instantiate(prefab, tPos, Quaternion.identity);
                yi.transform.parent = transform;
                yi.transform.localScale = meshScale;
                ocean.Add(yi);
            }
            tPos.z -= meshScale.z * 2 * size.y;
        }

        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
