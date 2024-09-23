using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchievementLib.Pack
{
    /// <summary>
    /// <inheritdoc cref="IResolveContext"/>
    /// Provides information on asset textures.
    /// </summary>
    public interface ITextureResolveContext : IResolveContext<IResolvableTexture, Texture2D>
    {

    }
}
