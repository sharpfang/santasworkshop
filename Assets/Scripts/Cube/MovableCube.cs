﻿using UnityEngine;
using System.Collections;

public enum CubeSide
{
    Left = 0,
    Right = 1
}

public class MovableCube : MonoBehaviour
{
	public float StationaryThreshold;

	public bool Stationary
	{
		get { return IsStationary(); }
	}

	private Vector3[] lastPositions;
	private int lastPositionIndex;
	private int lastPositionsMax = 60;

    [HideInInspector]
    public CubeSide Side;
    [HideInInspector]
    public GameObject OwnerRotator;

    // Privates

	private bool wasStationary; //temporary?

    private bool isInPlay;
    private bool isAnimatingSpawn;
    private bool isAnimatingExit;

    // Animation helpers
    private float spawnAnimationProgress;
    public float spawnAnimationSpeed;
    private Vector3 spawnAnimationStartPosition;
    private Quaternion spawnAnimationStartRotation;

	void Awake()
	{
		// Initialize the lastPositions array
		lastPositions = new Vector3[lastPositionsMax];
		lastPositionIndex = 0;
		for (int i = 0; i < lastPositionsMax; ++i)
		{
			lastPositions[i] = transform.position;
		}
	}

    /// <summary>
    /// Cube spawns into existence. Starts animating the cube towards the assigned controller location.
    /// </summary>
    public void Init()
    {
        isAnimatingSpawn = true;

        spawnAnimationStartPosition = transform.position;
        spawnAnimationStartRotation = transform.rotation;
    }

    /// <summary>
    /// Assigns the cube as a child of the controller and does other activation duties like activating the present physics.
    /// </summary>
    public void TakeIntoPlay()
    {
        isAnimatingSpawn = false;
        // Announces itself to the general gameflow object
        GameObject.Find("General").GetComponent<GameFlow>().RegisterCube(gameObject);
        gameObject.transform.SetParent(OwnerRotator.transform);

        isInPlay = true;
        // TODO activate present physics? Perhaps the presnet should be instantiated already and should follow the box.
    }

    /// <summary>
    /// Takes the cube out of gameplay.
    /// </summary>
    public void TakeOutOfPlay()
    {
        isAnimatingExit = true;
        isInPlay = false;
    }

	void Update()
	{
		StorePosition();

		if (Stationary)
		{
			if (!wasStationary) // Start being stationary, can be used for events
			{
				wasStationary = true;
			}
		}
		else // Remove stationary status
		{
			wasStationary = false;
		}

        if (isAnimatingSpawn)
        {
            AnimateSpawn();
        }
        if (isAnimatingExit)
        {
            AnimateExit();
        }
        
	}

    /// <summary>
    /// Animates the object's rotation and position during the spawn.
    /// </summary>
    void AnimateSpawn()
    {
        float dt = Time.deltaTime;

        spawnAnimationProgress += dt * spawnAnimationSpeed;

        transform.position = Vector3.Lerp(spawnAnimationStartPosition, OwnerRotator.transform.position, spawnAnimationProgress);
        transform.rotation = Quaternion.Lerp(spawnAnimationStartRotation, OwnerRotator.transform.rotation, spawnAnimationProgress);

        if (spawnAnimationProgress >= 1f)
        {
            TakeIntoPlay();
        }
    }

    /// <summary>
    /// Animates the object's rotation and position when it is taken out of play.
    /// </summary>
    void AnimateExit()
    {

    }

	/// <summary>
	/// Stores current position into the lastPositions array. Overwrites old values.
	/// </summary>
	void StorePosition()
	{
		lastPositionIndex = (lastPositionIndex + 1) % lastPositionsMax;

		lastPositions[lastPositionIndex] = transform.position;
	}

	/// <summary>
	/// Returns true if the object has been relatively stationary.
	/// </summary>
	/// <returns></returns>
	bool IsStationary()
	{
		float sum = 0f;
		for (int i = 0; i < lastPositionsMax; ++i)
		{
			sum += Vector3.Distance(lastPositions[i], transform.position);
		}
		sum /= lastPositionsMax;

		return sum <= StationaryThreshold;
	}
}
