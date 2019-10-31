
namespace Web.Types.Errors
{
    public interface IError
    {
        string GetDescription();
    }
    
    public interface IUpdateVaultIndexErrors {}

    public interface IGetBlobErrors : IUpdateVaultIndexErrors, ILoadVaultIndexErrors, ISaveNewVaultIndexErrors {}
        
    public interface IGetBlobTextErrors : ILoadVaultIndexErrors {}
        
    public interface ISetBlobTextErrors : IUpdateVaultIndexErrors, ISaveNewVaultIndexErrors {}
    
    public interface ISerializeVaultIndexErrors : IUpdateVaultIndexErrors, ISaveNewVaultIndexErrors {}
    
    public interface IDeserializeVaultIndexErrors : ILoadVaultIndexErrors {}
    
    public interface ILoadVaultIndexErrors : IGetVaultErrors {}
    
    public interface ISaveNewVaultIndexErrors {}
    
    public interface IGetVaultErrors {}
    
    public interface IAssertVaultAccessErrors : IGetVaultErrors {}
    
    public interface IParseAuthorizationErrors : IGetVaultErrors {}
}