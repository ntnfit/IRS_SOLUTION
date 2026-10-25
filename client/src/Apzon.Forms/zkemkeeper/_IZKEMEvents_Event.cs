using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace zkemkeeper;

[ComVisible(false)]
[ComEventInterface(typeof(_IZKEMEvents), typeof(_IZKEMEvents))]
public interface _IZKEMEvents_Event
{
	void _VtblGap1_4();

	event _IZKEMEvents_OnEnrollFingerEventHandler OnEnrollFinger;

	void _VtblGap2_4();

	event _IZKEMEvents_OnConnectedEventHandler OnConnected;

	event _IZKEMEvents_OnDisConnectedEventHandler OnDisConnected;

	event _IZKEMEvents_OnFingerEventHandler OnFinger;

	void _VtblGap3_16();

	event _IZKEMEvents_OnAttTransactionExEventHandler OnAttTransactionEx;
}
