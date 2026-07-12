namespace LabProject.Extensions;

using System;
using System.Runtime.InteropServices;

public static class MemoryMarshalExtensions
{
    extension(MemoryMarshal)
    {
        public static Memory<TTo> Cast<TFrom, TTo>(Memory<TFrom> memory)
            where TFrom : struct
            where TTo : struct
        {
            using var manager = new CastMemoryManager<TFrom, TTo>(memory);

            return manager.Memory;
        }

        public static ReadOnlyMemory<TTo> Cast<TFrom, TTo>(ReadOnlyMemory<TFrom> memory)
            where TFrom : struct
            where TTo : struct
        {
            using var manager = new CastMemoryManager<TFrom, TTo>(MemoryMarshal.AsMemory(memory));

            return manager.Memory;
        }
    }
}