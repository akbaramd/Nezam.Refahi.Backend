public static class FinanceMessagingRoutes
{
  // One command/messages queue for the Finance BC (billing lives here in your current layout)
  public const string FinanceMessagesQueueName = "nezam.finance.commands.v1";
  public static readonly Uri FinanceMessagesQueue = new Uri($"queue:{FinanceMessagesQueueName}");

  public static class ReferenceTypes
  {
    public const string WalletDeposit = "WalletDeposit";
    public const string TourReservation = "TourReservation";
  }
};

