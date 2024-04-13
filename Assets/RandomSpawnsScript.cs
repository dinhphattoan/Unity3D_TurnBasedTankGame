using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawnsScript : MonoBehaviour
{
    public float fRadius = 10f;

    public int nSpawns = 2;
    public float fSeperate_Distance = 10f;
    float leftDistance, rightDirection;
    public List<GameObject> ListSpawns = new List<GameObject>();
    // Start is called before the first frame update
    // Middle vertical of sphere is default 0
    float fSphereHeightScale = 0;
    void Start()
    {
        fRadius = this.GetComponent<SphereCollider>().radius;
        fSphereHeightScale = this.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void onClickRandom()
    {
        
        //this.rightDirection = this.fRadius/2;
        //this.leftDistance = rightDirection * -1;
        //for (int i=0;i<ListSpawns.Count;i++)
        //{
        //    Vector3 targetVector = new Vector3(UnityEngine.Random.Range(leftDistance,rightDirection),this.fSphereHeightScale, UnityEngine.Random.Range(leftDistance,rightDirection));
        //    bool flag = false;
        //    for (int j=i;j<ListSpawns.Count;j++)
        //    {
        //        if (Vector3.Distance(ListSpawns[j].transform.position,targetVector)>this.fSeperate_Distance)
        //        {
        //            break;
        //        }
        //    }
        //    flag = true;
        //    if (!flag)
        //    {
        //        i--;s
        //        continue;
        //    }
        //    ListSpawns[i].transform.position = targetVector;
            
            
        //}
    }
}

