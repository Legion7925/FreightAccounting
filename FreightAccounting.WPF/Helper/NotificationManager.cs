using System;

namespace FreightAccounting.WPF.Helper;

public static class NotificationManager
{
    public static event EventHandler<MessageTypeEnum>? showMessage;

    public static void OnShowMessage(string message, MessageTypeEnum messageType)
    {
        showMessage?.Invoke(message, messageType);
    }
}
