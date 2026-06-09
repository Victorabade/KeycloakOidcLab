<script setup lang="ts">
import { ref, onMounted } from 'vue'
import userManager from '../services/auth'

const user = ref<any>(null)
const apiResult = ref<any>(null)
const loading = ref(false)

const bffUser = ref<any>(null)
const bffApiResult = ref<any>(null)
const bffLoading = ref(false)

onMounted(async () => {
  console.groupCollapsed(
    '%c[home] %conMounted -- carregando usuario do storage',
    'font-weight:bold;color:#818cf8',
    'color:#94a3b8',
  )
  const usuario = await userManager.getUser()
  if (usuario && !usuario.expired) {
    user.value = usuario.profile
    console.log('status: usuario SPA logado')
    console.log('profile:', usuario.profile)
  } else {
    console.log('status: nenhum usuario SPA logado')
  }
  console.groupEnd()

  // --- Verifica sessão BFF ---
  try {
    const res = await fetch('http://localhost:5000/api/me', { credentials: 'include' })
    if (res.ok) {
      bffUser.value = await res.json()
      console.log('[home] sessao BFF ativa:', bffUser.value)
    }
  } catch {
    // sem sessão BFF
  }
})

async function login() {
  console.log('%c[home] %clogin() -- redirecionando para Keycloak',
    'font-weight:bold;color:#818cf8', 'color:#94a3b8',
  )
  await userManager.signinRedirect()
}

async function logout() {
  console.log('%c[home] %clogout() -- redirecionando para end session',
    'font-weight:bold;color:#818cf8', 'color:#94a3b8',
  )
  await userManager.signoutRedirect()
  user.value = null
  apiResult.value = null
}

async function chamarApi() {
  console.groupCollapsed(
    '%c[home] %cchamarApi()',
    'font-weight:bold;color:#818cf8',
    'color:#94a3b8',
  )
  const usuario = await userManager.getUser()
  if (!usuario || usuario.expired) {
    console.warn('Nenhum usuario logado ou token expirado')
    console.groupEnd()
    alert('Faca login primeiro')
    return
  }

  const token = usuario.access_token
  console.log(`access_token: ${token.substring(0, 40)}... (${token.length} chars)`)

  loading.value = true
  try {
    console.log('fetch: GET http://localhost:5000/api/me')
    const res = await fetch('http://localhost:5000/api/me', {
      headers: { Authorization: `Bearer ${token}` },
    })

    console.log(`response: ${res.status} ${res.statusText}`)
    if (res.ok) {
      const data = await res.json()
      apiResult.value = data
      console.log('body:', data)
    } else {
      const text = await res.text()
      console.error('erro body:', text)
      alert('Erro: ' + text)
    }
  } finally {
    loading.value = false
    console.groupEnd()
  }
}

function loginBff() {
  console.log('[home] loginBff() -- redirecionando para BFF')
  window.location.href = 'http://localhost:5000/login-bff'
}

async function logoutBff() {
  console.log('[home] logoutBff() -- redirecionando para logout BFF')
  window.location.href = 'http://localhost:5000/logout-bff'
  bffUser.value = null
  bffApiResult.value = null
}

async function chamarApiBff() {
  console.groupCollapsed('[home] chamarApiBff()', 'font-weight:bold;color:#818cf8', 'color:#94a3b8')
  bffLoading.value = true
  try {
    const res = await fetch('http://localhost:5000/api/me', { credentials: 'include' })
    console.log('response:', res.status, res.statusText)
    if (res.ok) {
      const data = await res.json()
      bffApiResult.value = data
      console.log('body:', data)
    } else {
      const text = await res.text()
      console.error('erro body:', text)
      alert('Erro: ' + text)
    }
  } finally {
    bffLoading.value = false
    console.groupEnd()
  }
}
</script>

<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
    <!-- Navbar -->
    <header v-if="user || bffUser" class="border-b border-white/10 bg-white/5 backdrop-blur-md">
      <div class="mx-auto flex max-w-5xl items-center justify-between px-6 py-3">
        <div class="flex items-center gap-3">
          <div class="flex h-8 w-8 items-center justify-center rounded-lg bg-indigo-500 text-sm font-bold text-white">
            S
          </div>
          <span class="font-semibold text-white">SSO Lab</span>
        </div>
        <div class="flex items-center gap-4">
          <div class="hidden items-center gap-2 text-sm text-slate-300 sm:flex">
            <div class="h-2 w-2 rounded-full bg-emerald-400"></div>
            <span class="truncate max-w-[180px]">{{ user?.name || user?.preferred_username || bffUser?.Nome }}</span>
          </div>
          <button
            v-if="user"
            @click="logout"
            class="cursor-pointer rounded-lg border border-white/10 bg-white/5 px-4 py-2 text-sm font-medium text-slate-200 transition hover:bg-white/10 hover:text-white"
          >
            Sair (SPA)
          </button>
          <button
            v-if="bffUser"
            @click="logoutBff"
            class="cursor-pointer rounded-lg border border-white/10 bg-white/5 px-4 py-2 text-sm font-medium text-slate-200 transition hover:bg-white/10 hover:text-white"
          >
            Sair (BFF)
          </button>
        </div>
      </div>
    </header>

    <!-- Not logged in (neither SPA nor BFF) -->
    <div v-if="!user && !bffUser" class="flex min-h-screen items-center justify-center px-4">
      <div class="w-full max-w-md space-y-8 text-center">
        <div class="space-y-2">
          <div class="mx-auto flex h-16 w-16 items-center justify-center rounded-2xl bg-indigo-500/20 ring-1 ring-indigo-500/30">
            <svg class="h-8 w-8 text-indigo-400" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" d="M16.5 10.5V6.75a4.5 4.5 0 10-9 0v3.75m-.75 11.25h10.5a2.25 2.25 0 002.25-2.25v-6.75a2.25 2.25 0 00-2.25-2.25H6.75a2.25 2.25 0 00-2.25 2.25v6.75a2.25 2.25 0 002.25 2.25z" />
            </svg>
          </div>
          <h1 class="text-3xl font-bold tracking-tight text-white">SSO Lab</h1>
          <p class="text-sm text-slate-400">Keycloak + .NET + Vue 3</p>
        </div>

        <div class="rounded-2xl border border-white/10 bg-white/5 p-8 backdrop-blur-sm">
          <p class="mb-6 text-sm leading-relaxed text-slate-300">
            Authorization Code Flow com PKCE usando Keycloak como provedor OIDC.
          </p>
          <button
            @click="login"
            class="inline-flex cursor-pointer items-center gap-2 rounded-xl bg-indigo-500 px-6 py-3 text-sm font-semibold text-white shadow-lg shadow-indigo-500/25 transition hover:bg-indigo-400 active:scale-95"
          >
            <svg class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" d="M15.75 9V5.25A2.25 2.25 0 0013.5 3h-6a2.25 2.25 0 00-2.25 2.25v13.5A2.25 2.25 0 007.5 21h6a2.25 2.25 0 002.25-2.25V15m3 0l3-3m0 0l-3-3m3 3H9" />
            </svg>
            Entrar com Keycloak (SPA)
          </button>

          <button
            @click="loginBff"
            class="mt-3 inline-flex cursor-pointer items-center gap-2 rounded-xl border border-indigo-400/30 bg-white/5 px-6 py-3 text-sm font-semibold text-indigo-300 transition hover:bg-indigo-500/10 hover:border-indigo-400/50 active:scale-95"
          >
            Entrar via BFF
          </button>
        </div>

        <p class="text-xs text-slate-600">
          Usuario de teste: <code class="rounded bg-white/5 px-1.5 py-0.5 text-slate-400">teste / 123456</code>
        </p>
      </div>
    </div>

    <!-- Logged in: Dashboard -->
    <main v-else class="mx-auto max-w-5xl space-y-8 px-6 py-10">
      <!-- SPA Dashboard -->
      <div v-if="user" class="rounded-2xl border border-white/10 bg-white/5 p-6 backdrop-blur-sm sm:p-8">
        <div class="mb-4 flex items-center gap-2">
          <span class="rounded-lg bg-emerald-500/20 px-2 py-0.5 text-xs font-semibold text-emerald-300">SPA</span>
          <span class="text-xs text-slate-500">Authorization Code + PKCE</span>
        </div>
        <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div class="flex items-center gap-4">
            <div class="flex h-12 w-12 items-center justify-center rounded-xl bg-indigo-500/20 text-lg font-bold text-indigo-300">
              {{ (user.name || user.preferred_username || '?').charAt(0).toUpperCase() }}
            </div>
            <div>
              <h2 class="text-lg font-semibold text-white">{{ user.name || user.preferred_username }}</h2>
              <p class="text-sm text-slate-400">{{ user.email || 'Email nao informado' }}</p>
            </div>
          </div>
          <button
            @click="chamarApi"
            :disabled="loading"
            class="cursor-pointer inline-flex items-center justify-center gap-2 rounded-xl bg-emerald-500 px-5 py-2.5 text-sm font-semibold text-white shadow-lg shadow-emerald-500/25 transition hover:bg-emerald-400 active:scale-95 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <svg v-if="!loading" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" d="M4.5 12a7.5 7.5 0 0015 0m-15 0a7.5 7.5 0 1115 0m-15 0H3m16.5 0H21m-1.5 0H12m-8.457 3.077l1.41-.513m14.095-5.13l1.41-.513M5.106 17.785l1.15-.964m11.49-9.642l1.149-.964M7.501 19.795l.75-1.3m7.5-12.99l.75-1.3m-6.063 16.658l.26-1.477m2.605-14.772l.26-1.477m0 17.726l-.26-1.477M10.698 4.614l-.26-1.477M16.5 19.794l-.75-1.299M7.5 4.205L12 12m6.894 5.785l-1.149-.964M6.256 7.178l-1.15-.964m15.352 8.864l-1.41-.513M4.954 9.435l-1.41-.514M12.002 12l-3.75 6.495" />
            </svg>
            <span v-if="loading" class="h-5 w-5 animate-spin rounded-full border-2 border-white/30 border-t-white"></span>
            {{ loading ? 'Chamando...' : 'Chamar API (SPA)' }}
          </button>
        </div>
      </div>

      <!-- SPA API result -->
      <div
        v-if="apiResult"
        class="overflow-hidden rounded-2xl border border-white/10 bg-white/5 backdrop-blur-sm"
      >
        <div class="flex items-center gap-2 border-b border-white/10 px-6 py-3">
          <div class="flex gap-1.5">
            <div class="h-3 w-3 rounded-full bg-red-400/60"></div>
            <div class="h-3 w-3 rounded-full bg-amber-400/60"></div>
            <div class="h-3 w-3 rounded-full bg-emerald-400/60"></div>
          </div>
          <span class="ml-2 text-xs font-medium text-slate-500">SPA: GET /api/me -> 200 OK</span>
        </div>
        <pre class="overflow-auto p-6 text-sm leading-relaxed text-slate-300">{{ JSON.stringify(apiResult, null, 2) }}</pre>
      </div>

      <!-- BFF Dashboard -->
      <div v-if="bffUser" class="rounded-2xl border border-indigo-400/20 bg-indigo-500/5 p-6 backdrop-blur-sm sm:p-8">
        <div class="mb-4 flex items-center gap-2">
          <span class="rounded-lg bg-indigo-500/20 px-2 py-0.5 text-xs font-semibold text-indigo-300">BFF</span>
          <span class="text-xs text-slate-500">Cookie HttpOnly</span>
        </div>
        <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h2 class="text-lg font-semibold text-white">{{ bffUser.Nome }}</h2>
            <p class="text-sm text-slate-400">{{ bffUser.Email || 'Email nao informado' }}</p>
          </div>
          <div class="flex gap-2">
            <button
              @click="chamarApiBff"
              :disabled="bffLoading"
              class="cursor-pointer inline-flex items-center gap-2 rounded-xl bg-indigo-500 px-4 py-2 text-sm font-semibold text-white transition hover:bg-indigo-400 active:scale-95 disabled:opacity-50"
            >
              <span v-if="bffLoading" class="h-4 w-4 animate-spin rounded-full border-2 border-white/30 border-t-white"></span>
              {{ bffLoading ? 'Chamando...' : 'Chamar API (BFF)' }}
            </button>
          </div>
        </div>
        <pre v-if="bffApiResult" class="mt-4 overflow-auto rounded-lg bg-black/30 p-4 text-sm text-slate-300">{{ JSON.stringify(bffApiResult, null, 2) }}</pre>
      </div>

      <!-- Flow diagram -->
      <div class="rounded-2xl border border-white/10 bg-white/5 p-6 backdrop-blur-sm sm:p-8">
        <h3 class="mb-4 text-sm font-semibold uppercase tracking-wider text-slate-500">Fluxo</h3>
        <div class="grid grid-cols-1 gap-3 sm:grid-cols-5">
          <div class="flex items-center gap-3 rounded-xl bg-white/5 p-3 sm:flex-col sm:text-center">
            <span class="flex h-7 w-7 items-center justify-center rounded-lg bg-indigo-500/20 text-xs font-bold text-indigo-300">1</span>
            <span class="text-xs text-slate-400">Redireciona para Keycloak</span>
          </div>
          <div class="hidden items-center justify-center text-slate-700 sm:flex">-></div>
          <div class="flex items-center gap-3 rounded-xl bg-white/5 p-3 sm:flex-col sm:text-center">
            <span class="flex h-7 w-7 items-center justify-center rounded-lg bg-indigo-500/20 text-xs font-bold text-indigo-300">2</span>
            <span class="text-xs text-slate-400">Login no Keycloak</span>
          </div>
          <div class="hidden items-center justify-center text-slate-700 sm:flex">-></div>
          <div class="flex items-center gap-3 rounded-xl bg-white/5 p-3 sm:flex-col sm:text-center">
            <span class="flex h-7 w-7 items-center justify-center rounded-lg bg-indigo-500/20 text-xs font-bold text-indigo-300">3</span>
            <span class="text-xs text-slate-400">Auth code -> Tokens</span>
          </div>
          <div class="hidden items-center justify-center text-slate-700 sm:flex">-></div>
          <div class="flex items-center gap-3 rounded-xl bg-white/5 p-3 sm:flex-col sm:text-center">
            <span class="flex h-7 w-7 items-center justify-center rounded-lg bg-indigo-500/20 text-xs font-bold text-indigo-300">4</span>
            <span class="text-xs text-slate-400">Chama API com Bearer</span>
          </div>
          <div class="hidden items-center justify-center text-slate-700 sm:flex">-></div>
          <div class="flex items-center gap-3 rounded-xl bg-white/5 p-3 sm:flex-col sm:text-center">
            <span class="flex h-7 w-7 items-center justify-center rounded-lg bg-emerald-500/20 text-xs font-bold text-emerald-300">5</span>
            <span class="text-xs text-slate-400">API valida e responde</span>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>
