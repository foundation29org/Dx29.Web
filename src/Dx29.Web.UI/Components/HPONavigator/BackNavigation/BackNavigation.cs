using System;
using System.Collections.Generic;

namespace Dx29.Web.UI.Components
{
    public class BackNavigation<TItem>
    {
        public BackNavigation()
        {
            NavStack = new Stack<TItem>();
        }

        public Stack<TItem> NavStack { get; set; }

        public TItem Current { get; set; }
        public TItem Last => NavStack.Count > 0 ? NavStack.Peek() : default(TItem);

        public void Next(TItem item)
        {
            if (Current != null)
            {
                NavStack.Push(Current);
            }
            Current = item;
        }

        public void Back()
        {
            if (NavStack.Count > 0)
            {
                Current = NavStack.Pop();
            }
            else
            {
                Current = default(TItem);
            }
        }
    }
}
