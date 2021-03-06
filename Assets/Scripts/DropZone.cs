﻿using UnityEngine;
using System.Collections;
using System.Security.Cryptography;

public class DropZone : MonoBehaviour
{
	[HideInInspector]
	public bool ContainsCube;

    [HideInInspector]
    public PresentCube ContainedCube;

    private SphereCollider sphereCollider;

    void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }

    /// <summary>
    /// If the MovableCube stays within this trigger and is completely contained, assign ContainsCube and ContainedCube.
    /// </summary>
    /// <param name="collider"></param>
    void OnTriggerStay(Collider collider)
    {
        // Check if the other collider is completely encapsuled in our sphere collider
        if (collider.tag == "MovableCube")
        {
            // Find all corners of the cube, and see if all of them are inside the trigger
            GameObject cube = collider.gameObject;
            // TODO make const
            Vector3[] corners = {
                new Vector3(1, 1, 1),
                new Vector3(1, 1, -1),
                new Vector3(1, -1, 1),
                new Vector3(-1, 1, 1),
                new Vector3(1, -1, -1),
                new Vector3(-1, 1, -1),
                new Vector3(-1, -1, 1),
                new Vector3(-1, -1, -1)
            };

            bool partiallyOutside = false;

            foreach (Vector3 corner in corners)
            {
                Vector3 cornerPosition = cube.transform.TransformPoint(corner * 0.3f / 2.0f);
                if (Vector3.Distance(cornerPosition, transform.position) >= sphereCollider.radius)
                {
                    partiallyOutside = true;
                    break;
                }
            }

			// If the whole cube is contained within the sphere
			ContainsCube = !partiallyOutside;
            ContainedCube = !partiallyOutside ? cube.transform.parent.gameObject.GetComponent<PresentCube>() : null;
        } 
    }

	void OnTriggerExit(Collider collider)
	{
		if (collider.tag == "MovableCube")
		{
			ContainedCube = null;
			ContainsCube = false;
		}
	}
    
}
