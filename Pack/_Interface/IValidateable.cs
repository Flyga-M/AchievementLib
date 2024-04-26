namespace AchievementLib.Pack
{
    /// <summary>
    /// Represents a class that can be validated.
    /// </summary>
    public interface IValidateable
    {
        /// <summary>
        /// Checks whether the <see cref="IValidateable"/> is valid.
        /// </summary>
        /// <returns>True, if the <see cref="IValidateable"/> is valid. 
        /// Otherwise false.</returns>
        bool IsValid();

        /// <summary>
        /// Validates the <see cref="IValidateable"/>. Might throw exceptions, if the 
        /// <see cref="IValidateable"/> is not valid.
        /// </summary>
        void Validate();
    }
}
