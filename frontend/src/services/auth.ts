import { UserManager, WebStorageStateStore } from 'oidc-client-ts'

const userManager = new UserManager({
  authority: 'http://localhost:8080/realms/oidc-lab',
  client_id: 'oidc-lab-vue',
  redirect_uri: 'http://localhost:5173/auth-callback',
  post_logout_redirect_uri: 'http://localhost:5173',
  response_type: 'code',
  scope: 'openid profile email',
  userStore: new WebStorageStateStore({ store: window.sessionStorage }),
  automaticSilentRenew: true,
})

console.log(
  '%c[oidc] %cUserManager inicializado',
  'font-weight:bold;color:#818cf8',
  'color:#94a3b8',
  {
    authority: 'http://localhost:8080/realms/oidc-lab',
    client_id: 'oidc-lab-vue',
    flow: 'Authorization Code + PKCE',
    scope: 'openid profile email',
    storage: 'sessionStorage',
  },
)

function logToken(label: string, token: string | undefined) {
  if (!token) {
    console.log(`  ${label}: (ausente)`)
    return
  }
  const parts = token.split('.')
  let payload: any = {}
  if (parts.length === 3 && parts[1]) {
    try {
      const raw = atob(parts[1])
      payload = JSON.parse(raw)
    } catch {}
  }
  console.log(`  ${label}:`)
  console.log(`    raw:    ${token.substring(0, 40)}... (${token.length} chars)`)
  if (payload.exp) {
    const exp = new Date(payload.exp * 1000)
    console.log(`    exp:    ${exp.toLocaleString()} (${payload.exp})`)
  }
  if (payload.iat) {
    const iat = new Date(payload.iat * 1000)
    console.log(`    iat:    ${iat.toLocaleString()} (${payload.iat})`)
  }
  console.log(`    header: ${JSON.stringify(payload.iss ? { iss: payload.iss, sub: payload.sub, aud: payload.aud, azp: payload.azp, typ: payload.typ } : payload)}`)
}

userManager.events.addUserLoaded((user) => {
  console.groupCollapsed(
    '%c[oidc] %cUserLoaded',
    'font-weight:bold;color:#818cf8',
    'font-weight:bold;color:#34d399',
  )
  console.log('profile:', user.profile)
  logToken('access_token', user.access_token)
  logToken('refresh_token', user.refresh_token)
  logToken('id_token', user.id_token)
  console.log('expired:', user.expired)
  if (user.expires_at) {
    console.log('expires_at:', new Date(user.expires_at * 1000).toLocaleString())
  }
  console.groupEnd()
})

userManager.events.addUserUnloaded(() => {
  console.log(
    '%c[oidc] %cUserUnloaded %c-- sessao limpa',
    'font-weight:bold;color:#818cf8',
    'font-weight:bold;color:#f87171',
    'color:#94a3b8',
  )
})

userManager.events.addAccessTokenExpiring(() => {
  console.warn(
    '%c[oidc] %cAccessTokenExpiring %c-- tentando renovar...',
    'font-weight:bold;color:#818cf8',
    'font-weight:bold;color:#fbbf24',
    'color:#94a3b8',
  )
})

userManager.events.addAccessTokenExpired(() => {
  console.error(
    '%c[oidc] %cAccessTokenExpired %c-- token expirado, renovacao falhou',
    'font-weight:bold;color:#818cf8',
    'font-weight:bold;color:#ef4444',
    'color:#94a3b8',
  )
})

userManager.events.addSilentRenewError((error) => {
  console.error(
    '%c[oidc] %cSilentRenewError',
    'font-weight:bold;color:#818cf8',
    'font-weight:bold;color:#f59e0b',
    error,
  )
})

export default userManager
