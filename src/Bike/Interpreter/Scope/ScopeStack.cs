namespace Bike.Interpreter
{
    using System;
    using System.Collections.Generic;
    using Builtin;

    public class ScopeStack
    {
        private readonly object syncLock = new object();
        private readonly Stack<ScopeFrame> stack = new Stack<ScopeFrame>();
        public readonly ScopeFrame GlobalFrame;

        public ScopeStack(ScopeFrame globalFrame)
        {
            GlobalFrame = globalFrame;
            stack.Push(globalFrame); 
        }

        public ScopeStack(ScopeStack otherStack)
        {
            GlobalFrame = otherStack.GlobalFrame;
            lock (otherStack.syncLock)
            {
                stack = new Stack<ScopeFrame>(otherStack.stack);
            }
        }

        private void Push(ScopeFrame frame)
        {
            lock (syncLock)
            {
                stack.Push(frame);
            }
        }

        public ScopeFrame CurrentFrame
        {
            get
            {
                lock (syncLock)
                {
                    return stack.Peek();
                }
            }
        }

        private void Pop()
        {
            lock (syncLock)
            {
                stack.Pop();
            }
        }

        public T OpenScopeFor<T>(Func<T> body, 
            bool when = true,
            Action<ScopeFrame> withInit = null, 
            ScopeFrame parentScope = null,
            BikeFunction func = null)
        {
            if (when)
            {
                var scopeFrame = new ScopeFrame(func, GlobalFrame, parentScope ?? CurrentFrame, CurrentFrame);
                if (withInit != null)
                    withInit(scopeFrame);
                Push(scopeFrame);
            }
            try
            {
                return body();
            }
            finally
            {
                if (when)
                    Pop();
            }
        }

        public void OpenScopeFor(Action body, 
            bool when = true,
            Action<ScopeFrame> withInit = null,
            ScopeFrame parentScope = null,
            BikeFunction func = null)
        {
            if (when)
            {
                var scopeFrame = new ScopeFrame(func, GlobalFrame, parentScope ?? CurrentFrame, CurrentFrame);
                if (withInit != null)
                    withInit(scopeFrame);
                Push(scopeFrame);
            }
            try
            {
                body();
            }
            finally
            {
                if (when)
                    Pop();
            }
        }
    }
}
