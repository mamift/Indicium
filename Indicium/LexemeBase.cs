﻿using System;

namespace Indicium
{
    public abstract class LexemeBase<TTokenBase> 
        where TTokenBase: TokenBase, new()
    {
        public abstract TTokenBase Token { get; }

        protected string value;

        public string Value => value;

        protected LexemeBase(string value)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));

            this.value = value;
        }
    }
}