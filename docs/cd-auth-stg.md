# CD Auth → Azure Container Apps STG

Push a `main` construye la imagen, la publica en GHCR y **despliega** esa revisión en `ca-corelux-auth-stg`. Un CI verde en un PR (o un push que solo hace build/push) **no** actualiza ACA por sí solo: hasta este workflow, STG se quedaba en la imagen anterior hasta una revisión manual.

## Flujo

| Evento | `test` | `build` (GHCR) | `deploy-stg` |
| --- | --- | --- | --- |
| Pull request | sí | build, **sin** push | no |
| Push / `workflow_dispatch` en `main` | sí | push `:latest` y `:${{ github.sha }}` | sí, si test+build OK |

El job `deploy-stg` usa `azure/login@v2` con **OIDC** (federated credentials). No hay client secret de Azure en el repo ni en el YAML.

```text
main → test + build/push GHCR → az login (OIDC) → az containerapp update --image …:${{ github.sha }}
```

Recurso STG (inventario; este repo no tiene `Deploy.md`):

- Container App: `ca-corelux-auth-stg`
- Resource group: `rg-corelux-stg`
- Imagen: `ghcr.io/cristanpalma2203/corelux-auth:<git-sha>`
- GitHub Environment: `stg`

## Secretos de GitHub

Crear el environment **`stg`** en el repo (`Settings → Environments`) **sin** required reviewers si el deploy debe ser automático. Añadir estos secretos (en el environment `stg` o a nivel repo):

| Secreto | Uso |
| --- | --- |
| `AZURE_CLIENT_ID` | Application (client) ID de la app Entra usada por OIDC |
| `AZURE_TENANT_ID` | Directory (tenant) ID |
| `AZURE_SUBSCRIPTION_ID` | Subscription donde vive `rg-corelux-stg` |

No se necesita `AZURE_CLIENT_SECRET`. No hay variables de GitHub obligatorias: nombre de app y RG van fijos en el workflow.

El job solo pide `id-token: write` (OIDC) y `contents: read`. El push a GHCR sigue usando `GITHUB_TOKEN` + `packages: write` en `build`.

## App Entra + federated credential

1. Entra ID → App registrations → app (o crear una) usada solo para GitHub Actions.
2. **Certificates & secrets → Federated credentials → Add credential → GitHub Actions deploying Azure resources.**
3. Valores:

   | Campo | Valor |
   | --- | --- |
   | Organization | `CristanPalma2203` |
   | Repository | `Auth` |
   | Entity | **Environment** |
   | GitHub environment name | `stg` |
   | Audience | `api://AzureADTokenExchange` (default) |

   Subject claim resultante:

   `repo:CristanPalma2203/Auth:environment:stg`

4. Si se prefiere no usar environment de GitHub (no es el caso de este workflow), la alternativa es Entity = **Branch** / `main`:

   `repo:CristanPalma2203/Auth:ref:refs/heads/main`

   El subject tiene que coincidir **exactamente** con lo que emite Actions. Con `environment: stg` en el job, hace falta el credential de environment (el de branch no alcanza).

5. RBAC en Azure (la app, no un usuario):

   ```bash
   az role assignment create \
     --assignee "<AZURE_CLIENT_ID>" \
     --role "Contributor" \
     --scope "/subscriptions/<AZURE_SUBSCRIPTION_ID>/resourceGroups/rg-corelux-stg"
   ```

   Mínimo útil: `Microsoft.App/containerApps/write` sobre `ca-corelux-auth-stg` (p. ej. rol **Container Apps Contributor** en esa app).

## Pull de GHCR desde ACA

El paquete `ghcr.io/cristanpalma2203/corelux-auth` está **público**. ACA puede hacer pull sin credenciales de registry.

Si el paquete pasa a privado, configurar auth en la Container App (PAT de GitHub con `read:packages`, no el `GITHUB_TOKEN` del workflow):

```bash
az containerapp registry set \
  --name ca-corelux-auth-stg \
  --resource-group rg-corelux-stg \
  --server ghcr.io \
  --username CristanPalma2203 \
  --password "<GITHUB_PAT_read_packages>"
```

No hace falta ACR para este flujo. No commitear el PAT.

## Rollback

Las revisiones anteriores de ACA se conservan. Para volver a una imagen conocida (el SHA del commit previo en GHCR):

```bash
az containerapp update \
  --name ca-corelux-auth-stg \
  --resource-group rg-corelux-stg \
  --image ghcr.io/cristanpalma2203/corelux-auth:<sha-anterior>
```

Listar revisiones / tags:

```bash
az containerapp revision list \
  --name ca-corelux-auth-stg \
  --resource-group rg-corelux-stg \
  --query "[].{name:name,active:properties.active,created:properties.createdTime}" \
  -o table

# Tags publicados
# https://github.com/CristanPalma2203/Auth/pkgs/container/corelux-auth
```
