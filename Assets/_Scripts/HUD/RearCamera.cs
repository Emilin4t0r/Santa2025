using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RearCamera : MonoBehaviour
{
    public static RearCamera instance;

    Camera cam;
	public bool flipHorizontal;	

	public Transform trackTarget;
	Quaternion origRot;

    AircraftUtils au;
	public TVStatic tvStatic;

    void Awake()
	{
		cam = GetComponent<Camera>();
		instance = this;
    }

    private void Start()
    {
        au = AircraftUtils.instance;
        origRot = transform.localRotation;
    }

    void OnPreCull()
	{
		cam.ResetWorldToCameraMatrix();
		cam.ResetProjectionMatrix();
		Vector3 scale = new Vector3(flipHorizontal ? -1 : 1, 1, 1);
		cam.projectionMatrix = cam.projectionMatrix * Matrix4x4.Scale(scale);
	}

	void OnPreRender()
	{
		GL.invertCulling = flipHorizontal;
	}

	void OnPostRender()
	{
		GL.invertCulling = false;
	}

    private void Update()
    {
        if (!au.turnedOn) return;

        if (trackTarget)
		{
			transform.LookAt(trackTarget);
			cam.fieldOfView = Vector3.Distance(transform.position, trackTarget.position) / 120;
		}
    }

    public void StartTrack(Transform target)
	{				
		trackTarget = target;
        StartCoroutine(TVStatic(0.4f));
    }
	public void FreeCamera()
	{
        transform.localRotation = origRot;
		trackTarget = null;
		cam.fieldOfView = 20;
		StartCoroutine(TVStatic(0.4f));
    }

	IEnumerator TVStatic(float length)
	{
		tvStatic.ToggleStatic(true);
        yield return new WaitForSeconds(length);
        tvStatic.ToggleStatic(false);
    }
}
