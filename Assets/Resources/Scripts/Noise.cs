using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
	public static float[,] CalculateNoise(int width, int height, float scale, int octaves, float persistance, float lacunarity, Vector2 offset) {
        float[,] noiseMap = new float[width, height];

        if (scale <= 0) {
            scale = 0.001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
        
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++) {
                    float xCoord = (x - halfWidth) / scale * frequency + offset.x;
                    float yCoord = (y - halfHeight) / scale * frequency + offset.y;

                    float sample = Mathf.PerlinNoise(xCoord, yCoord) * 2 - 1;
                    noiseHeight += sample * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;

                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
	}
}
