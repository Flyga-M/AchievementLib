using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// <inheritdoc cref="IAchievementCollection"/>
    /// This is the V1 implementation.
    /// </summary>
    public class AchievementCollection : IAchievementCollection, ILoadable
    {
        /// <inheritdoc/>
        public string Id { get; set; }

        /// <inheritdoc cref="IAchievementCollection.Name"/>
        public Localizable Name { get; set; }

        /// <inheritdoc/>
        ILocalizable IAchievementCollection.Name => Name;

        /// <inheritdoc cref="IAchievementCollection.Achievements"/>
        public IEnumerable<Achievement> Achievements { get; set; }

        /// <inheritdoc/>
        IEnumerable<IAchievement> IAchievementCollection.Achievements => Achievements;

        /// <inheritdoc cref="IAchievementCollection.Icon"/>
        public LoadableTexture Icon { get; set; }

        /// <inheritdoc/>
        Texture2D IAchievementCollection.Icon => Icon.LoadedTexture;

        /// <inheritdoc/>
        [JsonIgnore]
        public bool IsLoaded => Icon?.IsLoaded ?? true;

        /// <inheritdoc/>
        [JsonIgnore]
        public IHierarchyObject Parent { get; set; } = null;

        /// <inheritdoc/>
        [JsonIgnore]
        public IHierarchyObject[] Children => Achievements.ToArray();

        /// <inheritdoc/>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Id)
                && Name != null
                && Name.IsValid()
                && Achievements != null
                && Achievements.Any()
                && Achievements.All(achievement => achievement.IsValid())
                && (Icon == null || Icon.IsValid());
        }

        /// <inheritdoc/>
        /// <exception cref="PackFormatException"></exception>
        public void Validate()
        {
            if (!IsValid())
            {
                try
                {
                    Name?.Validate();
                    if (Achievements != null)
                    {
                        foreach (Achievement achievement in Achievements)
                        {
                            achievement.Validate();
                        }
                    }
                    Icon?.Validate();
                }
                catch (PackFormatException ex)
                {
                    throw new PackFormatException($"AchievementCollection {this.GetFullName()} is invalid.", this.GetType(), ex);
                }

                throw new PackFormatException($"AchievementCollection {this.GetFullName()} is invalid.", this.GetType());
            }
        }

        /// <inheritdoc/>
        public void Load(AchievementPackResourceManager resourceManager, GraphicsDevice graphicsDevice)
        {
            Icon?.Load(resourceManager, graphicsDevice);
        }

        /// <inheritdoc/>
        public async Task LoadAsync(AchievementPackResourceManager resourceManager, GraphicsDevice graphicsDevice, CancellationToken cancellationToken)
        {
            await Icon?.LoadAsync(resourceManager, graphicsDevice, cancellationToken);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/> Also true, if the optional resource is null and does 
        /// not need to be loaded.</returns>
        public bool TryLoad(AchievementPackResourceManager resourceManager, GraphicsDevice graphicsDevice, out PackResourceException exception)
        {
            exception = null;
            if (Icon == null)
            {
                return true;
            }

            return Icon.TryLoad(resourceManager, graphicsDevice, out exception);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/> Also true, if the optional resource is null and does 
        /// not need to be loaded.</returns>
        /// <exception cref="OperationCanceledException"></exception>
        public async Task<(bool, PackResourceException)> TryLoadAsync(AchievementPackResourceManager resourceManager, GraphicsDevice graphicsDevice, CancellationToken cancellationToken)
        {
            if (Icon == null)
            {
                return (true, null);
            }
            return await Icon.TryLoadAsync(resourceManager, graphicsDevice, cancellationToken);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Icon?.Dispose();

            this.DisposeChildren();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{{ {typeof(AchievementCollection)}: {{ " +
                $"\"Id\": {Id}, " +
                $"\"Name\": {Name}, " +
                $"\"Achievements\": {{ {(Achievements == null ? "" : string.Join(", ", Achievements))} }}, " +
                $"\"Icon\": {Icon}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
