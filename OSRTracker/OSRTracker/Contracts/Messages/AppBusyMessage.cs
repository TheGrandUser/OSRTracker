using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace OSRTracker.Contracts.Messages;

public class AppBusyMessage() : RequestMessage<IDisposable>;
