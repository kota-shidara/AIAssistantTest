using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Audio
{
    public static class WavConverter
    {
        private const int BitsPerSample = 16;
        private const int AudioFormat = 1;

        /// <summary>
        /// Wavファイルのデータ構造に変換する
        /// </summary>
        /// <param name="audioClip"></param>
        /// <returns></returns>
        public static byte[] ToWav(this AudioClip audioClip)
        {
            using var stream = new MemoryStream();
            
            WriteRiffChunk(audioClip, stream);
            WriteFmtChunk(audioClip, stream);
            WriteDataChunk(audioClip, stream);

            return stream.ToArray();
        }

        private static void WriteRiffChunk(AudioClip audioClip, Stream stream)
        {
            // ChunkID RIFF
            stream.Write(Encoding.ASCII.GetBytes("RIFF"));

            // ChunkSize
            const int headerByteSize = 44;
            var chunkSize = BitConverter.GetBytes((UInt32)(headerByteSize + audioClip.samples  * audioClip.channels * BitsPerSample / 8));
            stream.Write(chunkSize);
            
            // Format WAVE
            stream.Write(Encoding.ASCII.GetBytes("WAVE"));
        }

        private static void WriteFmtChunk(AudioClip audioClip, Stream stream)
        {
            // Subchunk1ID fmt
            stream.Write(Encoding.ASCII.GetBytes("fmt "));

            // Subchunk1Size (16 for PCM)
            stream.Write(BitConverter.GetBytes((UInt32)16));
            
            // AudioFormat (PCM=1)
            stream.Write(BitConverter.GetBytes((UInt16)AudioFormat));
            
            // NumChannels (Mono = 1, Stereo = 2, etc.)
            stream.Write(BitConverter.GetBytes((UInt16)audioClip.channels));
            
            // SampleRate (audioClip.sampleではなくaudioClip.frequencyのはず)
            stream.Write(BitConverter.GetBytes((UInt32)audioClip.frequency));
            
            // ByteRate (=SampleRate * NumChannels * BitsPerSample/8)
            stream.Write(BitConverter.GetBytes((UInt32)(audioClip.samples * audioClip.channels * BitsPerSample / 8)));
            
            // BlockAlign (=NumChannels * BitsPerSample/8)
            stream.Write(BitConverter.GetBytes((UInt16)(audioClip.channels * BitsPerSample / 8)));
            
            // BitsPerSample
            stream.Write(BitConverter.GetBytes((UInt16)BitsPerSample));
        }

        private static void WriteDataChunk(AudioClip audioClip, Stream stream)
        {
            // Subchunk2ID data
            stream.Write(Encoding.ASCII.GetBytes("data"));
            
            // Subchuk2Size
            stream.Write(BitConverter.GetBytes((UInt32)(audioClip.samples * audioClip.channels * BitsPerSample / 8)));
            
            // Data
            var floatData = new float[audioClip.samples * audioClip.channels];
            audioClip.GetData(floatData, 0);

            switch (BitsPerSample)
            {
                case 8:
                    foreach (var f in floatData) stream.Write(BitConverter.GetBytes((sbyte) (f * sbyte.MaxValue)));
                    break;
                case 16:
                    foreach (var f in floatData) stream.Write(BitConverter.GetBytes((short)(f * short.MaxValue)));
                    break;
                case 32:
                    foreach (var f in floatData) stream.Write(BitConverter.GetBytes((int)(f * int.MaxValue)));
                    break;
                case 64:
                    foreach (var f in floatData) stream.Write(BitConverter.GetBytes((float)(f * float.MaxValue)));
                default:
                    throw new NotSupportedException(nameof(BitsPerSample));
            }
        }
    }
}