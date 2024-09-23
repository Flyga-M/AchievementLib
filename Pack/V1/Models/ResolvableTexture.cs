using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;

namespace AchievementLib.Pack.V1.Models
{
    /// <inheritdoc cref="IResolvableTexture"/>
    public class ResolvableTexture : LoadableOrResolvableTexture
    {
        /// <inheritdoc/>
        [JsonConstructor]
        public ResolvableTexture(int assetId) : base(string.Empty, assetId)
        {
            /** NOOP **/
        }

        /// <inheritdoc/>
        public override void Resolve(IResolveContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (AssetId <= 0)
            {
                throw new InvalidOperationException($"{nameof(AssetId)} must be greater than 0. " +
                    $"Given Value: {AssetId}");
            }

            if (!context.CanResolve(this))
            {
                throw new InvalidOperationException($"This {nameof(ResolvableTexture)} can not " +
                    $"be resolved by the provided {nameof(context)} ({context.GetType()}).");
            }

            try
            {
                ResolvedTexture = context.Resolve(this) as Texture2D;
            }
            catch (Exception ex)
            {
                throw new PackReferenceException($"Resource for AssetId {AssetId} could not be " +
                    $"loaded as {nameof(Texture2D)}.", AssetId.ToString(), ex);
            }

            if (ResolvedTexture == null)
            {
                throw new PackReferenceException($"Resource for AssetId {AssetId} could not be " +
                    $"loaded as {nameof(Texture2D)}.", AssetId.ToString());
            }

            OnResolved();
        }

        /// <inheritdoc/>
        public override bool TryResolve(IResolveContext context, out PackReferenceException exception)
        {
            exception = null;

            try
            {
                Resolve(context);
            }
            catch (ArgumentNullException ex)
            {
                exception = new PackReferenceException("Resolve context can't be null.", AssetId.ToString(), ex);
                return false;
            }
            catch (InvalidOperationException ex)
            {
                exception = new PackReferenceException($"{nameof(AssetId)} must be above 0 and " +
                    $"provided context must be able to resolve reference.", AssetId.ToString(), ex);
                return false;
            }
            catch (ArgumentException ex)
            {
                exception = new PackReferenceException("Provided context is of wrong type.", AssetId.ToString(), ex);
                return false;
            }
            catch (PackReferenceException ex)
            {
                exception = new PackReferenceException("Texture could not be resolved.", AssetId.ToString(), ex);
                return false;
            }

            return IsResolved;
        }

        /// <inheritdoc/>
        public override bool IsValid()
        {
            return AssetId > 0;
        }

        /// <inheritdoc/>
        /// <exception cref="PackFormatException"></exception>
        public override void Validate()
        {
            if (!IsValid())
            {
                throw new PackFormatException($"ResolvableTexture {this} is invalid.", this.GetType());
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{{ {typeof(ResolvableHierarchyReference)}: {{ " +
                $"\"AssetId\": {AssetId}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
