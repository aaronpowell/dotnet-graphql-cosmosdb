using System;
using HotChocolate.Language;

namespace HotChocolate.AspNetCore
{
    public class AzureFunctionsMiddlewareOptions
        : IAzureFunctionsMiddlewareOptions
    {
        private const int _minMaxRequestSize = 1024;
        private ParserOptions _parserOptions = new ParserOptions();
        private int _maxRequestSize = 20 * 1000 * 1000;

        public ParserOptions ParserOptions
        {
            get => _parserOptions;
            set
            {
                _parserOptions = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public int MaxRequestSize
        {
            get => _maxRequestSize;
            set
            {
                if (value < _minMaxRequestSize)
                {
                    // TODO : resources
                    throw new ArgumentException(
                        "The minimum max request size is 1024 byte.",
                        nameof(value));
                }

                _maxRequestSize = value;
            }
        }
    }
}