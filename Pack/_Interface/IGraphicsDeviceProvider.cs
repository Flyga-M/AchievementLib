using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace AchievementLib.Pack
{
    /// <summary>
    /// Provides access to a <see cref="GraphicsDevice"/>.
    /// </summary>
    public interface IGraphicsDeviceProvider
    {
        /// <summary>
        /// Lends the <see cref="GraphicsDevice"/> to the provided <paramref name="action"/>. 
        /// Only use with snychronous <see cref="Action"/>s.
        /// </summary>
        /// <param name="action"></param>
        void LendGraphicsDevice(Action<GraphicsDevice> action);
    }
}
