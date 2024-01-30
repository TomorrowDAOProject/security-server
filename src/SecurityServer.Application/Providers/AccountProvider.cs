using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AElf;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.KeyStore;
using SecurityServer.Common;
using SecurityServer.Dtos;
using SecurityServer.Options;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SecurityServer.Providers;

public class AccountProvider : ISingletonDependency
{
    private readonly ILogger<AccountProvider> _logger;
    private readonly Dictionary<string, AccountHolder> _accountHolders = new();
    private readonly IOptionsMonitor<KeyStoreOptions> _keyStoreOptions;
    private readonly IOptionsMonitor<KeyPairInfoOptions> _keyPairInfoOptions;
    private static readonly KeyStoreService KeyStoreService = new();

    public AccountProvider(IOptionsMonitor<KeyStoreOptions> keyStoreOptions,
        IOptionsMonitor<KeyPairInfoOptions> keyPairInfoOptions,
        ILogger<AccountProvider> logger)
    {
        _keyStoreOptions = keyStoreOptions;
        _keyPairInfoOptions = keyPairInfoOptions;
        _logger = logger;
        LoadKeyPair();
        LoadKeyStoreWithPassword();
        LoadKeyStoreWithInput();
    }

    public string GetSignature(string addressOrPublicKey, byte[] rawData)
    {
        var accountExists = _accountHolders.TryGetValue(addressOrPublicKey, out var account);
        if (!accountExists) throw new UserFriendlyException("Account not found");

        return account.GetSignatureWith(rawData).ToHex();
    }

    private void LoadKeyPair()
    {
        if (_keyPairInfoOptions.CurrentValue.PrivateKeyDictionary.IsNullOrEmpty()) return;
        foreach (var (publicKey, privateKey) in _keyPairInfoOptions.CurrentValue.PrivateKeyDictionary)
        {
            var account = LoadByPrivateKey(privateKey);
            _logger.LogInformation("Load key pair success, address: {Address}", account.AddressObj().ToBase58());
        }
    }

    private string ReadKeyStore(string address)
    {
        var path = PathHelper.ResolvePath(_keyStoreOptions.CurrentValue.Path + "/" + address + ".json");
        if (!File.Exists(path))
        {
            throw new UserFriendlyException("keystore file not exits " + path);
        }

        using var textReader = File.OpenText(path);
        return textReader.ReadToEnd();
    }

    private AccountHolder LoadByPrivateKey(string privateKeyHex)
    {
        var account = new AccountHolder(privateKeyHex);
        _accountHolders[account.PublicKey] = account;
        _accountHolders[account.AddressObj().ToBase58()] = account;
        return account;
    }

    private void LoadKeyStoreWithPassword()
    {
        if (_keyStoreOptions.CurrentValue.Path.IsNullOrEmpty()) return;
        if (_keyStoreOptions.CurrentValue.Passwords.IsNullOrEmpty()) return;
        foreach (var (address, password) in _keyStoreOptions.CurrentValue.Passwords)
        {
            var keyStoreContent = ReadKeyStore(address);
            var privateKey = KeyStoreService.DecryptKeyStoreFromJson(password, keyStoreContent);
            LoadByPrivateKey(privateKey.ToHex());
            _logger.LogInformation("Load key store success, address: {Address}", address);
        }
    }

    private void LoadKeyStoreWithInput()
    {
        if (_keyStoreOptions.CurrentValue.Path.IsNullOrEmpty()) return;
        if (_keyStoreOptions.CurrentValue.LoadAddress.IsNullOrEmpty()) return;
        var keyStoreDict = new Dictionary<string, string>();
        foreach (var address in _keyStoreOptions.CurrentValue.LoadAddress)
        {
            // account already exists, skip
            if (_accountHolders.ContainsKey(address)) continue;

            keyStoreDict[address] = ReadKeyStore(address);
        }

        _logger.LogInformation("Waiting for key store decode...");
        Task.Delay(1000);
        Console.WriteLine();
        Console.WriteLine("aaaaaaaaaaaaaaaaaaaaaaaaa Decode Key Store aaaaaaaaaaaaaaaaaaaaaaaaa");
        Console.WriteLine();
        Console.WriteLine(" Key store file path:     ");
        Console.WriteLine("\t" + PathHelper.ResolvePath(_keyStoreOptions.CurrentValue.Path));
        Console.WriteLine();
        Console.WriteLine(" Addresses will be loaded: ");
        foreach (var address in keyStoreDict.Keys)
        {
            Console.WriteLine("\t" + address);
        }

        Console.WriteLine();
        Console.WriteLine("aaaaaaaaaaaaaaaa  Press [Enter] to decode keystore  aaaaaaaaaaaaaaaa");
        Console.WriteLine();

        while (ConsoleKey.Enter != Console.ReadKey(true).Key)
        {
            // do nothing
        }

        foreach (var (address, json) in keyStoreDict)
        {
            AccountHolder account;
            while (!TryDecryptKeyStore(address, json, out account))
            {
            }

            _accountHolders[account.PublicKey] = account;
            _accountHolders[account.AddressObj().ToBase58()] = account;
        }

        _logger.LogInformation("Done!");
    }

    private bool TryDecryptKeyStore(string address, string json, out AccountHolder account)
    {
        try
        {
            /* read password from console */
            var pwd = InputHelper.ReadPassword($"Input password of {address}: ");
            var privateKey = KeyStoreService.DecryptKeyStoreFromJson(pwd, json);
            account = new AccountHolder(privateKey.ToHex());
            Console.WriteLine("Success!");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed!");
            account = null;
            return false;
        }
    }
}