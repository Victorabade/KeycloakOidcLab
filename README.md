# keycloak-oidc-lab

Laboratório de estudo de SSO / OIDC com **Keycloak**, **.NET 8** e **Vue 3**.

## Estrutura

```
├── backend/          # API .NET com autenticação JwtBearer
├── frontend/         # App Vue 3 com oidc-client-ts
├── docker/           # Docker Compose do Keycloak
└── ...
```

## Stack

| Componente | Tecnologia |
|------------|-----------|
| Provedor OIDC | Keycloak 26 |
| Backend API | .NET 8 |
| Frontend | Vue 3 + oidc-client-ts |

## Fluxo

Authorization Code Flow com PKCE.

## Referência

Guia detalhado em [`sso-guide.md`](./sso-guide.md) (se disponível).
