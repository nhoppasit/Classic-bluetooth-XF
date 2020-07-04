using System;
using System.Threading.Tasks;

namespace PaperX_SCG_forms.Views.CustomControls
{
    public class AsyncEvent<T>
    {
        /// <summary>
        /// Call this method to trigger the event. 
        /// </summary>        
        public static async Task OnEvent(
            Func<object, T, Task> handler,
            object sender,
            T eventData)
        {
            if (handler == null)
            {
                return;
            }

            Delegate[] invocationList = handler.GetInvocationList();
            Task[] handlerTasks = new Task[invocationList.Length];

            for (int i = 0; i < invocationList.Length; i++)
            {
                handlerTasks[i] = ((Func<object, T, Task>)invocationList[i])(sender, eventData);
            }

            await Task.WhenAll(handlerTasks);
        }
    }
}
