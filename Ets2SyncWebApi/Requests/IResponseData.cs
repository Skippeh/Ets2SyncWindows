namespace Ets2SyncWebApi.Requests
{
    public interface IResponseData<out T>
    {
        T ToRealType();
    }
}