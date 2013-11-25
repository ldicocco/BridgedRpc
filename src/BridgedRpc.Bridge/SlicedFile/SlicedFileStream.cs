using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BridgedRpc.Bridge.SlicedFile
{
	public class SlicedFileStream : Stream, IDisposable
	{
		private readonly Object _syncRoot = new Object();
		private readonly SliceData[] _slices;
		string _tempRoot;
		int _currentStreamIndex = 0;
		Stream _currentStream = null;
		long _position = 0;

		public SlicedFileStream(int numSlices, string tempRoot)
		{
			_slices = new SliceData[numSlices];
			_tempRoot = tempRoot;
			Directory.CreateDirectory(_tempRoot);
		}

		public int NumSlices
		{
			get
			{
				return _slices == null ? 0 : _slices.Length;
			}
		}

		public async Task<SliceData> SetSliceAsync(int index, Stream data)
		{
			SliceData sliceData = null;
			lock (_syncRoot)
			{
				if (_slices[index] != null)
				{
					return null;
				}
				sliceData = new SliceData();
				sliceData.TempFileName = Guid.NewGuid() + ".txt";
				_slices[index] = sliceData;
			}
			using (var fs = File.OpenWrite(Path.Combine(_tempRoot, sliceData.TempFileName)))
			{
				await data.CopyToAsync(fs);
			}

			if (HasAllSlices())
			{
				OpenAllStreams();
				_currentStream = _slices[_currentStreamIndex++].Stream;
			}
			return sliceData;
		}

		private bool HasAllSlices()
		{
			return _slices.All(i => i != null);
		}

		public bool IsReady()
		{
			return _slices.All(i => i != null && i.Stream != null);
		}

		private void OpenAllStreams()
		{
			foreach (var slice in _slices)
			{
				slice.Stream = File.OpenRead(Path.Combine(_tempRoot, slice.TempFileName));
			}
		}

		private void CloseAllStreams()
		{
			foreach (var slice in _slices)
			{
				slice.Stream.Close();
			}
		}

		private void DeleteAllStreams()
		{
			foreach (var slice in _slices)
			{
				File.Delete(Path.Combine(_tempRoot, slice.TempFileName));
			}
		}

		public override void Close()
		{
			CloseAllStreams();
			DeleteAllStreams();
			base.Close();
		}

		public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
		{
			int totBytesRead = 0;
			while (count > 0)
			{
				// Read what we can from the current stream
				int numBytesRead = await _currentStream.ReadAsync(buffer, offset, count);
				count -= numBytesRead;
				offset += numBytesRead;
				totBytesRead += numBytesRead;

				// If we haven't satisfied the read request, we have exhausted the child stream.
				// Move on to the next stream and loop around to read more data.
				if (count > 0)
				{
					// If we run out of child streams to read from, we're at the end of the MultiStream, and there is no more data to read
					if (_currentStreamIndex >= _slices.Length)
					{
						break;
					}

					// Otherwise, close the current child-stream and open the next one
					_currentStream.Close();
					_currentStream = _slices[_currentStreamIndex++].Stream;
				}
			}

			return totBytesRead;
		}

		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return false; }
		}

		public override bool CanWrite
		{
			get { return false; }
		}

		public override void Flush()
		{
		}

		public override long Length
		{
			get
			{
				long result = 0;
				foreach (var slice in _slices)
				{
					result += slice.Stream.Length;
				}
				return result;
			}
		}

		public override long Position
		{
			get { return _position; }
			set { Seek(value, SeekOrigin.Begin); }
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return ReadAsync(buffer, offset, count).Result;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
			/*			long len = Length;
						switch (origin)
						{
							case SeekOrigin.Begin:
								position = offset;
								break;
							case SeekOrigin.Current:
								position += offset;
								break;
							case SeekOrigin.End:
								position = len - offset;
								break;
						}
						if (position > len)
						{
							position = len;
						}
						else if (position < 0)
						{
							position = 0;
						}
						return position;*/
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		void IDisposable.Dispose()
		{
		}
	}
}
