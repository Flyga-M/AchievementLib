using AchievementLib.Pack.JSON;
using AchievementLib.Pack.V1.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace AchievementLib.Pack.V1.JSON
{
    public class ActionConverter : BasicConverter<Models.Action>
    {   
        internal static ActionConverter Default = new ActionConverter();
        
        //private static readonly Dictionary<string, Type> _subTypes = 

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
