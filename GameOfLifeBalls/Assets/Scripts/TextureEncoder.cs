using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureEncoder
{
    private TextureOutput output;
    Color[] xPositionsAsColors;
    Color[] yPositionsAsColors;

    public TextureOutput Encode(List<Vector2> data)
    {
        int size = DetermineSize(data.Count);
        if (output == null || size > output.Size)
        {
            //Debug.Log($"Created new TextureOutPut with size {size}");
            output = new TextureOutput(size);
        }
        InsertDataIntoTextures(data);
        return output;
    }

    private int DetermineSize(int dataCount)
    {
        if (dataCount < 4)
        {
            return 2;
        }
        // Unity will make the texture size pow2 internally.
        // We do it manually here so that we can be certain the sampling is accurate
        return RoundUpToPowerOf2(Mathf.CeilToInt(Mathf.Sqrt(dataCount)));
    }

    private static int RoundUpToPowerOf2(int number)
    {
        if (number < 2)
        {
            return 1;
        }
        return (int)System.Math.Pow(2, (int)System.Math.Log(number - 1, 2) + 1);
    }

    private void InsertDataIntoTextures(List<Vector2> data)
    {

        if (data.Count < 1)
        {
            return;
        }

        int dataLength = data.Count;
        int blockWidth = output.Size;
        int blockHeight = dataLength;
        if (dataLength >= blockWidth)
        {
            blockHeight = Mathf.CeilToInt((1.0f * dataLength) / (1.0f * blockWidth));
        }
        if (dataLength < blockWidth)
        {
            blockWidth = dataLength;
            blockHeight = 1;
        }
        int arraySize = blockWidth * blockHeight;

        if (xPositionsAsColors == null || dataLength > xPositionsAsColors.Length)
        {
            xPositionsAsColors = new Color[arraySize];
        }
        if (yPositionsAsColors == null || dataLength > yPositionsAsColors.Length)
        {
            yPositionsAsColors = new Color[arraySize];
        }
        for (int index = 0; index < dataLength; index += 1)
        {
            xPositionsAsColors[index] = EncodeFloatRGBA(data[index].x);
            yPositionsAsColors[index] = EncodeFloatRGBA(data[index].y);
        }
        output.SetPixels(xPositionsAsColors, yPositionsAsColors, blockWidth, blockHeight);

    }

    private float DecodeRGBAFloat(Color value)
    {
        Vector4 colorAsVector = new Vector4(value.r, value.g, value.b, value.a);
        Vector4 kDecodeDot = new Vector4(1.0f, 1 / 255.0f, 1 / 65025.0f, 1 / 160581375.0f);
        return Vector4.Dot(colorAsVector, kDecodeDot);

    }
    private Color EncodeFloatRGBA(float value)
    {
        Vector4 encodeMultiplier = new Vector4(1.0f, 255.0f, 65025.0f, 160581375.0f);
        float encodeBit = 1.0f / 255.0f;
        Vector4 encoded = encodeMultiplier * value;
        encoded = Frac(encoded);
        encoded -= new Vector4(encoded.y, encoded.z, encoded.w, encoded.w) * encodeBit;
        return encoded;
    }

    private Vector4 Frac(Vector4 value)
    {
        return new Vector4(
            (float)(value.x - System.Math.Truncate(value.x)),
            (float)(value.y - System.Math.Truncate(value.y)),
            (float)(value.z - System.Math.Truncate(value.z)),
            (float)(value.w - System.Math.Truncate(value.w))
        );
    }
}

public class TextureOutput
{
    public Texture2D TextureX;
    public Texture2D TextureY;

    public int Size;

    public TextureOutput(int size)
    {
        TextureX = InitializeTexture(size);
        TextureY = InitializeTexture(size);
        Size = size;
    }

    public void SetPixels(Color[] xPixels, Color[] yPixels, int blockWidth, int blockHeight)
    {
        TextureX.SetPixels(0, 0, blockWidth, blockHeight, xPixels);
        TextureX.Apply(false);
        TextureY.SetPixels(0, 0, blockWidth, blockHeight, yPixels);
        TextureY.Apply(false);
    }

    private Texture2D InitializeTexture(int size)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBAFloat, false);
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.filterMode = FilterMode.Point;
        return texture;
    }
}