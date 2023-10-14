using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BetterUI.Patches
{
  public static class ObjectSerialize
  {
    public static byte[] Serialize(this object obj)
    {
      if (obj == null)
      {
        return null;
      }

      using (MemoryStream memoryStream = new MemoryStream())
      {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(memoryStream, obj);
        byte[] compressed = Compress(memoryStream.ToArray());
        return compressed;
      }
    }

    public static object DeSerialize(this byte[] arrBytes)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        byte[] decompressed = Decompress(arrBytes);

        memoryStream.Write(decompressed, 0, decompressed.Length);
        memoryStream.Seek(0, SeekOrigin.Begin);

        return binaryFormatter.Deserialize(memoryStream);
      }
    }

    public static byte[] Compress(byte[] input)
    {
      byte[] compressesData;

      using (MemoryStream outputStream = new MemoryStream())
      {
        using (GZipStream zip = new GZipStream(outputStream, CompressionMode.Compress))
        {
          zip.Write(input, 0, input.Length);
        }
        compressesData = outputStream.ToArray();
      }

      return compressesData;
    }

    public static byte[] Decompress(byte[] input)
    {
      byte[] decompressedData;

      using (MemoryStream outputStream = new MemoryStream())
      {
        using (MemoryStream inputStream = new MemoryStream(input))
        {
          using (GZipStream zip = new GZipStream(inputStream, CompressionMode.Decompress))
          {
            zip.CopyTo(outputStream);
          }
        }

        decompressedData = outputStream.ToArray();
      }

      return decompressedData;
    }
  }

  class Helpers
  {
    public static string Repeat(string value, int count)
    {
      return new StringBuilder(value.Length * count).Insert(0, value, count).ToString();
    }

    public static string TimeString(double val1, double val2)
    {
      TimeSpan t = TimeSpan.FromSeconds(val1 - val2);
      return t.Hours > 0 ? $"{t.Hours:D2}h {t.Minutes:D2}m {t.Seconds:D2}s"
        : t.Minutes > 0 ? $"{t.Minutes:D2}m {t.Seconds:D2}s" : $"{t.Seconds:D2}s";
    }

    public static string TimeString(double seconds)
    {
      TimeSpan t = TimeSpan.FromSeconds(seconds);
      return t.Hours > 0 ? $"{t.Hours:D2}h {t.Minutes:D2}m {t.Seconds:D2}s"
        : t.Minutes > 0 ? $"{t.Minutes:D2}m {t.Seconds:D2}s" : $"{t.Seconds:D2}s";
    }

    public static void DebugLine(string str = "", bool pref = true, bool warn = false)
    {
      if (Main.isDebug.Value)
      {
        if (warn) Main.log.LogWarning($"{(pref ? $"[{typeof(Main).Namespace}] " : "")}{str}");
        else Main.log.LogInfo($"{(pref ? $"[{typeof(Main).Namespace}] " : "")}{str}");
      }
    }

    public static bool CheckHeldKey(KeyCode key)
    {
      try
      {
        return Input.GetKey(key);
      }
      catch
      {
        return false;
      }
    }
  }
}
