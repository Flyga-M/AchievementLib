namespace AchievementLib.Pack.V1
{
    /// <summary>
    /// <inheritdoc cref="IPackLoadReport"/>
    /// This is the V1 implementation.
    /// </summary>
    public class PackLoadReport : IPackLoadReport
    {
        /// <inheritdoc/>
        public bool? FaultyData { get; internal set; }

        /// <inheritdoc/>
        public bool? FaultyResources { get; internal set; }

        /// <inheritdoc/>
        public bool Success => FaultyData.HasValue && !FaultyData.Value;

        /// <inheritdoc/>
        public AchievementLibException Exception { get; internal set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"FaultyData?: {FaultyData}, FaultyResources?: {FaultyResources}, " +
                $"Success: {Success}, Exception: {Exception}";
        }
    }
}
