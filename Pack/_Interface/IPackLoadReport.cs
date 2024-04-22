using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchievementLib.Pack
{
    public interface IPackLoadReport
    {
        /// <summary>
        /// Might be null, if the data was not yet loaded.
        /// </summary>
        bool? FaultyData { get; }

        /// <summary>
        /// Might be null, if the data was not yet loaded, or faulty. Does not determine 
        /// <see cref="Success"/>.
        /// </summary>
        bool? FaultyResources { get; }

        /// <summary>
        /// Will still be true, if some resources are faulty.
        /// </summary>
        bool Success { get; }

        AchievementLibException Exception { get; }
    }
}
