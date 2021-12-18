namespace DevHopTools.MVVM.ViewModels
{
    public abstract class EntityViewModel<TEntity> : ViewModelBase
    {
        protected TEntity Entity { get; private set; }

        public EntityViewModel(TEntity entity)
        {
            Entity = entity;
        }
    }
}
