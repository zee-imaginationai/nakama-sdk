using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectCore.Events;
using UnityEngine;

namespace ProjectCore.Integrations.Internal
{
    [CreateAssetMenu(fileName = "Queue", menuName = "ProjectCore/Integrations/Queue")]
    public class Queue : ScriptableObject
    {
        [SerializeField] private GameEvent QueueChangedEvent; 
        private readonly object _lock = new object();
        private Queue<Task> _queue = new Queue<Task>();
        private const int MaxQueueSize = 5;
        
        public void Enqueue(Task task)
        {
            bool triggerEvent = false;
        
            lock (_lock)
            {
                // Trim oldest entries to keep only last 5 requests
                while (_queue.Count >= MaxQueueSize)
                {
                    var oldTask = _queue.Dequeue();
                    oldTask.Dispose(); // Clean up abandoned tasks
                }

                bool wasEmptyBeforeAdd = _queue.Count == 0;
                _queue.Enqueue(task);
                triggerEvent = wasEmptyBeforeAdd;
            }

            // Execute event on main thread if queue was empty
            if (triggerEvent)
            {
                QueueChangedEvent?.Invoke();
            }
        }


        public bool TryDequeue(out Task task)
        {
            lock (_lock)
            {
                if (_queue.Count == 0)
                {
                    task = null;
                    return false;
                }
                task = _queue.Dequeue();
                return true;
            }
        }
    }
}