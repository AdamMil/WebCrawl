using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace WebCrawl.Backend
{

#region IOBuffer
public unsafe abstract class IOBuffer : IDisposable
{
  public IOBuffer(int bufferSize)
  {
    if(bufferSize < 0) throw new ArgumentOutOfRangeException();
    if(bufferSize == 0) bufferSize = 4096;

    buffer = new byte[bufferSize];
    PinBuffer();
  }

  ~IOBuffer()
  {
    Dispose(true);
  }

  public void Dispose()
  {
    GC.SuppressFinalize(this);
    Dispose(false);
  }

  protected byte[] Buffer
  {
    get { return buffer; }
  }

  protected byte* BufferPtr
  {
    get { return bufferPtr; }
  }

  protected virtual byte[] CreateResizeBuffer(int newSize)
  {
    byte[] newBuffer = new byte[newSize];
    Array.Copy(buffer, newBuffer, buffer.Length);
    return newBuffer;
  }

  protected virtual void Dispose(bool finalizing)
  {
    FreeBuffer();
  }
  
  protected void EnlargeBuffer(int size)
  {
    if(size <= buffer.Length) return;

    int newSize = buffer.Length, add = 4096;

    if((newSize & 0xFFF) != 0) // if the buffer size is not a multiple of 4096, grow the buffer in doubles
    {
      add = buffer.Length;
    }

    do newSize += add; while(newSize < size);

    byte[] newBuffer = CreateResizeBuffer(newSize);

    FreeBuffer();
    buffer = newBuffer;
    PinBuffer();
  }

  protected static void Copy(void* src, void* dest, int nbytes)
  {
    uint* dwsrc=(uint*)src, dwdest=(uint*)dest;
    int quads = nbytes>>2;
    for(int i=0; i<quads; i++)
    {
      dwdest[i] = dwsrc[i];
    }

    nbytes = nbytes&3;
    if(nbytes != 0)
    {
      int offset = quads<<2;
      byte* bsrc=(byte*)src+offset, bdest=(byte*)dest+offset;
      do *bdest++ = *bsrc++; while(--nbytes != 0);
    }
  }

  protected static void MakeLittleEndian2(void* data, int words)
  {
    if(!BitConverter.IsLittleEndian)
    {
      SwapEndian2((byte*)data, words);
    }
  }

  protected static void MakeLittleEndian4(void* data, int dwords)
  {
    if(!BitConverter.IsLittleEndian)
    {
      SwapEndian4((byte*)data, dwords);
    }
  }

  protected static void MakeSystemEndian2(void* data, int words)
  {
    if(!BitConverter.IsLittleEndian)
    {
      SwapEndian2((byte*)data, words);
    }
  }

  protected static void MakeSystemEndian4(void* data, int dwords)
  {
    if(!BitConverter.IsLittleEndian)
    {
      SwapEndian4((byte*)data, dwords);
    }
  }

  static void SwapEndian2(byte* data, int words)
  {
    byte* end = data + words*sizeof(ushort);
    for(; data != end; data += sizeof(ushort))
    {
      byte t  = data[0];
      data[0] = data[1];
      data[1] = t;
    }
  }

  static void SwapEndian4(byte* data, int dwords)
  {
    byte* end = data + dwords*sizeof(uint);
    for(; data != end; data += sizeof(uint))
    {
      byte t  = data[0];
      data[0] = data[3];
      data[3] = t;

      t       = data[1];
      data[1] = data[2];
      data[2] = t;
    }
  }

  void FreeBuffer()
  {
    if(handle.IsAllocated) handle.Free();
    bufferPtr = null;
  }
  
  void PinBuffer()
  {
    handle    = GCHandle.Alloc(buffer, GCHandleType.Pinned);
    bufferPtr = (byte*)handle.AddrOfPinnedObject().ToPointer();
  }

  byte[] buffer;
  byte*  bufferPtr;
  GCHandle handle;
}
#endregion

#region IOReader
public unsafe sealed class IOReader : IOBuffer
{
  public IOReader(Stream stream) : this(stream, 0, false) { }

  public IOReader(Stream stream, int bufferSize, bool shared) : base(bufferSize)
  {
    if(stream == null) throw new ArgumentNullException();
    this.stream = stream;
    this.shared = shared;
    StoreStreamPosition();
  }

  public Stream BaseStream
  {
    get { return this.stream; }
  }

  public bool EOF
  {
    get { return AvailableData == 0 && stream.Position == stream.Length; }
  }

  public long Position
  {
    get { return stream.Position - AvailableData; }
    set
    {
      int availableData = AvailableData;
      long dataEnd = stream.Position, dataStart = dataEnd - availableData;

      // if the new position is within the range of data in our buffer, we can simply tweak our tail
      if(value >= dataStart && value < dataEnd)
      {
        AdvanceTail((int)(value-dataStart));
      }
      else // otherwise, we have to discard the whole buffer
      {
        tailIndex = headIndex = 0;
        stream.Position = lastStreamPosition = value;
      }
    }
  }

  public bool ReadBool()
  {
    return ReadByte() != 0;
  }

  public byte ReadByte()
  {
    return *ReadContiguousData(1);
  }

  public sbyte ReadSByte()
  {
    return (sbyte)ReadByte();
  }

  public char ReadChar()
  {
    return (char)ReadUShort();
  }

  public short ReadShort()
  {
    return (short)ReadUShort();
  }

  public ushort ReadUShort()
  {
    byte* data = ReadContiguousData(sizeof(ushort));
    return (ushort)(data[0] | (data[1]<<8));
  }

  public int ReadInt()
  {
    return (int)ReadUint();
  }

  public uint ReadUint()
  {
    byte* data = ReadContiguousData(sizeof(uint));
    return (uint)data[0] | ((uint)data[1]<<8) | ((uint)data[2]<<16) | ((uint)data[3]<<24);
  }

  public byte[] ReadByteArray(int nbytes)
  {
    byte[] data = new byte[nbytes];
    fixed(byte* ptr=data)
    {
      ReadData(ptr, nbytes);
    }
    return data;
  }

  public char[] ReadCharArray(int nchars)
  {
    char[] data = new char[nchars];
    fixed(char* ptr=data) ReadCharArray(ptr, nchars);
    return data;
  }

  public void ReadCharArray(char[] array, int index, int nchars)
  {
    fixed(char* ptr=array) ReadCharArray(ptr+index, nchars);
  }

  public void ReadCharArray(char* array, int nchars)
  {
    ReadData(array, nchars*sizeof(char));
    MakeSystemEndian2(array, nchars);
  }

  public ushort[] ReadUShortArray(int nwords)
  {
    ushort[] data = new ushort[nwords];
    fixed(ushort* ptr=data)
    {
      ReadData(ptr, nwords*sizeof(ushort));
      MakeSystemEndian2(ptr, nwords);
    }
    return data;
  }

  public uint[] ReadUintArray(int ndwords)
  {
    uint[] data = new uint[ndwords];
    fixed(uint* ptr = data)
    {
      ReadData(ptr, ndwords*sizeof(uint));
      MakeSystemEndian4(ptr, ndwords);
    }
    return data;
  }

  public string ReadString(int nchars)
  {
    if(nchars == 0) return string.Empty;
    char* data = (char*)ReadContiguousData(nchars * sizeof(char));
    MakeSystemEndian2(data, nchars);
    return new string(data, 0, nchars);
  }

  public string ReadStringWithLength()
  {
    int nChars = ReadInt();
    return nChars == -1 ? null : ReadString(nChars);
  }

  public void Skip(int nbytes)
  {
    int advance = Math.Min(AvailableData, nbytes);
    AdvanceTail(advance);
    nbytes -= advance;
    if(nbytes != 0) Position += nbytes;
  }

  public void SkipStringWithLength()
  {
    int length = ReadInt();
    if(length > 0) Skip(length * sizeof(char));
  }

  protected override byte[] CreateResizeBuffer(int newSize)
  {
    byte[] newBuffer = new byte[newSize];

    // copy the available data so it starts at the beginning of the new buffer
    int availableData = AvailableData;
    if(tailIndex <= headIndex)
    {
      Array.Copy(Buffer, tailIndex, newBuffer, 0, availableData);
    }
    else
    {
      Array.Copy(Buffer, tailIndex, newBuffer, 0, Buffer.Length - tailIndex);
      Array.Copy(Buffer, 0, newBuffer, tailIndex, headIndex);
    }
    
    tailIndex = 0; // fixup the indices to reflect that the data has been moved to the beginning
    headIndex = availableData;
    
    return newBuffer;
  }

  int AvailableData
  {
    get { return tailIndex <= headIndex ? headIndex - tailIndex : headIndex + Buffer.Length-tailIndex; }
  }
  
  int ContiguousData
  {
    get { return (tailIndex <= headIndex ? headIndex : Buffer.Length) - tailIndex; }
  }

  void AdvanceTail(int nbytes)
  {
    tailIndex += nbytes;

    Debug.Assert(tailIndex <= Buffer.Length);
    if(tailIndex == headIndex)
    {
      headIndex = tailIndex = 0;
    }
  }

  byte* ReadContiguousData(int nbytes)
  {
    if(ContiguousData < nbytes) // if there's not enough contiguous data, read more data and/or shift existing data
    {
      if(Buffer.Length < nbytes) // if the buffer simply isn't big enough, we'll first enlarge it
      {
        EnlargeBuffer(nbytes);
        if(ContiguousData >= nbytes) goto done; // enlarging the buffer compacts the data, so there may be enough now
      }

      // find out how many contiguous bytes we can fit without shifting data around
      int availableContiguousBytes = Buffer.Length - tailIndex;
      if(availableContiguousBytes < nbytes) // if it's not enough, we'll need to shift the data
      {
        if(tailIndex < headIndex) // if the data is in one chunk, we can simply move the chunk
        {
          Array.Copy(Buffer, tailIndex, Buffer, 0, headIndex-tailIndex);
        }
        else // otherwise we'll need to move two chunks, using a temporary storage space to hold one of them
        {
          // this is an edge case, so we'll always choose the second chunk for placement in temporary storage.
          // there are potential optimizations that can be done.
          byte[] temp = new byte[headIndex];
          Array.Copy(Buffer, temp, headIndex);
          Array.Copy(Buffer, tailIndex, Buffer, 0, Buffer.Length - tailIndex);
          Array.Copy(temp, 0, Buffer, Buffer.Length - tailIndex, headIndex);
        }
        headIndex = AvailableData; // update the pointers to indicate that the data has been defragmented
        tailIndex = 0;
        
        availableContiguousBytes = Buffer.Length; // now we have a contiguous region the size of the whole buffer
      }
      
      EnsureStreamPositioned();

      int toRead = availableContiguousBytes - headIndex; // fill the entire contiguous region
      do
      {
        int read = stream.Read(Buffer, headIndex, toRead);
        if(read == 0)
        {
          if(AvailableData < nbytes) throw new EndOfStreamException();
          break;
        }
        headIndex += read;
        toRead -= read;
      } while(toRead != 0);
      
      StoreStreamPosition();
    }

    done:
    byte* data = BufferPtr + tailIndex;
    AdvanceTail(nbytes);
    return data;
  }

  void ReadData(void* dest, int nbytes)
  {
    byte* ptr = (byte*)dest;
    
    // attempt to satisfy the request with the contiguous data starting from the tail
    ReadDataInternal(ref ptr, ref nbytes, Math.Min(ContiguousData, nbytes));

    if(nbytes != 0)
    {
      if(headIndex != 0) // attempt to satisfy the remaining request with contiguous data starting from index 0
      {
        ReadDataInternal(ref ptr, ref nbytes, Math.Min(headIndex, nbytes));
      }

      // if that wasn't enough either, we've exhausted the buffer and will read more in the loop below
      if(nbytes != 0)
      {
        EnsureStreamPositioned();
        do
        {
          headIndex = stream.Read(Buffer, 0, Buffer.Length);
          if(headIndex == 0) throw new EndOfStreamException();
          ReadDataInternal(ref ptr, ref nbytes, Math.Min(headIndex, nbytes));
        } while(nbytes != 0);
        StoreStreamPosition();
      }
    }
  }
  
  void EnsureStreamPositioned()
  {
    if(shared && stream.Position != lastStreamPosition)
    {
      stream.Position = lastStreamPosition;
    }
  }

  void ReadDataInternal(ref byte* ptr, ref int bytesNeeded, int bytesAvailable)
  {
    Copy(BufferPtr+tailIndex, ptr, bytesAvailable);
    AdvanceTail(bytesAvailable);
    ptr += bytesAvailable;
    bytesNeeded -= bytesAvailable;
  }

  void StoreStreamPosition()
  {
    if(shared) lastStreamPosition = stream.Position;
  }

  readonly Stream stream;
  long lastStreamPosition;
  int headIndex, tailIndex;
  bool shared;
}
#endregion

#region IOWriter
public unsafe sealed class IOWriter : IOBuffer
{
  public IOWriter(Stream stream) : base(0)
  {
    this.stream = stream;
  }
  
  public long Position
  {
    get { return stream.Position + writeIndex; }
    set
    {
      if(value != Position)
      {
        Flush();
        stream.Position = value;
      }
    }
  }

  public void Add(bool value)
  {
    Add(value ? (byte)1 : (byte)0);
  }

  public void Add(sbyte value)
  {
    Add((byte)value);
  }

  public void Add(byte value)
  {
    if(Buffer.Length == writeIndex) Flush(); // make sure there's room
    BufferPtr[writeIndex++] = value;
  }

  public void Add(char c)
  {
    Add((ushort)c);
  }

  public void Add(short value)
  {
    Add((ushort)value);
  }

  public void Add(ushort value)
  {
    EnsureSpace(sizeof(ushort));
    byte* ptr = BufferPtr + writeIndex;
    ptr[0] = (byte)value;
    ptr[1] = (byte)(value >> 8);
    writeIndex += sizeof(ushort);
  }

  public void Add(int value)
  {
    Add((uint)value);
  }
  
  public void Add(uint value)
  {
    EnsureSpace(sizeof(uint));
    byte* ptr = BufferPtr + writeIndex;
    ptr[0] = (byte)value;
    ptr[1] = (byte)(value >> 8);
    ptr[2] = (byte)(value >> 16);
    ptr[3] = (byte)(value >> 24);
    writeIndex += sizeof(uint);
  }

  public void Add(string str)
  {
    fixed(char* data=str) Add(data, str.Length);
  }
  
  public void AddStringWithLength(string str)
  {
    if(str == null)
    {
      Add(-1);
    }
    else
    {
      Add(str.Length);
      Add(str);
    }
  }

  public void Add(byte[] data)
  {
    fixed(byte* ptr=data) Add(ptr, data.Length);
  }

  public void Add(char[] data)
  {
    fixed(char* ptr=data) Add(ptr, data.Length);
  }

  public void Add(char[] data, int index, int count)
  {
    if(index < 0 || count < 0 || index+count > data.Length) throw new ArgumentOutOfRangeException();
    fixed(char* ptr=data) Add(ptr+index, count);
  }

  public void Add(uint[] data)
  {
    fixed(uint* ptr=data) Add(ptr, data.Length);
  }

  public void Add(uint[] data, int index, int count)
  {
    if(index < 0 || count < 0 || index+count > data.Length) throw new ArgumentOutOfRangeException();
    fixed(uint* ptr=data) Add(ptr+index, count);
  }

  public void Add(byte* data, int nbytes)
  {
    AddVoid(data, nbytes);
  }

  public void Add(char* data, int nchars)
  {
    Add((ushort*)data, nchars);
  }

  public void Add(ushort* data, int nwords)
  {
    AddVoid(data, nwords * sizeof(ushort));
    MakeLittleEndian2(nwords);
  }

  public void Add(uint* data, int ndwords)
  {
    AddVoid(data, ndwords * sizeof(uint));
    MakeLittleEndian4(ndwords);
  }

  public void Flush()
  {
    stream.Write(Buffer, 0, writeIndex);
    writeIndex = 0;
  }

  protected override byte[] CreateResizeBuffer(int newSize)
  {
    byte[] newBuffer = new byte[newSize];
    if(writeIndex != 0) Array.Copy(Buffer, newBuffer, writeIndex);
    return newBuffer;
  }

  protected override void Dispose(bool finalizing)
  {
    Flush();
    base.Dispose(finalizing);
  }

  void AddVoid(void* data, int nbytes)
  {
    if(nbytes < 0) throw new ArgumentOutOfRangeException();
    EnsureSpace(nbytes);
    Copy(data, BufferPtr+writeIndex, nbytes);
    writeIndex += nbytes;
  }

  void EnsureSpace(int nbytes)
  {
    int spaceLeft = Buffer.Length - writeIndex;
    if(spaceLeft < nbytes) // if there's not enough space...
    {
      Flush(); // see if flushing the buffer would free up enough space.
      if(Buffer.Length < nbytes) // if not, enlarge the buffer
      {
        EnlargeBuffer(nbytes);
      }
    }
  }

  void MakeLittleEndian2(int words)
  {
    MakeLittleEndian2(BufferPtr - words*sizeof(ushort), words);
  }

  void MakeLittleEndian4(int dwords)
  {
    MakeLittleEndian2(BufferPtr - dwords*sizeof(uint), dwords);
  }

  readonly Stream stream;
  int writeIndex;
}
#endregion

} // namespace WebCrawl.Backend
