using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RearCamera : MonoBehaviour
{
    public static RearCamera instance;

    Camera cam;
	public bool flipHorizontal;
	

	Transform trackTarget;
	Quaternion origRot;

	void Awake()
	{
		cam = GetComponent<Camera>();
		instance = this;
    }

    private void Start()
    {
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
		if (trackTarget)
		{
			transform.LookAt(trackTarget);
			cam.fieldOfView = Vector3.Distance(transform.position, trackTarget.position) / 120;
		}
    }

    public void TrackTarget(Transform target)
	{		
		trackTarget = target;
    }
	public void FreeCamera()
	{
		transform.localRotation = origRot;
		trackTarget = null;
    }
}
