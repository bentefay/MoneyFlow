using System;
using LanguageExt;
using Web.Types.Errors;

namespace Web.Types.Values
{
    public class UserId : TinyType<UserId, Guid>
    {
        public static Either<MalformedUserId, UserId> Create(string value) => 
            !string.IsNullOrWhiteSpace(value) && Guid.TryParse(value, out var guid) ?
                Prelude.Right<UserId>(new UserId(guid)) :
                Prelude.Left<MalformedUserId, UserId>(new MalformedUserId(value));

        public static UserId Create()
        {
            return new UserId(Guid.NewGuid());
        }

        private UserId(Guid value) : base(value)
        {
        }
    }
}