namespace ReactiveUI.Uno.SQLiteStudio;

public sealed class AppLocatorViewLocator : IViewLocator
{
    public IViewFor? ResolveView<T>(T? viewModel, string? contract = null)
    {
        if (viewModel is null)
        {
            return null;
        }

        return AppLocator.Current.GetService(typeof(IViewFor<>).MakeGenericType(viewModel.GetType())) as IViewFor;
    }
}
