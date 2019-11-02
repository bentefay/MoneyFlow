using System;
using LanguageExt;
using Web.Types.Errors;

namespace Web.Types.Values
{
    public class UserId : TinyType<UserId, Guid>
    {
        public static Either<InvalidUserId, UserId> Create(string value) => 
            !string.IsNullOrWhiteSpace(value) && Guid.TryParse(value, out var guid) ?
                Prelude.Right<UserId>(new UserId(guid)) :
                Prelude.Left<InvalidUserId, UserId>(new InvalidUserId(value));

        public static UserId Create()
        {
            return new UserId(Guid.NewGuid());
        }

        private UserId(Guid value) : base(value)
        {
        }
    }
}