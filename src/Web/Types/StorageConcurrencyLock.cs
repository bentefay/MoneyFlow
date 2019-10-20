using LanguageExt;
using Web.Types.Errors;

namespace Web.Types
{
    public class StorageConcurrencyLock : TinyType<StorageConcurrencyLock, string>
    {
        public static Either<InvalidHashedPassword, StorageConcurrencyLock> Create(string value) => 
            !string.IsNullOrWhiteSpace(value) ?
                Prelude.Right<StorageConcurrencyLock>(new StorageConcurrencyLock(value)) :
                Prelude.Left<InvalidHashedPassword, StorageConcurrencyLock>(new InvalidHashedPassword(value));

        public StorageConcurrencyLock(string value) : base(value)
        {
        }
    }
}