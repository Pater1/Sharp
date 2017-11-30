using NAudio.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAudio.Wave {
    /// <summary>
    /// Provides a buffered store of samples
    /// Read method will return queued samples or fill buffer with zeroes
    /// Now backed by a circular buffer
    /// </summary>
    public class BufferedSampleProvider : ISampleProvider, IWaveProvider {
        /// <summary>
        /// Creates a new buffered SampleProvider
        /// </summary>
        /// <param name="waveFormat">WaveFormat</param>
        public BufferedSampleProvider(WaveFormat waveFormat) {
            this.byteProvider = new BufferedWaveProvider(waveFormat);
            this.BufferLength = waveFormat.AverageBytesPerSecond * 5 * 4;
        }

        /// <summary>
        /// Buffer length in bytes
        /// </summary>
        public int BufferLength {
            get {
                return byteProvider.BufferLength;
            }
            set {
                byteProvider.BufferLength = value;
            }
        }

        /// <summary>
        /// Buffer duration
        /// </summary>
        public TimeSpan BufferDuration {
            get {
                return byteProvider.BufferDuration;
            }
            set {
                byteProvider.BufferDuration = value;
            }
        }

        /// <summary>
        /// If true, when the buffer is full, start throwing away data
        /// if false, AddSamples will throw an exception when buffer is full
        /// </summary>
        public bool DiscardOnBufferOverflow {
            get {
                return byteProvider.DiscardOnBufferOverflow;
            }
            set {
                byteProvider.DiscardOnBufferOverflow = value;
            }
        }

        /// <summary>
        /// The number of buffered bytes
        /// </summary>
        public int BufferedBytes {
            get {
                return byteProvider.BufferedBytes;
            }
        }

        /// <summary>
        /// Buffered Duration
        /// </summary>
        public TimeSpan BufferedDuration {
            get {
                return byteProvider.BufferedDuration;
            }
        }

        /// <summary>
        /// Gets the WaveFormat
        /// </summary>
        public WaveFormat WaveFormat {
            get {
                return byteProvider.WaveFormat;
            }
        }

        //private CircularBuffer circularBuffer;
        ///// <summary>
        ///// Adds samples. Takes a copy of buffer, so that buffer can be reused if necessary
        ///// </summary>
        //public void AddSamples(float[] buffer, int offset, int count) {
        //    // create buffer here to allow user to customise buffer length
        //    if (this.circularBuffer == null) {
        //        this.circularBuffer = new CircularBuffer(this.BufferLength * 4);
        //    }

        //    List<byte> byBuf = new List<byte>();
        //    for (int i = 0; i < buffer.Length; i++) {
        //        byBuf.AddRange(BitConverter.GetBytes(buffer[i]));
        //    }

        //    int written = this.circularBuffer.Write(byBuf.ToArray(), offset * 4, count * 4);
        //    if (written < count && !DiscardOnBufferOverflow) {
        //        throw new InvalidOperationException("Buffer full");
        //    }
        //}

        ///// <summary>
        ///// Reads from this WaveProvider
        ///// Will always return count bytes, since we will zero-fill the buffer if not enough available
        ///// </summary>
        //public int Read(float[] buffer, int offset, int count) {
        //    byte[] byBuf = new byte[buffer.Length * 4];
        //    int read = 0;
        //    if (this.circularBuffer != null) // not yet created
        //    {
        //        read = this.circularBuffer.Read(byBuf, offset, count);
        //    }

        //    List<float> reAssembl = new List<float>();
        //    for (int i = 0; i < byBuf.Length; i += 4) {
        //        reAssembl.Add(BitConverter.ToSingle(byBuf, i));
        //    }

        //    for (int i = 0; i < buffer.Length; i++) {
        //        if (i >= reAssembl.Count) {
        //            buffer[i] = 0;
        //        } else {
        //            buffer[i] = reAssembl[i];
        //        }
        //    }

        //    return count;
        //}

        //public int Read(byte[] buffer, int offset, int count) {
        //    int read = 0;
        //    if (this.circularBuffer != null) // not yet created
        //    {
        //        read = this.circularBuffer.Read(buffer, offset, count);
        //    }
        //    if (read < count) {
        //        // zero the end of the buffer
        //        Array.Clear(buffer, offset + read, count - read);
        //    }
        //    return count;
        //}

        private BufferedWaveProvider byteProvider;
        public BufferedWaveProvider ByteProvider {
            get {
                return byteProvider;
            }
        }

        List<byte> byBufWrite = new List<byte>();
        /// <summary>
        /// Adds samples. Takes a copy of buffer, so that buffer can be reused if necessary
        /// </summary>
        public void AddSamples(float[] buffer, int offset, int count) {
            byBufWrite.Clear();
            for (int i = 0; i < buffer.Length; i++) {
                byBufWrite.AddRange(BitConverter.GetBytes(buffer[i]));
            }

            if(byBufWrite.Count >= byteProvider.BufferLength) {
                byteProvider.BufferLength = byBufWrite.Count * 4;
            }

            this.byteProvider.AddSamples(byBufWrite.ToArray(), offset * 4, byBufWrite.Count);
        }

        byte[] byBufRead = new byte[0];
        List<float> reAssembl = new List<float>();
        /// <summary>
        /// Reads from this WaveProvider
        /// Will always return count bytes, since we will zero-fill the buffer if not enough available
        /// </summary>
        public int Read(float[] buffer, int offset, int count) {
            if (byBufRead.Length < buffer.Length * 4) {
                byBufRead = new byte[buffer.Length * 4];
            }
            int read = 0;
            if (this.byteProvider != null) // not yet created
            {
                read = this.byteProvider.Read(byBufRead, offset*4, count*4);
            }

            reAssembl.Clear();
            for (int i = 0; i < read; i += 4) {
                reAssembl.Add(BitConverter.ToSingle(byBufRead, i));
            }

            for (int i = 0; i < buffer.Length; i++) {
                if (i >= reAssembl.Count) {
                    buffer[i] = 0;
                } else {
                    buffer[i] = reAssembl[i];
                }
            }

            return count;
        }

        public int Read(byte[] buffer, int offset, int count) {
            if (this.byteProvider != null) // not yet created
            {
                return this.byteProvider.Read(buffer, offset, count);
            }
            return 0;
            //int read = 0;
            //if (this.circularBuffer != null) // not yet created
            //{
            //    read = this.circularBuffer.Read(buffer, offset, count);
            //}
            //if (read < count) {
            //    // zero the end of the buffer
            //    Array.Clear(buffer, offset + read, count - read);
            //}
            //return count;
        }
    }
}
