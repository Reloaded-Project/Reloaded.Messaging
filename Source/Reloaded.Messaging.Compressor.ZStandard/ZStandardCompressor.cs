using System;
using Reloaded.Messaging.Compression;
using ZstdNet;

namespace Reloaded.Messaging.Compressor.ZStandard
{
    public class ZStandardCompressor : ICompressor, IDisposable
    {
        public readonly ZstdNet.Compressor Compressor;
        public readonly Decompressor Decompressor;

        public ZStandardCompressor(CompressionOptions compressionOptions = null, DecompressionOptions decompressionOptions = null)
        {
            Compressor = compressionOptions != null ? new ZstdNet.Compressor(compressionOptions) 
                                                    : new ZstdNet.Compressor();

            Decompressor = decompressionOptions != null ? new Decompressor(decompressionOptions) 
                                                        : new Decompressor();
        }

        ~ZStandardCompressor()
        {
            Dispose();
        }

        public void Dispose()
        {
            Compressor?.Dispose();
            Decompressor?.Dispose();
            GC.SuppressFinalize(this);
        }

        public byte[] Compress(byte[] data)
        {
            return Compressor.Wrap(data);
        }

        public byte[] Decompress(byte[] data)
        {
            return Decompressor.Unwrap(data);
        }
    }
}
