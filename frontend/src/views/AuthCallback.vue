<script setup lang="ts">
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'
import userManager from '../services/auth'

const router = useRouter()

onMounted(async () => {
  const params = new URLSearchParams(window.location.search)
  const code = params.get('code')
  const sessionState = params.get('session_state')
  const iss = params.get('iss')
  const state = params.get('state')
  const error = params.get('error')

  console.groupCollapsed(
    '%c[auth-callback] %cCallback recebido',
    'font-weight:bold;color:#c084fc',
    'font-weight:bold;color:#e9d5ff',
  )
  console.log('URL:', window.location.href)
  console.table({
    code: code ? `${code.substring(0, 30)}...` : '(ausente)',
    session_state: sessionState ?? '(ausente)',
    iss: iss ?? '(ausente)',
    state: state ? `${state.substring(0, 20)}...` : '(ausente)',
    error: error ?? '(nenhum)',
  })
  console.groupEnd()

  if (error) {
    console.error('%c[auth-callback] %cErro retornado pelo Keycloak',
      'font-weight:bold;color:#c084fc', 'color:#ef4444',
      { error, error_description: params.get('error_description') },
    )
    router.push('/')
    return
  }

  try {
    console.log('%c[auth-callback] %cTrocando authorization code por tokens...',
      'font-weight:bold;color:#c084fc', 'color:#94a3b8',
    )
    const user = await userManager.signinCallback()
    console.log(
      '%c[auth-callback] %cToken exchange concluido',
      'font-weight:bold;color:#c084fc',
      'font-weight:bold;color:#34d399',
      { profile: user?.profile },
    )
  } catch (err) {
    console.error(
      '%c[auth-callback] %cFalha no signinCallback',
      'font-weight:bold;color:#c084fc',
      'color:#ef4444',
      err,
    )
  } finally {
    router.push('/')
  }
})
</script>

<template>
  <div class="flex min-h-screen items-center justify-center bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900 px-4">
    <div class="text-center space-y-6">
      <div class="mx-auto h-12 w-12 animate-spin rounded-full border-4 border-white/10 border-t-indigo-400"></div>
      <p class="text-lg font-medium text-white">Autenticando...</p>
      <p class="text-sm text-slate-400">Processando o callback do Keycloak</p>
    </div>
  </div>
</template>
