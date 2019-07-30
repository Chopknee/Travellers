using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator {

    public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height) {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;

        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D TextureFromHeightMap(float[,] heightMap) {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }
        return TextureFromColorMap(colorMap, width, height);
    }

    public static void BlurMap(float[,] map, int blurRadius = 3) {
        float[,] intputMap = new float[map.GetLength(0), map.GetLength(1)];
        System.Array.Copy(map, intputMap, map.Length);
        double[,] kernel = MakeKernel(blurRadius);

        for (int x = 0; x < map.GetLength(0); x++) {
            for (int y = 0; y < map.GetLength(1); y++) {
                //For every pixel
                float outNum = 0;
                for (int kx = 0; kx < kernel.GetLength(0); kx++) {
                    for (int ky = 0; ky < kernel.GetLength(1); ky++) {
                        //For every pixel in the kernel
                        float val = (float)kernel[kx,ky];
                        //map the pixels
                        int inX = x + (kx - blurRadius);
                        int inY = y + (ky - blurRadius);
                        inX = (inX < 0)? 0 : inX;inX = (inX >= map.GetLength(0))? map.GetLength(0)-1 : inX;
                        inY = (inY < 0)? 0 : inY;inY = (inY >= map.GetLength(1))? map.GetLength(1)-1 : inY;
                        outNum += val * intputMap[inX, inY];
                    }
                }
                map[x,y] = outNum;
            }
        }
    }

    public static double[,] MakeKernel(int radius) {
        double sigma = radius / 2.0;
        double[,] kernel = new double[2*radius+1, 2*radius+1];
        double sum = 0;
        
        for (int y = 0; y < kernel.GetLength(1); y++) {
            for (int x = 0; x < kernel.GetLength(0); x++) {
                double k = gaussian(x, radius, sigma) * gaussian(y, radius, sigma);
                kernel[x,y] = k;
                sum += k;
            }
        }

        for (int y = 0; y < kernel.GetLength(1); y++) {
            for (int x = 0; x < kernel.GetLength(0); x++) {
                kernel[x,y] /= sum;
            }
        }
        return kernel;
    }

    static double gaussian(double x, double mu, double sigma) {
        double a = (x - mu) / sigma;
        return System.Math.Exp(-0.5f * a * a);
    }
}
