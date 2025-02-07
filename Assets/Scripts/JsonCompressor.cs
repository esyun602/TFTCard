using System.IO;
using System.IO.Compression;
using System.Text;


public static class JsonCompressor
{
	public static byte[] CompressJson(string json)
	{
		byte[] raw = Encoding.UTF8.GetBytes(json);

		using (var outputStream = new MemoryStream())
		{
			using (var gzip = new GZipStream(outputStream, CompressionMode.Compress, leaveOpen: true))
			{
				gzip.Write(raw, 0, raw.Length);
			}
			// gzip이 닫히는 시점에 압축 완료됨
			return outputStream.ToArray();
		}
	}

	public static string DecompressJson(byte[] compressed)
	{
		using (var inputStream = new MemoryStream(compressed))
		using (var gzip = new GZipStream(inputStream, CompressionMode.Decompress))
		using (var reader = new StreamReader(gzip, Encoding.UTF8))
		{
			return reader.ReadToEnd();
		}
	}
}