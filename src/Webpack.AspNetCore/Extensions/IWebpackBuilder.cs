namespace Microsoft.Extensions.DependencyInjection
{
    public interface IWebpackBuilder
    {
        IServiceCollection Services { get; }
    }
}
