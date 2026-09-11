# CORS on Auth STG (`ca-corelux-auth-stg`)

Auth binds `Cors:AllowedOrigins` from JSON (`appsettings.Production.json`) and from environment variables. Indexed keys override the **same JSON index**:

| App setting | Maps to |
| --- | --- |
| `Cors__AllowedOrigins__0` | `Cors:AllowedOrigins:0` |
| `Cors__AllowedOrigins__1` | `Cors:AllowedOrigins:1` |
| `Cors__AllowedOrigins` | scalar / comma-separated list (also accepted) |

## Before this change (live STG)

| Origin | OPTIONS | POST `/api/AppUser/login` `{}` |
| --- | --- | --- |
| `https://www.temporasv.com` | 204 + `Access-Control-Allow-Origin` | 422 + Allow-Origin |
| `https://temporasv.com` (apex) | 204 **no CORS headers** | 422 **no Allow-Origin** (browser: “can’t connect”) |
| `https://corelux-tempora-stg.pages.dev` | OK | 422 + Allow-Origin |

`www` is already allowed (ACA env and/or file). Apex was missing from `DefaultRemoteOrigins`, so an env list that only has www never echoes CORS for QA on `temporasv.com`.

## After (image)

`DefaultRemoteOrigins` always includes both `https://temporasv.com` and `https://www.temporasv.com` (Pages hosts unchanged). Empty login POST stays **422**; apex gets `Access-Control-Allow-Origin: https://temporasv.com`.

## ACA STG — pick up apex without waiting only on a new image

On **`ca-corelux-auth-stg` only** (do **not** change production ACA), add apex. If `__0` is already `https://www.temporasv.com`, use the next free index:

```
Cors__AllowedOrigins__0=https://temporasv.com
Cors__AllowedOrigins__1=https://www.temporasv.com
```

Or one setting:

```
Cors__AllowedOrigins=https://temporasv.com,https://www.temporasv.com
```

If other `__N` values already exist, **append** apex as the next unused `N` instead of wiping the list. Restart the Container App revision after saving.
