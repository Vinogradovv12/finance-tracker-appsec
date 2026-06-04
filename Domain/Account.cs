using FinanceTracker.Api.Common;

namespace FinanceTracker.Api.Domain;

public class Account
    {
        private readonly IAppLogger _logger;
        private readonly List<ITransaction> _transactions = [];
        private readonly HashSet<Guid> _processedIds = [];
        private readonly object _lock = new ();
        private const int MaxTransaction = 10_000;

        public Account(IAppLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public void AddTransaction(ITransaction transaction)
        {
            lock (_lock)
            {
                ArgumentNullException.ThrowIfNull(transaction);

                if (_processedIds.Contains(transaction.Id))
                {
                    _logger.Error($"Transaction {transaction.Id} already exists. Skipping.");
                    return;
                }

                if (_transactions.Count >= MaxTransaction)
                {
                    _logger.Error("Transaction limit reached. Cannot add more transactions.");
                    throw new InvalidOperationException("Limit reached");
                }

                _processedIds.Add(transaction.Id);
                _transactions.Add(transaction);

                _logger.Info($"Transaction {transaction.Id} added successfully.");
            }
        }
        //public decimal GetBalance() => _transactions.Sum(t => t.Value);
        public IReadOnlyList<ITransaction> AllTransactions => _transactions.AsReadOnly();
    }