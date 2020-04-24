namespace FnTools.Types.Interfaces
{
    public interface IToResult<TOk, TError>
    {
        Result<TOk, TError> ToResult();
    }
}