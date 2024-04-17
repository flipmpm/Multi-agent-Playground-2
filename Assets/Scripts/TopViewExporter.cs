using UnityEngine;
using System.IO;

public class TopViewExporter : MonoBehaviour
{
    public RenderTexture topViewRenderTexture; // Reference to the Render Texture where the top view is rendered

    void LateUpdate()
    {
        // Create a temporary texture and read the pixels from the Render Texture
        Texture2D tempTexture = new Texture2D(topViewRenderTexture.width, topViewRenderTexture.height);
        RenderTexture.active = topViewRenderTexture;
        tempTexture.ReadPixels(new Rect(0, 0, topViewRenderTexture.width, topViewRenderTexture.height), 0, 0);
        tempTexture.Apply();

        // Convert the texture data to an array of Color values
        Color[] pixelData = tempTexture.GetPixels();

        // Export the pixel data to a CSV file
        ExportToCSV(pixelData);
    }

    void ExportToCSV(Color[] pixelData)
    {
        // Define the path where the CSV file will be saved
        string filePath = Application.dataPath + "/top_view_data.csv";

        // Open a stream writer to write to the CSV file
        StreamWriter writer = new StreamWriter(filePath);

        // Write each pixel data to the CSV file
        for (int i = 0; i < pixelData.Length; i++)
        {
            Color pixel = pixelData[i];
            string pixelLine = string.Format("{0},{1},{2},{3}", pixel.r, pixel.g, pixel.b, pixel.a);
            writer.WriteLine(pixelLine);
        }

        // Close the writer
        writer.Close();
    }
}
