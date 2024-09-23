using AchievementLib.Pack.V1.JSON;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// <inheritdoc cref="IAchievementCollection"/>
    /// This is the V1 implementation.
    /// </summary>
    public class AchievementCollection : IAchievementCollection, ILoadable, IResolvable
    {
        /// <inheritdoc/>
        public string Id { get; }

        /// <inheritdoc cref="IAchievementCollection.Name"/>
        public Localizable Name { get; }

        /// <inheritdoc/>
        [JsonIgnore]
        ILocalizable IAchievementCollection.Name => Name;

        /// <inheritdoc cref="IAchievementCollection.Achievements"/>
        public List<Achievement> Achievements { get; }

        /// <inheritdoc/>
        [JsonIgnore]
        IEnumerable<IAchievement> IAchievementCollection.Achievements => Achievements;

        /// <inheritdoc cref="IAchievementCollection.Icon"/>
        public LoadableOrResolvableTexture Icon { get; internal set; }

        /// <inheritdoc cref="IAchievementCollection.Color"/>
        [JsonConverter(typeof(ColorConverter))]
        public Color? Color { get; internal set; }

        /// <inheritdoc/>
        [JsonIgnore]
        Texture2D IAchievementCollection.Icon => Icon?.Texture;

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
        public bool IsResolved => Icon?.IsResolved ?? true;

        /// <summary>
        /// Instantiates an <see cref="AchievementCollection"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="achievements"></param>
        /// <param name="icon"></param>
        /// <param name="color"></param>
        [JsonConstructor]
        public AchievementCollection(string id, Localizable name, IEnumerable<Achievement> achievements, LoadableOrResolvableTexture icon, Color? color)
        {
            Id = id;
            Name = name;
            Achievements = new List<Achievement>();
            Icon = icon;
            Color = color;

            if (achievements != null)
            {
                // TODO: should throw if this returns false
                TryAddAchievements(achievements);
            }
        }

        /// <inheritdoc/>
        public event EventHandler Resolved;

        private bool TryAddAchievement(Achievement achievement)
        {
            if (achievement == null)
            {
                return false;
            }

            if (Achievements.Contains(achievement))
            {
                return false;
            }

            achievement.Parent = this;

            Achievements.Add(achievement);
            return true;
        }

        private bool TryAddAchievements(IEnumerable<Achievement> achievements)
        {
            if (achievements == null)
            {
                return false;
            }

            return achievements.All(achievement => TryAddAchievement(achievement));
        }

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
        public void Load(AchievementPackResourceManager resourceManager, IGraphicsDeviceProvider graphicsDeviceProvider)
        {
            Icon?.Load(resourceManager, graphicsDeviceProvider);
        }

        /// <inheritdoc/>
        public async Task LoadAsync(AchievementPackResourceManager resourceManager, IGraphicsDeviceProvider graphicsDeviceProvider, CancellationToken cancellationToken)
        {
            await Icon?.LoadAsync(resourceManager, graphicsDeviceProvider, cancellationToken);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/> Also true, if the optional resource is null and does 
        /// not need to be loaded.</returns>
        public bool TryLoad(AchievementPackResourceManager resourceManager, IGraphicsDeviceProvider graphicsDeviceProvider, out PackResourceException exception)
        {
            exception = null;
            if (Icon == null)
            {
                return true;
            }

            return Icon.TryLoad(resourceManager, graphicsDeviceProvider, out exception);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/> Also true, if the optional resource is null and does 
        /// not need to be loaded.</returns>
        /// <exception cref="OperationCanceledException"></exception>
        public async Task<(bool, PackResourceException)> TryLoadAsync(AchievementPackResourceManager resourceManager, IGraphicsDeviceProvider graphicsDeviceProvider, CancellationToken cancellationToken)
        {
            if (Icon == null)
            {
                return (true, null);
            }
            return await Icon.TryLoadAsync(resourceManager, graphicsDeviceProvider, cancellationToken);
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

        /// <inheritdoc/>
        public bool TryAddChild(IHierarchyObject child)
        {
            if (!(child is Achievement achievement))
            {
                return false;
            }

            return TryAddAchievement(achievement);
        }

        /// <inheritdoc/>
        public void Resolve(IResolveContext context)
        {
            Icon?.Resolve(context);

            Resolved?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc/>
        public bool TryResolve(IResolveContext context, out PackReferenceException exception)
        {
            exception = null;

            if (Icon != null)
            {
                if (!Icon.TryResolve(context, out exception))
                {
                    return false;
                }
            }

            Resolved?.Invoke(this, EventArgs.Empty);
            return true;
        }
    }
}
