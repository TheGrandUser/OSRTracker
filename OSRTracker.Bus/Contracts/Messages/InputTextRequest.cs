using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace OSRTracker.Contracts.Messages;

public class InputTextRequest : AsyncRequestMessage<string?>
{
   public string Title { get; }
   public string Message { get; }
   public string DefaultText { get; }
   public InputTextRequest(string title, string message, string defaultText = "")
   {
      Title = title;
      Message = message;
      DefaultText = defaultText;
   }
}
