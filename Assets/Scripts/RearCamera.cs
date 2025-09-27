using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RearCamera : MonoBehaviour
{
    public static RearCamera instance;

    Camera cam;
	public bool flipHorizontal;
	

	bool trackingEnemy;
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

    private void FixedUpdate()
    {
		if(trackingEnemy)
			transform.LookAt(trackTarget);
    }

    public void TrackTarget(Transform target)
	{		
		trackingEnemy = true;
		trackTarget = target;
    }
	public void FreeCamera()
	{
        trackingEnemy = false;
		transform.localRotation = origRot;
    }
}
