DELETE FROM transactions;
DELETE FROM currencyItems;
DELETE FROM wallets;
DELETE FROM users;
DELETE FROM currencies;

SET IDENTITY_INSERT users ON;
INSERT INTO users (Id, Name, Email, Password)
VALUES 
(1, 'Alice', 'alice@example.com', '$2a$11$8pTNd0YZh8YYN1XKzS6Z/OY7OwUhZpAIVgG1RI9Zq6L5PUI2Acx3e'),
(2, 'Bob', 'bob@example.com', '$2a$11$3P0cBqbm9j1AX2VmKkB1MeT46TqWT1m1bnM4RfRIu9fZ07dF0KOgG');  
SET IDENTITY_INSERT users OFF;

SET IDENTITY_INSERT wallets ON;
INSERT INTO wallets (WalletId, UserId, Balance)
VALUES 
(1, 1, 5000),
(2, 2, 10000);
SET IDENTITY_INSERT wallets OFF;

SET IDENTITY_INSERT currencies ON;
INSERT INTO currencies (CurrencyId, Name, Value)
VALUES 
(1, 'Bitcoin', 60000),
(2, 'Ethereum', 3000),
(3, 'Dogecoin', 0.12),
(4, 'Cardano', 0.58),
(5, 'Polkadot', 7.2),
(6, 'Solana', 145.5),
(7, 'Ripple', 0.62),
(8, 'Litecoin', 82.3),
(9, 'Chainlink', 17.8),
(10, 'Stellar', 0.11),
(11, 'Avalanche', 40.6),
(12, 'Uniswap', 10.9),
(13, 'Monero', 160.7),
(14, 'Aave', 95.2),
(15, 'VeChain', 0.034);
SET IDENTITY_INSERT currencies OFF;

SET IDENTITY_INSERT currencyItems ON;
INSERT INTO currencyItems (CurrencyItemId, CurrencyId, WalletId, BuyValue, CryptoAmount)
VALUES 
(1, 1, 1, 1200, 2.1), 
(2, 2, 2, 500, 3.2),   
(3, 3, 2, 300, 0.2);   
SET IDENTITY_INSERT currencyItems OFF;

SET IDENTITY_INSERT transactions ON;
INSERT INTO transactions (TransactionId, UserId, CurrencyId, TransactionType, Amount, Rate, Timestamp)
VALUES 
(1, 1, 1, 'Buy', 2.1, 60000, '2025-04-01 10:00:00'),
(2, 2, 2, 'Buy', 3.2, 3000, '2025-04-02 11:00:00'),
(3, 2, 3, 'Sell', 0.2, 0.12, '2025-04-03 12:00:00');
SET IDENTITY_INSERT transactions OFF;
