using System.Xml;
using System.Xml.Linq;

namespace GpxParser;
/// <summary>
/// Provides functionality to parse GPX (GPS Exchange Format) content and extract track points.
/// </summary>
public static class GpxParser
{
    /// <summary>
    /// Extracts track points from a GPX XML string.
    /// </summary>
    /// <typeparam name="T">The type of objects to extract from each &lt;trkpt&gt; element.</typeparam>
    /// <param name="input">The GPX XML content or its Base64-encoded string.</param>
    /// <param name="pointExtractor">A function that defines how to extract the desired object from each track point element.</param>
    /// <param name="isBase64">Indicates whether the input string is Base64-encoded. Default is false.</param>
    /// <returns>A list of extracted objects of type <typeparamref name="T"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the input is null or empty.</exception>
    /// <exception cref="ArgumentException">Thrown when the input is not valid Base64 (if <paramref name="isBase64"/> is true), or not valid XML.</exception>
    public static List<T> ExtractGpxTrackPoints<T>(string input, Func<XElement, XNamespace, T?> pointExtractor, bool isBase64 = false)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentNullException(nameof(input), "Input cannot be null or empty.");
        }

        string gpxContent = isBase64 ? DecodeBase64ToText(input) : input;
        var dataPoints = new List<T>();

        try
        {
            var gpx = XElement.Parse(gpxContent);
            XNamespace ns = gpx.GetDefaultNamespace() ?? XNamespace.None;

            foreach (var trkpt in gpx.Descendants(ns + "trkpt"))
            {
                var point = pointExtractor(trkpt, ns);
                if (point != null)
                {
                    dataPoints.Add(point);
                }
            }
        }
        catch (XmlException ex)
        {
            throw new ArgumentException("Invalid GPX content.", nameof(input), ex);
        }

        return dataPoints;
    }

    private static string DecodeBase64ToText(string base64)
    {
        try
        {
            byte[] bytes = Convert.FromBase64String(base64);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
        catch (FormatException ex)
        {
            throw new ArgumentException("Invalid Base64 string.", nameof(base64), ex);
        }
    }
}