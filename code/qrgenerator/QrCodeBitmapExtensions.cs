//
// QR code generator library (.NET)
// https://github.com/manuelbl/QrCodeGenerator
//
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Net.Codecrete.QrCodeGenerator
{
    /// <summary>
    /// <c>QrCode</c> extension for creating bitmaps using <c>System.Drawing</c> classes.
    /// <para>
    /// In .NET 6 and later versions, this extension will only work on Windows.
    /// </para>
    /// </summary>
    public static class QrCodeBitmapExtensions
    {
        /// <inheritdoc cref="ToBitmap(QrCode, int, int)"/>
        /// <param name="background">The background color.</param>
        /// <param name="foreground">The foreground color.</param>
        public static Bitmap ToBitmap(this QrCode qrCode, int scale, int border, Color foreground, Color background)
        {
            // check arguments
            if (scale <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(scale), "Value out of range");
            }
            if (border < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(border), "Value out of range");
            }

            int size = qrCode.Size;
            int dim = (size + border * 2) * scale;

            if (dim > short.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(scale), "Scale or border too large");
            }

            // create bitmap
            Bitmap bitmap = new Bitmap(dim, dim, PixelFormat.Format24bppRgb);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                Draw(qrCode, g, scale, border, foreground, background);
            }

            return bitmap;
        }

        
        /// <inheritdoc cref="Draw(QrCode, Graphics, float, float)"/>
        /// <param name="background">The background color.</param>
        /// <param name="foreground">The foreground color.</param>
        public static void Draw(this QrCode qrCode, Graphics graphics, float scale, float border, Color foreground, Color background)
        {
            if (scale <= 0 || border < 0)
            {
                return;
            }

            int size = qrCode.Size;
            float dim = (size + border * 2) * scale;

            // draw background
            if (background != null)
            {
                using SolidBrush brush = new SolidBrush(background);
                graphics.FillRectangle(brush, 0, 0, dim, dim);
            }

            // draw modules
            using (SolidBrush brush = new SolidBrush(foreground))
            {
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        if (qrCode.GetModule(x, y))
                        {
                            graphics.FillRectangle(brush, (x + border) * scale, (y + border) * scale, scale, scale);
                        }
                    }
                }
            }
        }

        /// <inheritdoc cref="SaveAsPng(QrCode, string, int, int)"/>
        /// <param name="background">The background color.</param>
        /// <param name="foreground">The foreground color.</param>
        public static void SaveAsPng(this QrCode qrCode, string filename, int scale, int border, Color foreground, Color background)
        {
            using Bitmap bitmap = qrCode.ToBitmap(scale, border, foreground, background);
            bitmap.Save(filename, ImageFormat.Png);
        }

        /// <summary>
        /// Saves this QR code as a PNG file.
        /// <para>
        /// The <paramref name="scale"/> parameter specifies the scale of the image, which is
        /// equivalent to the width and height of each QR code module. Additionally, the number
        /// of modules to add as a border to all four sides can be specified.
        /// </para>
        /// <para>
        /// For example, <c>SaveAsPng("qrcode.png", scale: 10, border: 4)</c> means to pad the QR code with 4 white
        /// border modules on all four sides, and use 10&#xD7;10 pixels to represent each module.
        /// </para>
        /// <para>
        /// If not specified, the foreground color is black (0x000000) und the background color always white (0xFFFFFF).
        /// </para>
        /// </summary>
        /// <param name="scale">The width and height, in pixels, of each module.</param>
        /// <param name="border">The number of border modules to add to each of the four sides.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="scale"/> is 0 or negative, <paramref name="border"/> is negative
        /// or the resulting image is wider than 32,768 pixels.</exception>
        public static void SaveAsPng(this QrCode qrCode, string filename, int scale, int border)
        {
            qrCode.SaveAsPng(filename, scale, border, Color.Black, Color.White);
        }
    }
}
