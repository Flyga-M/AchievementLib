using AchievementLib.Pack.JSON;
using AchievementLib.Pack.V1.Models;
using System;
using System.Collections.Generic;

namespace AchievementLib.Pack.V1.JSON
{
    public class ActionConverter : BasicConverter<Models.Action>
    {   
        internal static ActionConverter Default = new ActionConverter();

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
                { "lookingAt", typeof(LookingAtAction) }
            };
        }
    }
}
