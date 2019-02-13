﻿using System.Text.RegularExpressions;
using LanguageExt;
using Web.Types.Errors;

namespace Web.Types
{
    public struct Email : ITinyType<string>
    {
        private static readonly Regex _email = new Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]{2,}$");
        
        public static Either<InvalidEmail, Email> Create(string value) => 
            !string.IsNullOrWhiteSpace(value) && _email.IsMatch(value) ?
                Prelude.Right(new Email(value)) :
                Prelude.Left<InvalidEmail, Email>(new InvalidEmail(value));

        private Email(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}