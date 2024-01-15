using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SecurityServer.Common;
using SecurityServer.Dtos;
using SecurityServer.Options;
using SecurityServer.Providers.ExecuteStrategy;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SecurityServer.Providers;

public class StorageProvider : ISingletonDependency
{
    private readonly ILogger<StorageProvider> _logger;
    private readonly IOptionsMonitor<KeyStoreOptions> _keyStoreOptions;
    private readonly Dictionary<string, string> _secretStorage = new();

    public StorageProvider(IOptionsMonitor<KeyStoreOptions> keyStoreOptions, ILogger<StorageProvider> logger)
    {
        _keyStoreOptions = keyStoreOptions;
        _logger = logger;
        LoadEncryptDataWithProvidedPassword();
        LoadKeyStoreWithPassword();
    }

    private void AssertKeyMod(string key, Mod checkMod)
    {
        AssertHelper.IsTrue(_keyStoreOptions.CurrentValue.ThirdPart.TryGetValue(key, out var keyOption), "Secret of key {} not found", key);
        AssertHelper.NotNull(keyOption, "Option of key {} empty", key);
        
        AssertHelper.NotEmpty(keyOption!.Mod, "Permission denied");
        AssertHelper.IsTrue(keyOption.Mod.ToUpper().Contains(checkMod.ToString().ToUpper()), "Permission denied");
    }

    /// <summary>
    ///     Query readable keys
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public string GetThirdPartSecret(string key)
    {
        AssertKeyMod(key, Mod.R);
        AssertHelper.IsTrue(_secretStorage.TryGetValue(key, out var secret), "Secret of key {} not found", key);
        AssertHelper.NotEmpty(secret, "Secret of key {} empty", key);
        return secret!;
    }

    /// <summary>
    ///     Select custom policy to perform key calculation strategy
    /// </summary>
    /// <param name="key"></param>
    /// <param name="executeInput"></param>
    /// <param name="strategy"></param>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public TOutput ExecuteThirdPartSecret<TInput, TOutput>(TInput executeInput,
        IThirdPartExecuteStrategy<TInput, TOutput> strategy)
        where TInput : BaseThirdPartExecuteInput
    {
        AssertKeyMod(executeInput.Key, Mod.X);
        AssertHelper.IsTrue(_secretStorage.TryGetValue(executeInput.Key, out var secret), "Secret of key {} not found",
            executeInput.Key);
        AssertHelper.NotEmpty(secret, "Secret of key {} empty", executeInput.Key);
        return strategy.Execute(secret, executeInput);
    }

    private void LoadEncryptDataWithProvidedPassword()
    {
        if (_keyStoreOptions.CurrentValue.Path.IsNullOrEmpty()) return;
        if (_keyStoreOptions.CurrentValue.ThirdPart.IsNullOrEmpty()) return;

        // Decrypt with password provided in Option
        // Enter password to decrypt
        foreach (var (key, config) in _keyStoreOptions.CurrentValue.ThirdPart)
        {
            if (config.Password.IsNullOrEmpty()) continue;
            var file = $"/{key}.json";
            var path = PathHelper.ResolvePath(_keyStoreOptions.CurrentValue.Path + file);
            var keyStoreContent = InputHelper.ReadFile(path);
            var encrypt = JsonConvert.DeserializeObject<EncryptDataDto>(keyStoreContent);
            var decryptSuccess = encrypt!.TryDecrypt(config.Password, out var secretData, out var msg);
            if (!decryptSuccess)
            {
                _logger.LogWarning("Decrypt ThirdPart apikey data {Key}.json failed: {Message}", key, msg);
                continue;
            }

            _secretStorage[key] = secretData;
            _logger.LogWarning(
                "Decrypt ThirdPart apikey data {File} with provided password, Information:{EncryptInformation}",
                file, encrypt.Information);
        }
    }

    private void LoadKeyStoreWithPassword()
    {
        if (_keyStoreOptions.CurrentValue.Path.IsNullOrEmpty()) return;
        if (_keyStoreOptions.CurrentValue.ThirdPart.IsNullOrEmpty()) return;

        var jsonContents = new Dictionary<string, EncryptDataDto>();
        foreach (var (key, _) in _keyStoreOptions.CurrentValue.ThirdPart)
        {
            if (_secretStorage.ContainsKey(key)) continue;
            var path = PathHelper.ResolvePath(_keyStoreOptions.CurrentValue.Path + $"/{key}.json");
            var keyStoreContent = InputHelper.ReadFile(path);
            var encrypt = JsonConvert.DeserializeObject<EncryptDataDto>(keyStoreContent);
            jsonContents[key] = encrypt;
        }

        if (jsonContents.IsNullOrEmpty()) return;

        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("aaaaaaaaaaaaaaaaaaaaaa Decode ThirdPart apikey aaaaaaaaaaaaaaaaaaaaa");
        Console.WriteLine();
        Console.WriteLine("  Json files will be loaded: ");
        foreach (var (key, encryptData) in jsonContents)
        {
            var file = $"{key}.json";
            var info = encryptData.Information.IsNullOrEmpty() ? "(no info)" : encryptData.Information;
            Console.WriteLine($"     {file} - {info} ");
        }

        Console.WriteLine();
        Console.WriteLine("aaaaaaaaaaaaaaaa  Press [Enter] to decode keystore  aaaaaaaaaaaaaaaa");
        while (ConsoleKey.Enter != Console.ReadKey(true).Key)
        {
            // do nothing
        }

        foreach (var encryptData in jsonContents.Values)
        {
            var inputLabel = $"Input password of {encryptData.Key}.json [Hidden]: ";
            string secretData;
            while (!encryptData.TryDecrypt(InputHelper.ReadPassword(inputLabel), out secretData, out var msg))
            {
                Console.WriteLine($"Failed: {msg}");
            }

            _secretStorage[encryptData.Key] = secretData;
            Console.WriteLine("Success!");
        }

        Console.WriteLine();
    }
}