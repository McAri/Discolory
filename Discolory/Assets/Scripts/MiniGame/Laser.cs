﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Laser : MonoBehaviour {

    private LineRenderer lr;

    public GameObject successText;

	// Use this for initialization
	void Start () {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        lr.SetPosition(0, transform.position);


        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            lr.SetPosition(1, new Vector3(hit.point.x, hit.point.y + 0.5f, hit.point.z));

            if (hit.collider.tag == "Mirrors")
            {
                hit.collider.SendMessage("activateLaser");
            }

            if(hit.collider.tag == "Goal")
            {
                successText.SetActive(true);
            }

            /*
            if (hit.collider)
            {
                lr.SetPosition(1, hit.point);
            }*/
        }
        //else lr.SetPosition(1, transform.forward * 5000);
    }

    public int maxReflectionCount = 5;
    public float maxStepDistance = 200;

    void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.ArrowHandleCap(0, this.transform.position + this.transform.forward * 0.25f, this.transform.rotation, 0.5f, EventType.Repaint);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, 0.25f);

        DrawPredictedReflectionPattern(this.transform.position + this.transform.forward * 0.75f, this.transform.forward, maxReflectionCount);
    }

    private void DrawPredictedReflectionPattern(Vector3 position, Vector3 direction, int reflectionsRemaining)
    {
        if (reflectionsRemaining == 0)
        {
            return;
        }

        Vector3 startingPosition = position;

        Ray ray = new Ray(position, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxStepDistance))
        {
            direction = Vector3.Reflect(direction, hit.normal);
            position = hit.point;
        }
        else
        {
            position += direction * maxStepDistance;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startingPosition, position);

        DrawPredictedReflectionPattern(position, direction, reflectionsRemaining - 1);
    }
}
