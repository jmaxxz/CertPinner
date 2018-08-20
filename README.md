# CertPinner
[![Build status](https://ci.appveyor.com/api/projects/status/h6a9xeaklghs37nv/branch/master?svg=true)](https://ci.appveyor.com/project/jmaxxz/certpinner/branch/master)

A .net library which provides certificate pinning. Ideal for applications where a CA infrastructure is prohibitive or potentially comprimised.

## Use cases

### Trust on first use (ssh model)
For certain applications and environments it is difficult to role out and maintain a CA based infrastructure. Particularly for applications made up of several non-internet facing devices. Often times individuals deploying these environments are not technically savy or do not have a pki infrastructure. A trust on first use strategy attempts to limit the exposure to network based MiTM attacks to the moment of first contact. After first contact, the public key is remembered and used to validate all future connections.

### Manual pinning
Provides a mechanism to pin certificate at configuration or compilation time. This strategy is ideal when the number of endpoints is minimal and known in advance.


## Included Automatic Pinning Stratagies

Several built in pinning strategies are included in CertPinner. If none of these strategies meet your needs a custom one can be provided by implementing `IAutomaticPinPolicy`. Automatic pinning strategies only affect pinning if no key has yet been pinned for a given host. Stategies below are ordered from most restrictive to least. In general more restrictive strategies should be prefered.

### Never (Default)

The most restrictive option, this option should be self explanitory. If the automatic pinning strategy is set to never no public keys will be automatically pinned. If caller wishes to pin a key it must be done explicity by updating the keystore directly. **IMPORTANT** strategy can be changed at anytime, for example one may wish to run in a less restrictive mode to allow peer discovery during installation or configuration and switch to a more restrictive mode for normal operation. All of the following modes are safer than disabling certificate validation outright. If your application currently disables certificate validation switching using CertPinner with the always strategy will improve the security of your application with minimal risk of breakage.

```csharp
// Usage
CertificatePinner.AutomaticPinPolicy = new NeverAutoPin();
```

### Whitelist

The whitelist strategy only allows pinning hosts which have been explicitly specified. This strategy is recommended when remote public keys are not known at compile time or installation time. If self discovery of remote public keys is acceptable and desired this is the prefered strategy.

```csharp
var whitelist = new WhitelistAutoPin();
CertificatePinner.AutomaticPinPolicy = whitelist;
whitelist.AddToWhitelist("jmaxxz.com");

```

### BlackList

The blacklist strategy allows pinning on all hosts except those which have been explicitly excluded.


### Always

The always trust on first use strategy allows the pinning for all hosts. For applications which make requests to an indeterminately large number of hosts this strategy should be avoid as it will cause the keystore to grow quite large. This strategy is almost never the ideal long term solution. However, it provides a quick upgrade path for applications which may currently disable certificate validation entirely. The always trust on first use policy may be acceptable long term strategy for applications which only communicate with other devices on the lan especially if these other devices make use of self signed certificates.

```csharp
CertificatePinner.AutomaticPinPolicy = new AlwaysAutoPin();
```


## Included Pin Storage Options

Several storage options are provided out by CertPinner. If none of these options meet your needs a custom storage option can be implemented through the IKeyStore interface. Ideally pins should be stored on the local machine in a location which can not be written to by unauthorized users. Any user or system which can modify or delete a pin store can perform a man in the middle attack on the application. Store pins in a safe location! They do not have to be kept secret, but they should be kept safe from unauthorized modification.


### NullKeyStore (For testing only)

The null key store does not actually store any pins. This implementation is provided **only** for testing purposes. Using the null key store disables pinning.
```csharp
CertificatePinner.KeyStore = new NullKeyStore();
```

### InMemoryKeyStore

The InMemoryKeyStore stores all public keys in RAM. If the process is restarted (or crashes) all stored keys will be forgotten. It is not recommend you use this key store in production. However, if you are currently disabling certificate validation all together this keystore is a major improvement. If one uses the InMemoryKeyStore in production it is recommended to implement a means of repopulating the key store on application start.

```csharp
CertificatePinner.KeyStore = new InMemoryKeyStore();
```

### FileSystemKeyStore (Recommended)

Allows user to specify a file path as a single JSON formated file.
```csharp
CertificatePinner.KeyStore = new FileSystemKeyStore(@"%appdata%\MyApp\PinnedKeys.json");
```
