
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

    public interface IUpdateUserErrors : IUpdateVaultErrors {}

    public interface IGetBlobErrors : IUpdateUserErrors, ILoadUserErrors, ISaveNewUserErrors, ILoadVaultErrors {}
        
    public interface IGetBlobTextErrors : ILoadUserErrors, ILoadVaultErrors {}
        
    public interface ISetBlobTextErrors : IUpdateUserErrors, ISaveNewUserErrors {}
    
    public interface ISerializeUserErrors : IUpdateUserErrors, ISaveNewUserErrors {}
    
    public interface IDeserializeUserErrors : ILoadUserErrors {}
    
    public interface IUpdateVaultRequestToVaultErrors : IUpdateVaultErrors {}
    
    public interface ISerializeVaultErrors : IUpdateUserErrors {}
    
    public interface IDeserializeVaultErrors : ILoadVaultErrors {}
    
    public interface ILoadUserErrors : IGetVaultErrors, IUpdateVaultErrors {}
    
    public interface ILoadVaultErrors : IGetVaultErrors {}
    
    public interface ISaveNewUserErrors : ICreateUserErrors {}
    
    public interface IGetVaultErrors : IError {}
    
    public interface ICreateUserErrors : IError {}
    
    public interface IUpdateVaultErrors : IError {}
    
    public interface IAssertVaultAccessErrors : IGetVaultErrors, IUpdateVaultErrors {}
    
    public interface IParseAuthorizationErrors : IGetVaultErrors, ICreateUserErrors, IUpdateVaultErrors {}
    
    public interface IBuilderUserErrors : ICreateUserErrors {}
}