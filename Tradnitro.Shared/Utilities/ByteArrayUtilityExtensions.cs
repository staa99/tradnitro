using System;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;


namespace Tradnitro.Shared.Utilities
{
    public static class ByteArrayUtilityExtensions
    {
        public static async Task<byte[]?> NormalizeImageOrDefaultAsync(this byte[] bytes)
        {
            try
            {
                await using var ms = new MemoryStream(bytes);
                await using var outStream = new MemoryStream();
                using var image = await Image.LoadAsync(ms);
                image.Mutate(context => context.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(300,
                                    300)
                }));

                await image.SaveAsJpegAsync(outStream);
                return outStream.ToArray();
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync($"An error occurred while resizing the image:{ex}");
                return default;
            }
        }
    }
}