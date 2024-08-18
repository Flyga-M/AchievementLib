using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchievementLib
{
    /// <summary>
    /// Provides utility functions for <see cref="Texture2D"/>s.
    /// </summary>
    /// <remarks>
    /// copied from https://github.com/blish-hud/Blish-HUD/blob/dev/Blish%20HUD/_Utils/TextureUtil.cs
    /// </remarks>
    public static class TextureUtil
    {
        /// <summary>
        /// Creates a <see cref="Texture2D"/> from a stream. Supports formats bmp, gif, jpg, png, tif, and dds.
        /// The resulting texture has the alpha channel premultiplied to match the MonoGame 3.6
        /// implementation.
        /// </summary>
        /// <remarks>
        /// https://community.monogame.net/t/texture2d-fromstream-in-3-7/10973/9
        /// </remarks>
        /// <returns>
        /// The resulting <see cref="Texture2D"/>, or <see langword="null"/>, if an 
        /// exception occured.
        /// </returns>
		public static Texture2D FromStreamPremultiplied(GraphicsDevice graphics, Stream stream)
        {
            Texture2D texture = null;

            try
            {
                texture = Texture2D.FromStream(graphics, stream);

                Color[] data = new Color[texture.Width * texture.Height];
                texture.GetData(data);

                for (int i = 0; i < data.Length; ++i)
                {
                    byte a = data[i].A;

                    data[i].R = ApplyAlpha(data[i].R, a);
                    data[i].G = ApplyAlpha(data[i].G, a);
                    data[i].B = ApplyAlpha(data[i].B, a);
                }

                texture.SetData(data);
            }
            catch (SharpDX.SharpDXException ex)
            {
                switch (ex.HResult)
                {
                    case -2005270523:
                        // HRESULT: [0x887A0005], Module: [SharpDX.DXGI], ApiCode: [DXGI_ERROR_DEVICE_REMOVED/DeviceRemoved]
                        // The GPU device instance has been suspended. Use GetDeviceRemovedReason to determine the appropriate action.
                        break;
                }
            }
            catch (AccessViolationException)
            {
                // Not sure how this happens.
            }

            return texture;
        }

        private static byte ApplyAlpha(byte color, byte alpha)
        {
            var fc = color / 255.0f;
            var fa = alpha / 255.0f;
            var fr = (int)(255.0f * fc * fa);
            if (fr < 0)
            {
                fr = 0;
            }
            if (fr > 255)
            {
                fr = 255;
            }
            return (byte)fr;
        }
    }
}
