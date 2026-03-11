using CommunityToolkit.Mvvm.Messaging.Messages;
using MudBlazor;

namespace DCSMCT.Messages
{
    public class AddToasterMessage : ValueChangedMessage<KeyValuePair<string, Severity>>
    {
        public AddToasterMessage(KeyValuePair<string, Severity> val) : base(val)
        {
        }
    }
}
