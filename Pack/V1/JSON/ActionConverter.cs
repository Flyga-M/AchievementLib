using AchievementLib.Pack.V1.Models;
using System;
using System.Collections.Generic;
using PositionEvents.Area.JSON;

namespace AchievementLib.Pack.V1.JSON
{
    /// <summary>
    /// A converter to (de-)serialize the base V1 Actions in this library.
    /// </summary>
    public class ActionConverter : BasicConverter<Models.Action>
    {   
        /// <summary>
        /// The default <see cref="ActionConverter"/> that only containes all the V1 Actions 
        /// that are part of this library.
        /// </summary>
        public static ReadOnlyBasicConverter<Models.Action> Default = new ReadOnlyBasicConverter<Models.Action>(new ActionConverter());

        /// <inheritdoc/>
        public ActionConverter() : base(GetSubTypes())
        {
            // NOOP
        }

        private static Dictionary<string, Type> GetSubTypes()
        {
            return new Dictionary<string, Type>()
            {
                { "achievement", typeof(AchievementAction) },
                { "apiContains", typeof(ApiActionContains) },
                { "apiCopy", typeof(ApiActionCopy) },
                { "apiCount", typeof(ApiActionCount) },
                { "apiCountComparison", typeof(ApiActionCountComparison) },
                { "apiComparison", typeof(ApiActionComparison) },
                { "position", typeof(PositionAction) },
                { "positionArea", typeof(PositionAreaAction)},
                { "lookingAt", typeof(LookingAtAction) }
            };
        }
    }
}
