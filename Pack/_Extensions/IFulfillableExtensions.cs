namespace AchievementLib.Pack
{
    /// <summary>
    /// Provides extensions to the <see cref="IFulfillable"/> interface.
    /// </summary>
    public static class IFulfillableExtensions
    {
        /// <summary>
        /// Sets <see cref="IFulfillable.FreezeUpdates"/> to true. Useful, if you're not sure if 
        /// the <see cref="IFulfillable"/> is null.
        /// <code>IFulfillable fulfillable?.FreezeUpdates()</code>
        /// </summary>
        /// <param name="fulfillable"></param>
        public static void FreezeUpdates(this IFulfillable fulfillable)
        {
            fulfillable.FreezeUpdates = true;
        }

        /// <summary>
        /// Sets <see cref="IFulfillable.FreezeUpdates"/> to false. Useful, if you're not sure if 
        /// the <see cref="IFulfillable"/> is null.
        /// <code>IFulfillable fulfillable?.UnfreezeUpdates()</code>
        /// </summary>
        /// <param name="fulfillable"></param>
        public static void UnfreezeUpdates(this IFulfillable fulfillable)
        {
            fulfillable.FreezeUpdates = false;
        }
    }
}
