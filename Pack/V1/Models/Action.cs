namespace AchievementLib.Pack.V1.Models
{
    public abstract class Action : IValidateable
    {
        public abstract bool IsValid();

        public abstract void Validate();
    }
}
