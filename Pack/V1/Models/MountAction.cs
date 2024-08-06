using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Checks whether the player is on a certain mount.
    /// </summary>
    public class MountAction : Action
    {
        /// <summary>
        /// The mount type that is checked. For reference take a look at 
        /// https://wiki.guildwars2.com/wiki/API:MumbleLink or 
        /// https://api.guildwars2.com/v2/mounts/types (does not contain the skiff)
        /// </summary>
        public MountType MountType { get; set; }

        /// <summary>
        /// Instantiates a <see cref="MountAction"/>.
        /// </summary>
        /// <param name="mountType"></param>
        [JsonConstructor]
        public MountAction(MountType mountType)
        {
            MountType = mountType;
        }

        /// <inheritdoc/>
        public override bool IsValid()
        {
            return true;
        }

        /// <inheritdoc/>
        /// <exception cref="PackFormatException"></exception>
        public override void Validate()
        {
            if (!IsValid())
            {
                throw new PackFormatException($"MapAction {this} is invalid.", this.GetType());
            }
        }

        /// <inheritdoc/>
        protected override Dictionary<string, object> InnerToString()
        {
            Dictionary<string, object> inner = base.InnerToString();

            inner.Add($"{nameof(MountType)}", MountType);

            return inner;
        }
    }
}
