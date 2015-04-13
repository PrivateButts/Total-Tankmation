using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DestructableTerrain : MonoBehaviour {
	public float Seed, Scale;
	public Terrain BaseTerrain;
	private float[,] GeneratedHeightMap;
	// Use this for initialization
	void Start () {
		TerrainData terrain = BaseTerrain.terrainData;
		//Generate a huge array for fun
		GeneratedHeightMap = new float[terrain.heightmapWidth, terrain.heightmapWidth];
		float Noise;

		for (int y = 0; y < terrain.heightmapWidth; y++) {
			for (int x = 0; x < terrain.heightmapWidth; x++) {
				// Make are coords for PerlinNoise
				float xCoord = Seed + (float)x / (float)terrain.heightmapWidth * Scale;
				float yCoord = Seed + (float)y / (float)terrain.heightmapWidth * Scale;
				Noise = Mathf.PerlinNoise (xCoord, yCoord);
				// Make sure our heightmap doesn't get too tall
				GeneratedHeightMap [y, x] = Mathf.Clamp (Noise, 0, 1);
			}
		}
		// Set the new heightmap and clear the placeholder array
		terrain.SetHeights (0, 0, GeneratedHeightMap);
	}

	public void Crater(int xCoord, int yCoord, int radius, float Deepness = 150f){
		TerrainData terrain = BaseTerrain.terrainData;
		Vector2 center = new Vector2 (0, 0);
		for (int z = 0; z<2*radius; z++) {
			for (int x = 0; x<2*radius; x++) {
				Vector2 wanted = new Vector2 (x-radius, z-radius);
				float height = -Mathf.Sqrt(Mathf.Pow(radius,2)-Mathf.Pow(Vector2.Distance(wanted, center),2));
				GeneratedHeightMap[yCoord+Mathf.RoundToInt(wanted.y),xCoord+Mathf.RoundToInt(wanted.x)] = float.IsNaN(height) ? GeneratedHeightMap[yCoord+Mathf.RoundToInt(wanted.y),xCoord+Mathf.RoundToInt(wanted.x)] : GeneratedHeightMap[yCoord+Mathf.RoundToInt(wanted.y),Mathf.RoundToInt(xCoord+wanted.x)]+(height/Deepness);
			}
		}

		terrain.SetHeights (0,0,GeneratedHeightMap);
	}
}
