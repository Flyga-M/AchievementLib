using AchievementLib.Pack.V1.Models;
using System;
using System.Collections.Generic;
using PositionEvents.Area.JSON;

namespace AchievementLib.Pack.V1.JSON
{
    public class ActionConverter : BasicConverter<Models.Action>
    {   
        public static ReadOnlyBasicConverter<Models.Action> Default = new ReadOnlyBasicConverter<Models.Action>(new ActionConverter());

        public ActionConverter() : base(GetSubTypes())
        {
            /** NOOP **/
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
