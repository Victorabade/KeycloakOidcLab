<script setup lang="ts">
import { ref, onMounted } from 'vue';
import userManager from '../services/auth';

const user = ref<any>(null);
const apiResult = ref<any>(null);

onMounted(async () => {
  const usuario = await userManager.getUser();
  if (usuario && !usuario.expired) {
    user.value = usuario.profile;
  }
});

async function login() {
  await userManager.signinRedirect();
}

async function logout() {
  await userManager.signoutRedirect();
  user.value = null;
  apiResult.value = null;
}

async function chamarApi() {
  const usuario = await userManager.getUser();
  if (!usuario || usuario.expired) {
    alert('Faça login primeiro');
    return;
  }

  const res = await fetch('http://localhost:5000/api/me', {
    headers: { Authorization: `Bearer ${usuario.access_token}` },
  });

  if (res.ok) {
    apiResult.value = await res.json();
  } else {
    alert('Erro: ' + (await res.text()));
  }
}
</script>

<template>
  <div style="font-family: sans-serif; max-width: 600px; margin: 40px auto">
    <h1>SSO com Keycloak + .NET + Vue</h1>

    <div v-if="!user">
      <button @click="login">Login</button>
    </div>

    <div v-else>
      <p><strong>Logado como:</strong> {{ user.name || user.preferred_username }}</p>
      <p><strong>Email:</strong> {{ user.email }}</p>
      <button @click="logout" style="margin-right: 8px">Logout</button>
      <button @click="chamarApi">Chamar API</button>
    </div>

    <pre v-if="apiResult" style="background: #f4f4f4; padding: 16px; margin-top: 24px">
{{ JSON.stringify(apiResult, null, 2) }}
    </pre>
  </div>
</template>
