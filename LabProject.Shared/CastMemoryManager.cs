// ============================================================================
// File Name:    CastMemoryManager.cs
// Author:       Apollo3zehn
// URL:          https://github.com/Apollo3zehn/HDF5.NET/blob/master/src/HDF5.NET/Utils/CastMemoryManager.cs
// Licensed under the MIT License.
// ============================================================================

namespace LabProject;

using System;
using System.Buffers;
using System.Runtime.InteropServices;

public class CastMemoryManager<TFrom, TTo>(Memory<TFrom> from) : MemoryManager<TTo>
    where TFrom : struct
    where TTo : struct
{
    public override Span<TTo> GetSpan() => MemoryMarshal.Cast<TFrom, TTo>(from.Span);

    protected override void Dispose(bool disposing)
    {
    }

    public override MemoryHandle Pin(int elementIndex = 0) => throw new NotSupportedException();

    public override void Unpin() => throw new NotSupportedException();
}