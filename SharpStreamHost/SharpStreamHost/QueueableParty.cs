using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpStreamHost {
    public class QueueableParty : PartyHost {
        public Queue<ISampleProvider> _Queue { get; private set; } = new Queue<ISampleProvider>();

        public QueueableParty() {
            OnEmpty += WhenEmpty;
        }

        public override void AddSource(ISampleProvider source) {
            _Queue.Enqueue(source);
        }
        public override void AddSource(string path) {
            ISampleProvider ret = new AudioFileReader(path);
            _Queue.Enqueue(ret);
        }
        public void AddSource(string[] paths) {
            foreach(string path in paths) {
                AddSource(path);
            }
        }

        private void WhenEmpty() {
            if (_Queue.Count > 0) {
                Source = _Queue.Dequeue();
            }
        }
    }
}
