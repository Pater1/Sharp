using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpStreamExtentions {
    public static class HelperExtentions {
        public static bool IsFull(this BufferedWaveProvider buffer) {
            return buffer != null &&
                       (buffer.BufferLength - buffer.BufferedBytes
                       < buffer.WaveFormat.AverageBytesPerSecond / 2);
        }
        public static bool IsFull(this BufferedWaveProvider buffer, int cutoff) {
            return buffer != null &&
                       (buffer.BufferLength - buffer.BufferedBytes
                       < cutoff);
        }
        //public static bool IsFull(this BufferedSampleProvider buffer, int cutoff) {
        //    return buffer != null &&
        //               (buffer.BufferLength - buffer.BufferedBytes
        //               < cutoff);
        //}
        public static bool IsFull(this BufferedSampleProvider buffer, int cutoff) {
            return IsFull(buffer.ByteProvider, cutoff * 4);
        }
        public static bool IsFull(this Stream buffer) {
            return buffer != null &&
                buffer.Position >= (buffer.Length/4) *3;
        }

        public static T[] Resize<T>(this T[] raw, int toLength) {
            Array.Resize(ref raw, toLength);
            return raw;
        }
    }
}
