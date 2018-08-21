# CertPinner
[![Build status](https://ci.appveyor.com/api/projects/status/h6a9xeaklghs37nv/branch/master?svg=true)](https://ci.appveyor.com/project/jmaxxz/certpinner/branch/master) 
[![NuGet](https://img.shields.io/nuget/dt/CertPinner.svg)](https://www.nuget.org/packages/CertPinner)


A .net library which provides certificate pinning. Ideal for applications where a CA infrastructure is prohibitive or potentially comprimised.

## Use cases

### Trust on first use (ssh model)
For certain applications and environments it is difficult to role out and maintain a CA based infrastructure. Particularly for applications made up of several non-internet facing devices. Often times individuals deploying these environments are not technically savy or do not have a pki infrastructure. A trust on first use strategy attempts to limit the exposure to network based MiTM attacks to the moment of first contact. After first contact, the public key is remembered and used to validate all future connections.

```Csharp
// At the start of your app configure CertPinner

// The FileSystemKeyStore should be used for production applications
var fileSystemKeyStore = new FileSystemKeyStore(@"%appdata%\MyFirstApp\KeyStore.json");

// Write key store to disk every 5 min
fileSystemKeyStore.AutoSaveInterval = TimeSpan.FromMinutes(5);

// At present one has to manually load the keystore once (autoloading coming soon)
secondInstance.Reload();
CertificatePinner.KeyStore = fileSystemKeyStore;

// Configure auto pin policy. In most trust on first use scenerios a whitelist is ideal
var autoPinWhitelist = new WhitelistAutoPin();

// Add any host you wish to allow Trust On First use for to the white list
// new hosts can be added at any time.
autoPinWhitelist.AddHost("jmaxxz.com");
autoPinWhitelist.AddHost("localhost");
autoPinWhitelist.AddHost("127.0.0.1");
autoPinWhitelist.AddHost("192.168.0.1");

// Enable trust on first use for host on the whitelist
CertificatePinner.AutomaticPinPolicy = autoPinWhitelist;

// If a host for which we have a pin presents a different certificate
// than the one we have pinned, distrust it. In general this is the
// recommended mode of operation.
CertificatePinner.CertificateAuthorityMode = CertificateAuthorityMode.TrustIfNotPinned;

// Enable CertPinner, this only has to be done once per process
CertificatePinner.Enable();
```

### Manual pinning
Provides a mechanism to pin certificate at configuration or compilation time. This strategy is ideal when the number of endpoints is minimal and known in advance.

```Csharp
// At the start of your app configure CertPinner

// When in manul pinning mode the InMemoryKeyStore works well because CertPinner
// will not need to learn/save any new public keys.
CertificatePinner.KeyStore = new InMemoryKeyStore();

// Auto pinning can be completely disabled.
CertificatePinner.AutomaticPinPolicy = new NeverAutoPin();

// Manually pin any public keys by passing the entire public key to the PinForHost function.
CertificatePinner.KeyStore.PinForHost("jmaxxz.com", new byte[] { 0x30, 0x82, 0x01, 0x0a, 0x02, 0x82, 0x01, 0x01, 0x00, 0xd6, 0x7a, 0xc2, 0xcc, 0x46, 0x99, 0xc3, 0x62, 0x3f, 0xe9, 0x88, 0x44, 0x99, 0xcf, 0x47, 0x96, 0xcd, 0x93, 0x78, 0x80, 0x9d, 0x52, 0x2e, 0x55, 0x2b, 0xed, 0x1e, 0xd4, 0x07, 0x90, 0x79, 0x02, 0x11, 0x8c, 0x1e, 0xb4, 0xad, 0x57, 0x01, 0x38, 0x80, 0xe4, 0xc7, 0x5a, 0x28, 0x96, 0x0a, 0xb4, 0x22, 0x45, 0x39, 0x5d, 0x41, 0x77, 0x35, 0xff, 0x91, 0x48, 0xa3, 0x81, 0x21, 0x79, 0x6b, 0xe0, 0x18, 0x39, 0x55, 0x0c, 0xc8, 0xce, 0xac, 0xe4, 0x0b, 0x33, 0x7d, 0xff, 0xc7, 0xf0, 0xb6, 0xe3, 0x6a, 0xbb, 0x73, 0xe8, 0x75, 0xbb, 0xdf, 0x98, 0x7b, 0x7d, 0x49, 0xec, 0x41, 0x04, 0x47, 0xb1, 0x21, 0x9a, 0x80, 0x9b, 0x37, 0x00, 0xe7, 0xb6, 0x4b, 0x02, 0xd1, 0x09, 0x15, 0x8c, 0x34, 0x86, 0xe8, 0xb5, 0xc1, 0x4f, 0x12, 0x6b, 0x9f, 0x29, 0x11, 0x9e, 0xe0, 0x86, 0x5f, 0x05, 0xdd, 0xac, 0x9d, 0xad, 0x03, 0x42, 0xf5, 0x5b, 0x35, 0xde, 0x95, 0xe9, 0xcc, 0xf6, 0x9f, 0xa1, 0x3f, 0xde, 0xb7, 0xb5, 0xb8, 0xa9, 0x27, 0xaf, 0x22, 0xe7, 0xd9, 0xcd, 0xee, 0x6a, 0x33, 0xf7, 0xc5, 0xee, 0xbd, 0x9b, 0xf5, 0xf9, 0xf7, 0x0d, 0x54, 0xb8, 0xe8, 0x81, 0x6c, 0x85, 0x8d, 0x98, 0xb6, 0x93, 0xef, 0xa3, 0x83, 0xb6, 0x9a, 0xf1, 0x2c, 0x3a, 0xf9, 0xfd, 0x51, 0xbe, 0xcd, 0xf7, 0x7e, 0xad, 0x17, 0x31, 0x36, 0x1f, 0x59, 0xb1, 0x1a, 0x0c, 0x58, 0xd0, 0x82, 0x60, 0x5b, 0xbf, 0x15, 0xcd, 0x8f, 0x64, 0x11, 0xc6, 0x34, 0xa5, 0x98, 0x2d, 0xb4, 0x86, 0x04, 0xda, 0xcd, 0xe5, 0x83, 0x6b, 0xd1, 0x64, 0xee, 0x74, 0xd0, 0x24, 0x31, 0xbd, 0x9e, 0x94, 0x9f, 0x38, 0x82, 0x3a, 0x60, 0xf5, 0xdf, 0xc3, 0x24, 0xa1, 0x0a, 0x90, 0xb3, 0x44, 0x2c, 0xce, 0xfa, 0x2f, 0x61, 0xda, 0xda, 0x33, 0x02, 0x03, 0x01, 0x00, 0x01});

// If a host for which we have a pin presents a different certificate
// than the one we have pinned, distrust it. In general this is the
// recommended mode of operation.
CertificatePinner.CertificateAuthorityMode = CertificateAuthorityMode.TrustIfNotPinned;

// Enable CertPinner, this only has to be done once per process
CertificatePinner.Enable();
```


## Included Automatic Pinning Stratagies

Several built in pinning strategies are included in CertPinner. If none of these strategies meet your needs a custom one can be provided by implementing `IAutomaticPinPolicy`. Automatic pinning strategies only affect pinning if no key has yet been pinned for a given host. Stategies below are ordered from most restrictive to least. In general more restrictive strategies should be prefered.

 **IMPORTANT** strategy can be changed at anytime, for example one may wish to run in a less restrictive mode to allow peer discovery during installation or configuration and switch to a more restrictive mode for normal operation. All of the following modes are safer than disabling certificate validation outright. If your application currently disables certificate validation switching using CertPinner with the always strategy will improve the security of your application with minimal risk of breakage.

### Never (Default)

The most restrictive option, this option should be self explanitory. If the automatic pinning strategy is set to never no public keys will be automatically pinned. If caller wishes to pin a key it must be done explicity by updating the keystore directly.

```csharp
CertificatePinner.AutomaticPinPolicy = new NeverAutoPin();
// No host will have its certificate pinned on first use.
```

### Whitelist

The whitelist strategy only allows pinning hosts which have been explicitly specified. This strategy is recommended when remote public keys are not known at compile time or installation time. If self discovery of remote public keys is acceptable and desired this is the prefered strategy.

```csharp
var whitelist = new WhitelistAutoPin();
CertificatePinner.AutomaticPinPolicy = whitelist;
whitelist.AddHost("jmaxxz.com");
// jmaxxz.com is the only host allowed to be pinned
```

### BlackList

The blacklist strategy allows pinning on all hosts except those which have been explicitly excluded.
```csharp
var blacklist = new BlacklistAutoPin();
CertificatePinner.AutomaticPinPolicy = blacklist;
blacklist.AddHost("jmaxxz.com");
// jmaxxz.com will never have its certificate pinned
```

### Always

The always trust on first use strategy allows the pinning for all hosts. For applications which make requests to an indeterminately large number of hosts this strategy should be avoid as it will cause the keystore to grow quite large. This strategy is almost never the ideal long term solution. However, it provides a quick upgrade path for applications which may currently disable certificate validation entirely. The always trust on first use policy may be acceptable long term strategy for applications which only communicate with other devices on the lan especially if these other devices make use of self signed certificates.

```csharp
CertificatePinner.AutomaticPinPolicy = new AlwaysAutoPin();
// Every host will have its certificate pinned on the first request
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
