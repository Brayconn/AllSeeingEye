using System;
using System.Collections.Generic;
using System.Drawing;

namespace GuxtModdingFramework.Images
{
    public static class Scrambler
    {
        /// <summary>
        /// Hash A value that vanilla Guxt uses for image scrambling
        /// </summary>
        public const ushort HashAInit = 0x4444;
        /// <summary>
        /// Hash B value that vanilla Guxt uses for image scrambling
        /// </summary>
        public const ushort HashBInit = 0x8888;

        /// <summary>
        /// Used for scrambling/unscrambling Guxt's images
        /// </summary>
        /// <param name="a">Hash A value</param>
        /// <param name="b">Hash B value</param>
        /// <returns>High byte of Hash A's new value (minimum of 1)</returns>
        private static ushort Shuffle(ref ushort a, ref ushort b)
        {
            ushort sum = (ushort)(a + b);
            b = a;
            //a = (ushort)(a + b) but in reverse byte order
            a = (ushort)((sum << 8) | (sum >> 8));
            //Returning the high byte of a (minimum of 1)
            return (byte)Math.Max(a >> 8, 1);
        }

        /// <summary>
        /// Repetedly runs shuffle to get the scrambled image order
        /// </summary>
        /// <param name="h">Height of the image</param>
        /// <param name="a">Hash A value</param>
        /// <param name="b">Hash B value</param>
        /// <returns>The nth row's unscrambled position</returns>
        private static IEnumerable<int> Iterate(int h, ushort a, ushort b)
        {
            //Keeps track of what rows have been filled in
            List<int> free_rows = new List<int>(h);
            for (int i = 0; i < h; i++)
                free_rows.Add(i);

            int current_line = 0;
            //for each row in the image...
            for (int i = 0; i < h; i++)
            {
                //move Shuffle() rows down the image (looping around when reaching the end)
                current_line = (current_line + Shuffle(ref a, ref b)) % free_rows.Count;

                //we have now reached where the "i"th row in the decoded image is
                yield return free_rows[current_line];

                //now remove that row from list, and move the current_line back by one
                //moving back by one simulates how current_line would still be pointing to the old line in other implementations
                free_rows.RemoveAt(current_line--);
            }
        }

        /// <summary>
        /// Unscrambles an image using the standard hash values
        /// </summary>
        /// <param name="img">Image to unscramble</param>
        /// <returns>The unscrambled Image</returns>
        public static Bitmap Unscramble(Image img)
        {
            return Unscramble(img, HashAInit, HashBInit);
        }
        /// <summary>
        /// Unscrambles an image using the given A and B hashes
        /// </summary>
        /// <param name="img">Image to unscramble</param>
        /// <param name="a">Hash A value</param>
        /// <param name="b">Hash B value</param>
        /// <returns>The unscrambled image</returns>
        public static Bitmap Unscramble(Image img, ushort a, ushort b)
        {
            Bitmap newImage = new Bitmap(img.Width, img.Height);
            int i = 0;
            foreach (var row in Iterate(img.Height, a, b))
            {
                for (int j = 0; j < img.Width; j++)
                    newImage.SetPixel(j, row, ((Bitmap)img).GetPixel(j, i));
                i++;
            }
            return newImage;
        }

        /// <summary>
        /// Scrambles an image using the standard hash values
        /// </summary>
        /// <param name="img">The image to scramble</param>
        /// <returns>The scrambled image</returns>
        public static Bitmap Scramble(Image img)
        {
            return Scramble(img, HashAInit, HashBInit);
        }
        /// <summary>
        /// Scrambles an image with the given A and B hashes
        /// </summary>
        /// <param name="img">The image to scramble</param>
        /// <param name="a">Hash A value</param>
        /// <param name="b">Hash B value</param>
        /// <returns>The scrambled image</returns>
        public static Bitmap Scramble(Image img, ushort a, ushort b)
        {
            Bitmap newImage = new Bitmap(img.Width, img.Height);
            int i = 0;
            foreach (var row in Iterate(img.Height, a, b))
            {
                for (int j = 0; j < img.Width; j++)
                    newImage.SetPixel(j, i, ((Bitmap)img).GetPixel(j, row));
                i++;
            }
            return newImage;
        }
    }
}
