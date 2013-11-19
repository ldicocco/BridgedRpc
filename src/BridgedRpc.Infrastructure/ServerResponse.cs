using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BridgedRpc.Infrastructure
{
	public class ServerResponse
	{
		public object Result { get; set; }
		public bool Success { get; set; }
		public ServerResponseErrorCode ErrorCode { get; set; }
		public string ErrorReason { get; set; }
		public string ExceptionDescription { get; set; }
//		public string EncodedFile { get; set; }
		public Stream Stream { get; set; }

		public ServerResponse()
		{
		}

		public ServerResponse(object result)
		{
			Success = true;
			Result = result;
			ErrorCode = ServerResponseErrorCode.None;
			ErrorReason = "OK";
			ExceptionDescription = "NONE";
		}

		public ServerResponse(ServerResponseErrorCode errorCode, string errorReason, string exceptionDescription = null)
		{
			Success = false;
			ErrorCode = errorCode;
			ErrorReason = errorReason;
			ExceptionDescription = exceptionDescription;
		}
	}
}
