﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffBehaviour : MonoBehaviour
{

	public float energyConsumption = 3f;

	private bool lightBeamActive;
	public Transform muzzle;
	private LineRenderer lightBeam;
	// Layer 9-14; R - Y(G) - B - O - P - G, only RYB usable for lightbeam, purple objects can be hit by both
	private int validLayermask;
	private Color currentColor;

	// How long/much can the lightBeam be used, decreases in Update while lightBeamActive
	private float lightPower = 100f;

	// Use this for initialization
	void Awake()
	{
		lightBeam = muzzle.GetComponent<LineRenderer>();
		lightBeam.enabled = false;
		validLayermask = LayerMask.GetMask("Default"); // Yellow, standard
	}

	// Update is called once per frame
	void Update()
	{
		if (lightBeamActive)
		{
			if (lightPower <= 0)
			{
				lightPower = 0;
				DeactivateLightBeam();
				return;
			}

			lightBeam.SetPosition(0, muzzle.transform.position);
			RaycastHit hit;
			if (Physics.Raycast(muzzle.transform.position, muzzle.transform.forward, out hit, 100f, validLayermask))
			{
				if (hit.collider)
				{
					lightBeam.SetPosition(1, hit.point);
					// TODO Call function on hit object if it is lightbeam-interactable
					if (hit.collider.tag == "SingleColorLightBlock")
					{
						hit.transform.GetComponent<SingleColorLightBlock>().ActivateLightblock(energyConsumption);
					}
					else if (hit.collider.tag == "MultiColorLightBlock")
					{
						hit.transform.GetComponent<MultiColorLightBlock>().ActivateLightblock(energyConsumption, currentColor);
					}
				}
			}
			else
			{
				lightBeam.SetPosition(1, muzzle.transform.position + muzzle.transform.forward * 1000f);
			}

			lightPower -= Time.deltaTime * energyConsumption;
		}
	}

	public void ActivateLightBeam()
	{
		if (lightPower <= 0)
			return;
		lightBeamActive = true;
		lightBeam.enabled = true;
	}

	public void DeactivateLightBeam()
	{
		lightBeamActive = false;
		lightBeam.enabled = false;
	}

	public void ReloadNRG()
	{
		lightPower = 100f;
	}

	public void ChangeGem(Color changeToColor)
	{
		lightBeam.startColor = changeToColor;
		currentColor = changeToColor;

		// valid layermask = Lightblocks allowed to hit
		if (changeToColor == Color.red)
			validLayermask = LayerMask.GetMask("Default", "Red", "Orange", "Purple");
		else if (changeToColor == Color.yellow)
			validLayermask = LayerMask.GetMask("Default", "Yellow", "Orange", "Green");
		else
			validLayermask = LayerMask.GetMask("Default", "Blue", "Green", "Purple");
	}
}
