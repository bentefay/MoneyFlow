using LanguageExt;

namespace Web.Utils.Extensions
{
    public static class PreludeExt
    {
        public static Either<TL2, TR> Left<TL1, TL2, TR>(this Either<TL1, TR> @this, Cast<TL2> to) where TL1 : TL2
        {
            return @this.MapLeft(left => (TL2)left);
        }
    }

    public static class Cast
    {
        public static Cast<T> To<T>() => new Cast<T>();
    }
    
    public struct Cast<T>
    {
    }
}