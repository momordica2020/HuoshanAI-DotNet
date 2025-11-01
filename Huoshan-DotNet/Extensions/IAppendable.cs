// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace HuoshanAI.Extensions
{
    internal interface IAppendable<in T> : IIndexable
    {
        void AppendFrom(T other);
    }
}
