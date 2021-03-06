﻿using UnityEngine;
using System.Collections;
using NewtonVR;

public class Pipe : MonoBehaviour
{
    public GameObject CubePrefab;
    public GameObject[] PresentPrefabs;
    public GameObject OwnerRotator;
    public Transform CubeSpawnPoint;
    public CubeSide Side;
    public float AnimationTime;
    public Transform ExitPoint;
	public float ExitSpeed = 1.2f;

	[HideInInspector]
    public bool Closed;

	Animator Anim;
	DropZone dropZone;
    bool animating;

    void Awake()
    {
        dropZone = GetComponentInChildren<DropZone>();
		Anim = GetComponent<Animator>();
    }

    void Start()
    {

    }
	
	void Update()
    {
        if (animating) return;

        if (!Closed)
        {
            if (dropZone.ContainsCube)
            {
                PresentCube cube = dropZone.ContainedCube;
                if (cube.HasLid && cube.Stationary)
                {
                    MovableCube mCube = cube.GetComponent<MovableCube>();
					if (mCube.Side == Side)
					{
						StartCoroutine(ClosePipe(mCube));
					}
                }
            }
        }
    }

    /// <summary>
    /// Spawn the cube and pass it a randomly chosen present
    /// </summary>
    public GameObject SpawnCube(NVRHand hand)
    {
        GameObject cube = Instantiate(CubePrefab);
        cube.transform.position = CubeSpawnPoint.position;
        cube.transform.rotation = Quaternion.Euler(Random.Range(-30, 30), Random.Range(0, 360), Random.Range(-30, 30));

        // Assign the side (left/right) of the cube
        var movableCube = cube.GetComponent<MovableCube>();
        movableCube.Side = Side;
        movableCube.OwnerRotator = hand.gameObject;

        movableCube.Init(hand);

		// Spawn a random present
        GameObject randomPresentPrefab = PresentPrefabs[Random.Range(0, PresentPrefabs.Length)];
        GameObject randomPresent = Instantiate(randomPresentPrefab);

        var presentCube = cube.GetComponent<PresentCube>();
        presentCube.AssignPresent(randomPresent);

		StartCoroutine(OpenPipe());

		return cube;
    }

	IEnumerator ClosePipe(MovableCube cube)
    {
        animating = true;
		if (cube != null) cube.TakeOutOfPlay();

		Anim.SetTrigger("Close");
		transform.Find("SlideSound").GetComponent<AudioSource>().Play();
		yield return new WaitForSeconds(AnimationTime);

        if (cube != null)
        {
            Anim.SetTrigger("Hatch");
			cube.gameObject.transform.Find("WholeCollider").gameObject.SetActive(false);
		}

		// try to spawn new cube
		StartCoroutine(RespawnCube(cube));

        Closed = true;
        animating = false;
    }

	IEnumerator RespawnCube(MovableCube cube)
	{
		transform.Find("WhompSound").GetComponent<AudioSource>().Play();
		yield return new WaitForSeconds(Random.Range(2.0f, 5.0f));
		NVRHand hand = cube.GetComponent<MovableCube>().hand;
		Destroy(cube.gameObject);

		GameObject.Find("General").GetComponent<GameFlow>().SpawnCube(Side);
	}

	IEnumerator OpenPipe()
    {
        animating = true;
		yield return new WaitForSeconds(2.0f);

		Anim.SetTrigger("Open");
		transform.Find("SlideSound").GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(AnimationTime);

        Closed = false;
        animating = false;
    }
}
