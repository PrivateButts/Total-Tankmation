using UnityEngine;
using System.Collections;

public class DestructableTerrain : MonoBehaviour {
	public float Seed, Scale;
	public Terrain BaseTerrain;
	// Use this for initialization
	void Start () {
		TerrainData terrain = BaseTerrain.terrainData;
		//Generate a huge array for fun
		float[,] GeneratedHeightMap = new float[terrain.heightmapWidth, terrain.heightmapWidth];
		float Noise;

		for(int y = 0; y < terrain.heightmapWidth; y++){
			for(int x = 0; x < terrain.heightmapWidth; x++){
				// Make are coords for PerlinNoise
				float xCoord = Seed + (float)x / (float)terrain.heightmapWidth * Scale;
				float yCoord = Seed + (float)y / (float)terrain.heightmapWidth * Scale;
				Noise = Mathf.PerlinNoise(xCoord,yCoord);
				// Make sure our heightmap doesn't get too tall
				GeneratedHeightMap[y,x] = Mathf.Clamp(Noise, 0, 1);
			}
			int a = 1+1;
		}
		// Set the new heightmap and clear the placeholder array
		terrain.SetHeights(0,0,GeneratedHeightMap);
		GeneratedHeightMap = null;

	}

	void Crater(int x, int y, float size){
		TerrainData terrain = BaseTerrain.terrainData;


	}
}
