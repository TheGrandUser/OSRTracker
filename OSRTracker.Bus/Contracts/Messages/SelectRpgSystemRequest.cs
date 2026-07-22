using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.Messaging.Messages;
using OSRTracker.Models;

namespace OSRTracker.Contracts.Messages;

public abstract record SelectRpgResponse
{
   public sealed record Success(SystemDto RpgSystem) : SelectRpgResponse;
   public sealed record Cancelled() : SelectRpgResponse;
   public sealed record BlankSystem() : SelectRpgResponse;
}

public class SelectRpgSystemRequest : AsyncRequestMessage<SelectRpgResponse>;
