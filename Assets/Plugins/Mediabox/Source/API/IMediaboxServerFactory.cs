namespace Mediabox.API {
    public interface IMediaboxServerFactory {
        int Priority { get; }
        IMediaboxServer Create();
    }
}