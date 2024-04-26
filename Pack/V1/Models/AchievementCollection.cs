using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// A collection of multiple achievements.
    /// </summary>
    public class AchievementCollection : IHierarchyObject, ILoadable
    {
        /// <inheritdoc/>
        public string Id { get; set; }

        /// <summary>
        /// The name of the <see cref="AchievementCollection"/>.
        /// </summary>
        public Localizable Name { get; set; }

        /// <summary>
        /// The <see cref="Achievement">Achievements</see> in the 
        /// <see cref="AchievementCollection"/>.
        /// </summary>
        public IEnumerable<Achievement> Achievements { get; set; }

        /// <summary>
        /// The <see cref="LoadableTexture"/> to the icon that is displayed for 
        /// the <see cref="AchievementCollection"/>. [Optional]
        /// </summary>
        public LoadableTexture Icon { get; set; }

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
        public async Task LoadAsync(AchievementPackResourceManager resourceManager, GraphicsDevice graphicsDevice)
        {
            await Icon?.LoadAsync(resourceManager, graphicsDevice);
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
        public async Task<(bool, PackResourceException)> TryLoadAsync(AchievementPackResourceManager resourceManager, GraphicsDevice graphicsDevice)
        {
            if (Icon == null)
            {
                return (true, null);
            }
            return await Icon.TryLoadAsync(resourceManager, graphicsDevice);
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
