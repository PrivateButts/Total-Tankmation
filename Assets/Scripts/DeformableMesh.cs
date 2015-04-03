using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeformableMesh : MonoBehaviour {
	Mesh mesh;
	Material mat;
	List<Vector3> Points;
	List<Vector3> Verts;
	List<int> Tris;
	List<Vector2> UVs;

	float size = 0.5f;

	// Use this for initialization
	void Start () {
		Points = new List<Vector3> ();

		Points.Add (new Vector3 (-size, size, -size));
		Points.Add (new Vector3 (size, size, -size));
		Points.Add (new Vector3 (size, -size, -size));
		Points.Add (new Vector3 (-size, -size, -size));

		Points.Add (new Vector3 (size, size, size));
		Points.Add (new Vector3 (-size, size, size));
		Points.Add (new Vector3 (-size, -size, size));
		Points.Add (new Vector3 (size, -size, size));

		Verts = new List<Vector3> ();
		Tris = new List<Vector3> ();
		UVs = new List<Vector3> ();

		CreateMesh ();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void CreateMesh(){
		GameObject.AddComponent ("MeshFilter");
		GameObject.AddComponent ("MeshRenderer");
		GameObject.AddComponent ("MeshCollider");

		mat = Resources.Load ("Materials/Default") as Material;
		if (mat == null) {
			Debug.LogError ("Material Not Found!");
			return;
		}

		MeshFilter meshfilter = GetComponent<MeshFilter> ();
		if (meshfilter == null) {
			Debug.LogError ("MeshFilter Not Found!");
			return;
		}

		mesh = meshfilter.sharedMesh;
		if (mesh == null) {
			meshfilter.mesh = new Mesh();
			mesh = meshfilter.sharedMesh;
		}

		MeshCollider meshcollider = GetComponent<MeshCollider> ();
		if (meshcollider == null) {
			Debug.LogError ("No Mesh Collider!");
			return;
		}

		mesh.Clear ();
		UpdateMesh ();
	}

	private void UpdateMesh(){
		//Front
		Verts.Add (Points [0]);  Verts.Add (Points [1]);  Verts.Add (Points [2]);  Verts.Add (Points [3]);  
		//Back
		Verts.Add (Points [4]);  Verts.Add (Points [5]);  Verts.Add (Points [6]);  Verts.Add (Points [7]);  
		//Left
		Verts.Add (Points [5]);  Verts.Add (Points [0]);  Verts.Add (Points [3]);  Verts.Add (Points [6]);  
		//Right
		Verts.Add (Points [1]);  Verts.Add (Points [4]);  Verts.Add (Points [7]);  Verts.Add (Points [2]);
		//Top
		Verts.Add (Points [5]);  Verts.Add (Points [4]);  Verts.Add (Points [1]);  Verts.Add (Points [0]);  
		//Bottom
		Verts.Add (Points [3]);  Verts.Add (Points [2]);  Verts.Add (Points [7]);  Verts.Add (Points [6]);

		//Front
		Tris.Add (0); Tris.Add (1); Tris.Add (2);
		Tris.Add (2); Tris.Add (3); Tris.Add (0);
		//Back
		Tris.Add (4); Tris.Add (5); Tris.Add (6);
		Tris.Add (6); Tris.Add (7); Tris.Add (4);
		//Left
		Tris.Add (8); Tris.Add (9); Tris.Add (10);
		Tris.Add (10); Tris.Add (11); Tris.Add (8);
		//Right
		Tris.Add (12); Tris.Add (13); Tris.Add (14);
		Tris.Add (14); Tris.Add (15); Tris.Add (12);
		//Top
		Tris.Add (16); Tris.Add (17); Tris.Add (18);
		Tris.Add (18); Tris.Add (19); Tris.Add (16);
		//Bottom
		Tris.Add (20); Tris.Add (21); Tris.Add (22);
		Tris.Add (22); Tris.Add (23); Tris.Add (20);


}
