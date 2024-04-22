using Microsoft.Xna.Framework;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Checks whether a <see cref="Achievement"/> is completed.
    /// </summary>
    public class AchievementAction : Action
    {
        /// <summary>
        /// The full name (id) of the <see cref="Achievement"/> in the hierarchy tree.
        /// </summary>
        public string FullName { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(FullName);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="PackFormatException"></exception>
        public override void Validate()
        {
            if (!IsValid())
            {
                throw new PackFormatException($"AchievementAction {this} is invalid.", this.GetType());
            }
        }

        public override string ToString()
        {
            return $"{{ {typeof(AchievementAction)}: {{ " +
                $"\"FullName\": {FullName}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
