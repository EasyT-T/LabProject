namespace LabProject.Extensions;

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;

public static class StreamExtension
{
    extension(Stream stream)
    {
        public unsafe T Read<T>() where T : unmanaged
        {
            Span<byte> buffer = stackalloc byte[sizeof(T)];

            var length = stream.Read(buffer);

            Guard.IsEqualTo(length, buffer.Length);

            return MemoryMarshal.Read<T>(buffer);
        }

        public unsafe int Read<T>(Span<T> buffer)
            where T : unmanaged
        {
            var span = MemoryMarshal.Cast<T, byte>(buffer);

            var length = stream.Read(span);

            Guard.IsEqualTo(length % sizeof(T), 0);

            return length / sizeof(T);
        }

        public async Task<int> ReadAsync<T>(Memory<T> buffer, CancellationToken cancellationToken = default)
            where T : unmanaged
        {
            var memory = MemoryMarshal.Cast<T, byte>(buffer);

            var length = await stream.ReadAsync(memory, cancellationToken);

            unsafe
            {
                Guard.IsEqualTo(length % sizeof(T), 0);

                return length / sizeof(T);
            }
        }
    }
}