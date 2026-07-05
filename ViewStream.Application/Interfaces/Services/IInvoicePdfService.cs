namespace ViewStream.Application.Interfaces.Services
{
    /// <summary>
    /// Generates PDF invoices for subscriptions.
    /// </summary>
    public interface IInvoicePdfService
    {
        /// <summary>
        /// Generates a PDF invoice and returns the raw byte content.
        /// </summary>
        Task<byte[]> GenerateAsync(long invoiceId, string userName, string userEmail,
            string planType, decimal amount, string currency, DateOnly invoiceDate, string? transactionId);
    }
}
