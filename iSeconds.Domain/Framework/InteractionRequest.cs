using System;
using System.Collections.Generic;
using System.Text;

namespace iSeconds.Domain.Framework
{
    public interface IInteractionRequest<T>
    {
        event EventHandler<GenericEventArgs<T>> Raised;
    }

    public class InteractionRequest<T> : IInteractionRequest<T>
    {
        public event EventHandler<GenericEventArgs<T>> Raised;

        public void Raise(T context)
        {
            var handler = this.Raised;
            if (handler != null)
            {
                handler(this, new GenericEventArgs<T>(context));
            }
        }
    }
}
