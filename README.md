# GpxParser

A lightweight .NET library for parsing GPX (GPS Exchange Format) files, allowing flexible extraction of track points with customizable output.

## Features
- Parse GPX files to extract track points (latitude, longitude, time, and more).
- Supports Base64-encoded or raw XML input.
- Generic output for flexible data structures (tuples, custom classes, etc.).
- Robust handling of XML namespaces and invalid data.
- Culture-invariant parsing for consistent results across locales.
- Minimal dependencies (uses only `System.Xml.Linq`).

## Installation
Install the `GpxParser` NuGet package using the .NET CLI or Package Manager Console:

### .NET CLI
```bash
dotnet add package GpxParser
```

### Package Manager Console
```powershell
Install-Package GpxParser
```

Alternatively, search for `GpxParser` in the Visual Studio NuGet Package Manager.

## Usage

### Basic Example
Extract latitude, longitude, and time from a GPX file:

```csharp
using System;
using System.Collections.Generic;
using System.Globalization;

string gpxContent = "BASE64_ENCODED_GPX_STRING"; // or raw GPX XML

var points = GpxParser.ExtractGpxTrackPoints(
    gpxContent,
    (trkpt, ns) =>
    {
        if (!decimal.TryParse(trkpt.Attribute("lat")?.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat) ||
            !decimal.TryParse(trkpt.Attribute("lon")?.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var lon))
        {
            return null;
        }

        var timeStr = trkpt.Element(ns + "time")?.Value;
        if (string.IsNullOrEmpty(timeStr) || !DateTime.TryParse(timeStr, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var time))
        {
            return null;
        }

        return (Latitude: lat, Longitude: lon, Time: time);
    },
    isBase64: true // Set to false for raw XML
);

foreach (var point in points)
{
    Console.WriteLine($"Lat: {point.Latitude}, Lon: {point.Longitude}, Time: {point.Time}");
}
```

### Advanced Example
Extract additional elements, such as elevation, into a custom object:

```csharp
using System;
using System.Collections.Generic;
using System.Globalization;

string gpxContent = "<gpx>...</gpx>"; // Raw GPX XML

var points = GpxParser.ExtractGpxTrackPoints(
    gpxContent,
    (trkpt, ns) =>
    {
        if (!decimal.TryParse(trkpt.Attribute("lat")?.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat) ||
            !decimal.TryParse(trkpt.Attribute("lon")?.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var lon))
        {
            return null;
        }

        var timeStr = trkpt.Element(ns + "time")?.Value;
        if (string.IsNullOrEmpty(timeStr) || !DateTime.TryParse(timeStr, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var time))
        {
            return null;
        }

        decimal? elevation = null;
        var eleStr = trkpt.Element(ns + "ele")?.Value;
        if (!string.IsNullOrEmpty(eleStr))
        {
            decimal.TryParse(eleStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var ele);
            elevation = ele;
        }

        return new
        {
            Latitude = lat,
            Longitude = lon,
            Time = time,
            Elevation = elevation
        };
    },
    isBase64: false
);

foreach (var point in points)
{
    Console.WriteLine($"Lat: {point.Latitude}, Lon: {point.Longitude}, Time: {point.Time}, Elevation: {point.Elevation}");
}
```

## API Reference

### `ExtractGpxTrackPoints<T>`
Extracts track points from a GPX file or string.

```csharp
public static List<T> ExtractGpxTrackPoints<T>(
    string input,
    Func<XElement, XNamespace, T?> pointExtractor,
    bool isBase64 = false)
```

- **Parameters**:
  - `input`: The GPX content (Base64-encoded or raw XML string).
  - `pointExtractor`: A function to extract data from each `<trkpt>` element, returning a user-defined type `T` or `null` to skip invalid points.
  - `isBase64`: Set to `true` if the input is Base64-encoded; otherwise, `false`.
- **Returns**: A `List<T>` containing the extracted track points.
- **Throws**:
  - `ArgumentNullException`: If `input` is null or empty.
  - `ArgumentException`: If the input is invalid (e.g., malformed XML or Base64).

## Requirements
- .NET 9.
- No external dependencies beyond the .NET runtime.

## Contributing
Contributions are welcome! Please submit issues or pull requests to the [GitHub repository](https://github.com/USERNAME/GpxParser).

## License
This library is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Support
For questions or issues, please open an issue on the [GitHub repository](https://github.com/USERNAME/GpxParser) or contact the maintainer at [email@example.com].