using UnityEngine;
using System.Collections;

public class DestructableTerrain : MonoBehaviour {
	public float Seed, Scale;
	public Terrain BaseTerrain;
	// Use this for initialization
	void Start () {
		TerrainData terrain = BaseTerrain.terrainData;
		//Randomize the Terrain
		float[,] GeneratedHeightMap = new float[terrain.heightmapWidth, terrain.heightmapWidth];
		float[,] OldHeightMap = new float[terrain.heightmapWidth, terrain.heightmapWidth];
		float Noise;
		OldHeightMap = terrain.GetHeights (0, 0, 513, 513);
		for(int y = 0; y < terrain.heightmapWidth; y++){
			for(int x = 0; x < terrain.heightmapWidth; x++){
				//Debug.Log(Mathf.Clamp(Mathf.PerlinNoise(Seed+x, Seed+y), 0, 1));
				float xCoord = Seed + (float)x / (float)terrain.heightmapWidth * Scale;
				float yCoord = Seed + (float)y / (float)terrain.heightmapWidth * Scale;
				Noise = Mathf.PerlinNoise(xCoord,yCoord);
				GeneratedHeightMap[y,x] = Mathf.Clamp(Noise, 0, 1);
			}
			int a = 1+1;
		}

		terrain.SetHeights(0,0,GeneratedHeightMap);
		GeneratedHeightMap = null;

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
