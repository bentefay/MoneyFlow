
using System;

namespace Web.Types.Errors
{
    public interface IError
    {
        string GetDescription();
    }

    public interface IErrorWithException : IError
    {
        Exception Exception { get; }
    }

    public interface IUpdateVaultIndexErrors : IUpdateVaultErrors {}

    public interface IGetBlobErrors : IUpdateVaultIndexErrors, ILoadVaultIndexErrors, ISaveNewVaultIndexErrors, ILoadVaultErrors {}
        
    public interface IGetBlobTextErrors : ILoadVaultIndexErrors, ILoadVaultErrors {}
        
    public interface ISetBlobTextErrors : IUpdateVaultIndexErrors, ISaveNewVaultIndexErrors {}
    
    public interface ISerializeVaultIndexErrors : IUpdateVaultIndexErrors, ISaveNewVaultIndexErrors {}
    
    public interface IDeserializeVaultIndexErrors : ILoadVaultIndexErrors {}
    
    public interface IUpdateVaultRequestToVaultErrors : IUpdateVaultErrors {}
    
    public interface ISerializeVaultErrors : IUpdateVaultIndexErrors {}
    
    public interface IDeserializeVaultErrors : ILoadVaultErrors {}
    
    public interface ILoadVaultIndexErrors : IGetVaultErrors, IUpdateVaultErrors {}
    
    public interface ILoadVaultErrors : IGetVaultErrors {}
    
    public interface ISaveNewVaultIndexErrors : ICreateVaultErrors {}
    
    public interface IGetVaultErrors : IError {}
    
    public interface ICreateVaultErrors : IError {}
    
    public interface IUpdateVaultErrors : IError {}
    
    public interface IAssertVaultAccessErrors : IGetVaultErrors, IUpdateVaultErrors {}
    
    public interface IParseAuthorizationErrors : IGetVaultErrors, ICreateVaultErrors, IUpdateVaultErrors {}
    
    public interface ICreateVaultIndexErrors : ICreateVaultErrors {}
}