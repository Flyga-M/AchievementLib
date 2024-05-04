using AchievementLib.Pack.V1.Models;
using PositionEvents.Area.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchievementLib.Pack.V1.JSON
{
    /// <summary>
    /// A converter to (de-)serialize the base V1 Restraints in this library.
    /// </summary>
    public class RestraintConverter : BasicConverter<Models.Restraint>
    {
        /// <summary>
        /// The default <see cref="RestraintConverter"/> that only containes all the V1 Restraints 
        /// that are part of this library.
        /// </summary>
        public static ReadOnlyBasicConverter<Models.Restraint> Default = new ReadOnlyBasicConverter<Models.Restraint>(new RestraintConverter());

        /// <inheritdoc/>
        public RestraintConverter() : base(GetSubTypes())
        {
            // NOOP
        }

        private static Dictionary<string, Type> GetSubTypes()
        {
            return new Dictionary<string, Type>()
            {
                { "base", typeof(RestraintBaseLevel) },
                { "sub", typeof(RestraintSubLevel) }
            };
        }
    }
}
