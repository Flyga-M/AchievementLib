namespace AchievementLib.Pack.V1.Models
{
    public class Condition
    {
        /// <summary>
        /// If not null, an alternative <see cref="Condition"/> that may be satisfied 
        /// instead of this <see cref="Condition"/> to be true. Functions as an 
        /// OR-condition. [Optional]
        /// </summary>
        public Condition OrCondition { get; set; }

        /// <summary>
        /// If not null, an additional <see cref="Condition"/> that must be satisfied 
        /// with this <see cref="Condition"/> to be true. Functions as an 
        /// AND-condition. [Optional]
        /// </summary>
        public Condition AndCondition { get; set; }

        /// <summary>
        /// The <see cref="Action"/> carrying the data associated with the 
        /// <see cref="Condition"/>.
        /// </summary>
        public Action Action { get; set; }

        public bool IsValid()
        {
            return Action != null
                && Action.IsValid()
                && (OrCondition == null || OrCondition.IsValid())
                && (AndCondition == null || AndCondition.IsValid());
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="PackFormatException"></exception>
        public void Validate()
        {
            if (!IsValid())
            {
                try
                {
                    Action?.Validate();
                    OrCondition?.Validate();
                    AndCondition?.Validate();
                }
                catch (PackFormatException ex)
                {
                    throw new PackFormatException($"Condition {this} is invalid.", this.GetType(), ex);
                }

                throw new PackFormatException($"Condition {this} is invalid.", this.GetType());
            }
        }

        public override string ToString()
        {
            return $"{{ {typeof(Condition)}: {{ " +
                $"\"OrCondition\": {OrCondition}, " +
                $"\"AndCondition\": {AndCondition}, " +
                $"\"Action\": {Action}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
