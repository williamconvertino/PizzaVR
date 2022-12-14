using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script was made with help from this video: https://www.youtube.com/watch?v=11ofnLOE8pw&t=423s

public class BezierFollow : MonoBehaviour
{
    private List<Transform> routes;

    [SerializeField]
    private GameObject fullRoute;

    public int routeOffset;

    private int routeToGo;

    private float tParam;

    private Vector3 objectPosition;

    public float speedModifier;

    private bool coroutineAllowed;

    void Start()
    {
        routes = new List<Transform>();
        int routeNum = routeOffset;

        // add all the child route segments to the route (one iteration of the loop per segment)
        for (int i = 0; i < fullRoute.transform.childCount; i++)
        {
            routes.Add(fullRoute.transform.GetChild(routeNum).gameObject.transform);
            routeNum += 1;

            // when we surpass the number of routes, loop back to the first route
            if (routeNum == fullRoute.transform.childCount)
            {
                routeNum = 0;
            }

        }

        routeToGo = 0;
        tParam = 0f;
        coroutineAllowed = true;
    }

    void Update()
    {
        if (coroutineAllowed)
        {
            StartCoroutine(GoByTheRoute(routeToGo));
        }
    }

    private IEnumerator GoByTheRoute(int routeNum)
    {
        coroutineAllowed = false;

        Vector3 p0 = routes[routeNum].GetChild(0).position;
        Vector3 p1 = routes[routeNum].GetChild(1).position;
        Vector3 p2 = routes[routeNum].GetChild(2).position;
        Vector3 p3 = routes[routeNum].GetChild(3).position;

        while (tParam < 1)
        {
            tParam += Time.deltaTime * speedModifier;

            objectPosition = Mathf.Pow(1 - tParam, 3) * p0 + 3 * Mathf.Pow(1 - tParam, 2) * tParam * p1 + 3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p2 + Mathf.Pow(tParam, 3) * p3;

            transform.LookAt(objectPosition);
            transform.position = objectPosition;
            yield return new WaitForEndOfFrame();
        }

        tParam = 0;
        speedModifier = speedModifier * 0.90f;
        routeToGo += 1;

        if (routeToGo > routes.Count - 1)
        {
            routeToGo = 0;
        }

        coroutineAllowed = true;

    }


}