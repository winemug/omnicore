namespace OmniCore.Common.Api;

public class ApiConstants
{
    public const string RootCertificate = """
                                          -----BEGIN CERTIFICATE-----
                                          MIIFCzCCAvOgAwIBAgIUUneQeeOskxRQ/7X4f6o38k6PplYwDQYJKoZIhvcNAQEL
                                          BQAwFDESMBAGA1UEAwwJYmFseWEubmV0MCAXDTI0MDUwNTIyMzIxN1oYDzIxMjQw
                                          NDExMjIzMjE3WjAUMRIwEAYDVQQDDAliYWx5YS5uZXQwggIiMA0GCSqGSIb3DQEB
                                          AQUAA4ICDwAwggIKAoICAQCkxA0ZKlxmg5HkAuIiiXAMQnS2ErHAgxByyrLb7Yjz
                                          6afcdVa5s7KbO4HDKBsbJ+ttCh3hR5u8U1YASje46wgXrOVnkb/tNWOoJy3zMe7p
                                          lfKGk+EcrXRb+0+898fsq7DKIvXMVTHauE3pQVljR15raYY2VEMjn2RHp9RmG0C5
                                          j5Vs4CUge5uU/+PxNsPZN+qKr89K8oIXFaT1eQjUI7JlQUY3dFALM4XYTt1Duqfs
                                          xMBq3VZfCGejwErZCSgq+4wvS9ZNwIl3dirF38MI5F3KtoS/sJDKzSZyAOV2oudO
                                          H2PKIG5noGyRcYtBGjRpXY8EoMckCaKHsRutg4fvA5uCHaJNvYtqZj36goFVSTtb
                                          LfYb+fuMLB2BgJnarLJTv4RV0kwPTjepU166UOnoOIxFjqZ/LCDnPSsJDANU7NAt
                                          A4nPEfELg9BizTGO3AvmNUBsePjMoWqNiZHG//XRS5lxcUZoicbHz4l8qeddRpsw
                                          N/vHohB53Z9zYAJKaNjFNoN3Jt10oZOr1669IJzZ7Vk9lMoL2vABSDrKJOUG4tLv
                                          vsv0RjWUtIfWiJiMAm1jO7mwDR+1JBijNUa27nSV3uL6+J158Z4StkjKrLUWKD4K
                                          o+9SJzuM1Jk81tZSbdMkzPoS35xgdVLgynSv5K+kwnBb06npgJqxS+Ue+SsI33/X
                                          XwIDAQABo1MwUTAdBgNVHQ4EFgQU9Xotoy8RRnbtty349loxRh784UkwHwYDVR0j
                                          BBgwFoAU9Xotoy8RRnbtty349loxRh784UkwDwYDVR0TAQH/BAUwAwEB/zANBgkq
                                          hkiG9w0BAQsFAAOCAgEAEFnPXd7N/L0mgMj07g9e47SB+VSLJj4JQBGufcJJwd+8
                                          KOkbBQjGiwbFpRMuqq3wdrsSjvZrsu2AsEla3rQS8PUOpOtSm8bJ4lz0V9HTlJLX
                                          N1zkUVMn05Gq33iMlTpbIXrQQlbPGweO0vfBi0irkSJRL3aiQuBXANwsH/wLUK67
                                          T6aX/qQ6Jy37jiNYXfnFrFkR3JeAlxqMkCicCe9JiG2w0JDxu89sNFpM1u7IIPQX
                                          koFSyC/Bs5xfp+LRdbs5DNe0P+4FY990xKjli1spCJguKX/Uk1snySlqpzWLJ8ej
                                          DsfUx9e8KapP9ouQZtsYB8tbsOmjKqd4SEAk3l5MdEjjbmIaBzvnXo/y0vQyYOOG
                                          hmP051sQsWqHHCygA+ZKGjxWFy7JZsO7OKHA/mKpZS1fXwri6jv6eQ8MNJZ/FYWE
                                          qmk5tpnY7S45TS9RYbA3E/u+RSHceCO09Gak11W16C8CMrmmsPh1vyCyEaLPuKPa
                                          LR3MkMoHIaebobuCm2nj5Ucnj5sHMUZm9GS7xdvWloHRszqmhTi7J2Euq2NeJC4G
                                          JSvUMtAUkmlBJYRB2WWJMTPQXoSsKYewSZOXZg8kN6N5ivT2+Jakgjay+FGC73QT
                                          S5Yn11/XVpPWNlsqTERPsUCkJqWEKGmz5gBsY8eqPsNvABC+TF1vUhWI/H40hLg=
                                          -----END CERTIFICATE-----
                                          """;

    public const string CaCertificate = """
                                        -----BEGIN CERTIFICATE-----
                                        MIIFGzCCAwOgAwIBAgIUCT376zDbfbKA6RsW9xvYU8alqUgwDQYJKoZIhvcNAQEL
                                        BQAwFDESMBAGA1UEAwwJYmFseWEubmV0MB4XDTI0MDUwNTIyMzIxOVoXDTM0MDUw
                                        MzIyMzIxOVowFjEUMBIGA1UEAwwLT21uaWNvcmUgQ0EwggIiMA0GCSqGSIb3DQEB
                                        AQUAA4ICDwAwggIKAoICAQCxe5cz3QcxjWLIhBXIjwTm5UzBH4S7SE70/h5c7htL
                                        Uv1MA/NyF4ZsDU8Lkf37tGNVlvcCUQP8EXJeZc8KWOI5TVxwWP0AiOeNHh5H+JzP
                                        Ht5XVyJD+KylSr5u3YhM1AeCiTz6Rpc/ipe/dgpRe3NGQlhjudDHGmmfEFSu3Oti
                                        JuZhHukrqy1ab/5PHj5TxZ8mPb0dqeUrhj7z/V1osVyYRF0ip948iQZDZhBUdu2x
                                        0rdEHOtpcIBgDJnLBYfl4YB59/Nnb8cQ1zqbDIRj0Gp+vW2a92NQhoHk9w8zqeOX
                                        /ZgAsAOUMSNcEKNhFGuWlkO1X9Xe/T7/NkBlGEmwOvuIE6+6AlxVfLbJ9oFgDSbr
                                        oETfLJHV18M96GMmCS4WuuBjab6/O7gNx5YaQRxO8w36n/LsTM+wGEgODpmuNxJC
                                        tGSDbLKtfP120R0u+uQLPz4tYaQCuw32U3tqBuQ8Wkaz6gHYSDPaBbDRmr3YDnyF
                                        oEdYtEzjfLS25DwU6EDTfu6+9ZITItLgz89RqQpRarI4HCWQeeXStSmZgA5eJyaG
                                        ciGSXVLs/V9sbOvrXZW96t+0b/Z2B8sNe98o8PZJ9dg8exaxc/XdIVaIjmMPnztm
                                        VrqYpK6iKsr5qtiHBmmWjE4s7DkDfnqO0n/updqL+jPRXTXWwZwW1li0a12gc+M4
                                        WwIDAQABo2MwYTAdBgNVHQ4EFgQUf9CvROGRskPaAUACG/A0gTlGPtgwHwYDVR0j
                                        BBgwFoAU9Xotoy8RRnbtty349loxRh784UkwEgYDVR0TAQH/BAgwBgEB/wIBATAL
                                        BgNVHQ8EBAMCAgQwDQYJKoZIhvcNAQELBQADggIBAIEMZuaxtEZ+kBDT/K41amZ3
                                        KbDmSiuyi4k0GCGD++ikq1VWddQRFIKzDemoVuVoc550gogzyz43kKWEL6M7LWJw
                                        rDN2a0CMkFOwrd+nlBToog9xcwfu2smjAYLe7zgQ3i2YlG9PLOA3cgvi1NgYhdd1
                                        6Kl4k7J65uiviRhWT1JQ3RebWMPYyYRmRe+mPLY+wUN70dASkBuI/fkFg0HlWII5
                                        9b0Z/PX0zmjxY4UdDtwTpp0BCv1wtm3852HZuNRz5e1MUggmdNgzlfIgInlOSL0m
                                        pxMKP1mRHe7ZGdXlQx3rAgG7B3LkognBIDgMdKIU9AbgCNjAfjbSoWNIHtNJHbtv
                                        GewRlu5x0qlVh6yLHdOPUoRIXrGFmh4hwS9jntj9h0xSiLHRXBHn5VDOfWRd7Xtw
                                        HwfLOw6J87KW+2/hd9P8Lcywrlx/YpBiQ29cF0ROuTL/YKWF4Efq8P2gegcse6hN
                                        WhnkHFdlGAkHSa/HtaDl3gZFY6xdnLh/B9/LT8B6+CeRVTEiv0hol6BXVxHTGMsw
                                        kQ0CbmzD3pZf6bTyu2+Vz7pYzCApqWmgMBcptSocBQcPLALB/LNzgK+wWtifvzns
                                        4HODyd7Bd6DDgMzP+1Nvmq2S2Jy81VB3F4Y3hddkX61zZyoTFdQ1KKm/uYK+Kk1R
                                        4EhZGTxClUtjWry3sroy
                                        -----END CERTIFICATE-----
                                        """;
}