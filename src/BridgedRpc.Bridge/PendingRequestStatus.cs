﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgedRpc.Bridge
{
	public enum PendingRequestStatus
	{
		Started,
		ServerGotParameters,
		Success
	}
}
