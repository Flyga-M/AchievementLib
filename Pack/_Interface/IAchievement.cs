﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AchievementLib.Pack
{
    /// <summary>
    /// An Achievement that can contain multiple <see cref="IObjective">IObjectives</see>.
    /// </summary>
    public interface IAchievement : IHierarchyObject, IValidateable, IFulfillable
    {
        /// <summary>
        /// Fires, when the <see cref="CurrentObjectives"/> changes.
        /// </summary>
        event EventHandler<int> CurrentObjectivesChanged;

        /// <summary>
        /// Fires, when the <see cref="IsWatched"/> property changed.
        /// </summary>
        event EventHandler<bool> IsWatchedChanged;

        /// <summary>
        /// Fires, when the <see cref="IsUnlocked"/> property changed.
        /// </summary>
        event EventHandler<bool> IsUnlockedChanged;

        /// <summary>
        /// The name of the <see cref="IAchievement"/>.
        /// </summary>
        ILocalizable Name { get; }

        /// <summary>
        /// The description for this <see cref="IAchievement"/>.
        /// </summary>
        ILocalizable Description { get; }

        /// <summary>
        /// The description for this <see cref="IAchievement"/> before it is unlocked. 
        /// [Optional] if <see cref="Prerequesites"/> are empty or null.
        /// </summary>
        ILocalizable LockedDescription { get; }

        /// <summary>
        /// The icon that is displayed for 
        /// the <see cref="IAchievement"/>. [Optional]
        /// </summary>
        Texture2D Icon { get; }

        /// <summary>
        /// The <see cref="Color"/> that is used for the achievement display. [Optional]
        /// </summary>
        Color? Color { get; }

        /// <summary>
        /// The <see cref="IAchievement">IAchievements</see> that need to be 
        /// completed, before this <see cref="IAchievement"/> is available. 
        /// [Optional]
        /// </summary>
        IEnumerable<IAchievement> Prerequesites { get; }

        /// <summary>
        /// The <see cref="ITier"/>s in which this <see cref="IAchievement"/> can be completed. 
        /// The values describe how many <see cref="CurrentObjectives"/> are needed to complete 
        /// the tier and how many points are awarded for completion.
        /// </summary>
        IEnumerable<ITier> Tiers { get; }

        /// <summary>
        /// All <see cref="IObjective">IObjectives</see> that are part of this 
        /// <see cref="IAchievement"/>.
        /// </summary>
        IEnumerable<IObjective> Objectives { get; }

        /// <summary>
        /// Determines how the <see cref="Objectives"/> are displayed. [Optional]
        /// </summary>
        ObjectiveDisplay ObjectiveDisplay { get; }

        /// <summary>
        /// Determines whether the <see cref="IAchievement"/> can be repeated 
        /// multiple times. [Optional]
        /// </summary>
        bool IsRepeatable { get; }

        /// <summary>
        /// Determines whether an <see cref="IAchievement"/> is not visible, until its 
        /// <see cref="Prerequesites"/> are completed. [Optional]
        /// </summary>
        bool IsHidden { get; }

        /// <summary>
        /// Determines at which rate the <see cref="IAchievement"/> resets, if at all. 
        /// [Optional]
        /// </summary>
        ResetType ResetType { get; }

        /// <summary>
        /// Will reset the progress for the whole <see cref="IAchievement"/>, if met. 
        /// [Optional]
        /// </summary>
        ICondition ResetCondition { get; }

        /// <summary>
        /// Determines whether the <see cref="IAchievement"/> is pinned to the top, 
        /// when it's <see cref="IAchievementCollection"/> is displayed. This is usually 
        /// <see langword="true"/> for meta-achievements. 
        /// [Optional]
        /// </summary>
        bool IsPinned { get; }

        /// <summary>
        /// Determines whether any of the associated <see cref="IAction"/>s use the 
        /// GW2 API.
        /// </summary>
        [JsonIgnore]
        bool UsesApi {  get; }

        /// <summary>
        /// The current tier, that the <see cref="IAchievement"/> is completing. 1-indexed.
        /// </summary>
        [JsonIgnore]
        int CurrentTier { get; }

        /// <summary>
        /// The current achievement progress.
        /// </summary>
        [JsonIgnore]
        int CurrentObjectives { get; }

        /// <summary>
        /// Determines whether the <see cref="Prerequesites"/> are met, if there are any.
        /// </summary>
        [JsonIgnore]
        bool IsUnlocked { get; }

        /// <summary>
        /// Determines whether the <see cref="IAchievement"/> is currently hidden (<see cref="IsHidden"/> is 
        /// <see langword="true"/> and the <see cref="Prerequesites"/> are not met), or visible.
        /// </summary>
        [JsonIgnore]
        bool IsVisible { get; }

        /// <summary>
        /// The amount of times the <see cref="IAchievement"/> was completed.
        /// </summary>
        [JsonIgnore]
        int RepeatedAmount { get; }

        /// <summary>
        /// The <see cref="DateTime"/> when the <see cref="IAchievement"/> was last completed.
        /// </summary>
        /// <remarks>
        /// Will be <see cref="DateTime.MinValue"/>, if the <see cref="IAchievement"/> has not been completed.
        /// </remarks>
        [JsonIgnore]
        DateTime LastCompletion { get; }

        /// <summary>
        /// Determines whether the <see cref="IAchievement"/> is currently on the watch list.
        /// </summary>
        [JsonIgnore]
        bool IsWatched { get; set; }

        /// <summary>
        /// Resets the current progress of the <see cref="IAchievement"/>. May be used on (daily/weekly/monthly) reset or 
        /// repeatable completion, if 
        /// <see cref="ResetType"/> is something other than <see cref="ResetType.Permanent"/> or <see cref="IsRepeatable"/> is 
        /// <see langword="true"/>.
        /// </summary>
        /// <returns><see langword="true"/>, if the <see cref="ResetType"/> is not <see cref="ResetType.Permanent"/>, or 
        /// <see cref="IsRepeatable"/> is <see langword="true"/>. 
        /// Othwise <see langword="false"/>.</returns>
        bool ResetProgress();
    }
}
