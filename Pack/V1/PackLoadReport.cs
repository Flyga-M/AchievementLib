namespace AchievementLib.Pack.V1
{
    public class PackLoadReport : IPackLoadReport
    {
        /// <summary>
        /// Might be null, if the data was not yet loaded.
        /// </summary>
        public bool? FaultyData { get; internal set; }

        /// <summary>
        /// Might be null, if the data was not yet loaded, or faulty. Does not determine 
        /// <see cref="Success"/>.
        /// </summary>
        public bool? FaultyResources { get; internal set; }

        /// <summary>
        /// Will still be true, if some resources are faulty.
        /// </summary>
        public bool Success => FaultyData.HasValue && !FaultyData.Value;

        public AchievementLibException Exception { get; internal set; }

        public override string ToString()
        {
            return $"FaultyData?: {FaultyData}, FaultyResources?: {FaultyResources}, " +
                $"Success: {Success}, Exception: {Exception}";
        }
    }
}
