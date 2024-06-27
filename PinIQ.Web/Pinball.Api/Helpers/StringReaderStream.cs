using System;
using System.IO;
using System.Text;

namespace Pinball.Api.Helpers;

/// <summary>
///     Convert string to byte stream.
///     <para>
///         Slower than <see cref="Encoding.GetBytes()" />, but saves memory for a large string.
///     </para>
/// </summary>
public class StringReaderStream : Stream
{
    private readonly Encoding encoding;
    private readonly string input;
    private readonly int inputLength;
    private readonly long length;
    private readonly int maxBytesPerChar;
    private int inputPosition;
    private long position;

    public StringReaderStream(string input)
        : this(input, Encoding.UTF8)
    {
    }

    public StringReaderStream(string input, Encoding encoding)
    {
        this.encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        this.input = input;
        inputLength = input.Length;
        if (!string.IsNullOrEmpty(input))
            length = encoding.GetByteCount(input);
        maxBytesPerChar = Equals(encoding, Encoding.ASCII) ? 1 : encoding.GetMaxByteCount(1);
    }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => length;

    public override long Position
    {
        get => position;
        set => throw new NotImplementedException();
    }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (inputPosition >= inputLength)
            return 0;
        if (count < maxBytesPerChar)
            throw new ArgumentException("count has to be greater or equal to max encoding byte count per char");
        var charCount = Math.Min(inputLength - inputPosition, count / maxBytesPerChar);
        var byteCount = encoding.GetBytes(input, inputPosition, charCount, buffer, offset);
        inputPosition += charCount;
        position += byteCount;
        return byteCount;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }
}