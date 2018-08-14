# CertPinner
A .net library which provides certificate pinning. Ideal for applications where a CA infrastructure is prohibitive or potentially comprimised.

## Use cases

### Trust on first use (ssh model)
For certain applications and environments it is difficult to role out and maintain a CA based infrastructure. Particularly for applications made up of several non-internet facing devices. Often times individuals deploying these environments are not technically savy or do not have a pki infrastructure. A trust on first use strategy attempts to limit the exposure to network based MiTM attacks to the moment of first contact. After first contact, the public key is remembered and used to validate all future connections.

### Manual pinning
Provides a mechanism to pin certificate at configuration or compilation time. This strategy is ideal when the number of endpoints is minimal and known in advance.
