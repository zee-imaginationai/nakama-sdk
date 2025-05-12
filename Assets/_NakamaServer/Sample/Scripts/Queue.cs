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
        private Queue<Task> _queue = new Queue<Task>();

        public int Count => _queue.Count;

        public void Enqueue(Task task)
        {
            _queue.Enqueue(task);
            if(Count > 1) return;
            QueueChangedEvent.Invoke();
        }

        public Task DeQueue()
        {
            return !_queue.TryDequeue(out var result) ? null : result;
        }
    }
}